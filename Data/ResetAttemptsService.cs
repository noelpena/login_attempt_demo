using Dapper;
using LoginAttemptDemo.Models;
using Microsoft.AspNetCore.DataProtection;
using Npgsql;
using System.Security.Cryptography;

namespace LoginAttemptDemo.Data
{
    public class ResetAttemptsService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IDataProtector _dataProtector;
        private readonly IConfiguration _config;

        public ResetAttemptsService(
            IHttpContextAccessor httpContextAccessor,
            IServiceScopeFactory scopeFactory,
            IDataProtector dataProtector,
            IConfiguration config
        )
        {
            _httpContextAccessor = httpContextAccessor;
            _scopeFactory = scopeFactory;
            _dataProtector = dataProtector;
            _config = config;
        }

        public string GenerateProtectedCookie()
        {
            byte[] randomBytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }

            string identificationValue = Convert.ToBase64String(randomBytes);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddMinutes(1440),
                IsEssential = true,
                SameSite = SameSiteMode.Strict,
                Secure = true
            };

            bool cookieExist = doesCookieExist();

            if (cookieExist)
            {
                identificationValue = GetCookie();
            }
            else
            {
                var signedIdentificationValue = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IDataProtector>().Protect(identificationValue);
                _httpContextAccessor.HttpContext.Response.Cookies.Append("Logindemo_UserIdentification", signedIdentificationValue, cookieOptions);
            }

            return identificationValue;
        }

        public string GetCookie()
        {
            var signedIdentificationValue = _httpContextAccessor.HttpContext.Request.Cookies["Logindemo_UserIdentification"];
            string identificationValue = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IDataProtector>().Unprotect(signedIdentificationValue);

            return identificationValue;
        }

        private bool doesCookieExist()
        {
            var signedIdentificationValue = _httpContextAccessor.HttpContext.Request.Cookies["Logindemo_UserIdentification"];
            if (String.IsNullOrEmpty(signedIdentificationValue))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<int> GetAttempts(string employee_id_email, string ip_address, string cookie, int step_number)
        {
            var num_of_attempts = 0;
            using (var connection = new NpgsqlConnection(_config.GetConnectionString("Default")))
            {
                try
                {
                    var parameters = new
                    {
                        ip_address = ip_address,
                        employee_id_email = employee_id_email,
                        cookie = cookie,
                        step_number = step_number
                    };
                    var sql = @"SELECT * 
                        FROM login_attempt_demo_attempts
                        WHERE ip_address = @ip_address 
                        AND employee_id_email = @employee_id_email
                        AND cookie = @cookie
                        AND step_number = @step_number
                        AND attempt_time >= NOW() - INTERVAL '5 minute'";

                    var attempts = await connection.QueryAsync<ResetAttemptModel>(sql, parameters);

                    if (attempts.Count() > 0)
                    {
                        num_of_attempts = attempts.Count();

                        // cookies check
                        var dbIdentityValue = cookie;
                        var usersIdentityValue = GetCookie();

                        if (dbIdentityValue != usersIdentityValue)
                        {
                            // cookie modified
                            num_of_attempts = 5;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var errMsg = ex.Message;
                }
            }

            return num_of_attempts;
        }

        public async Task<bool> AddAttempt(ResetAttemptModel attempt)
        {
            bool attemptAdded = false;

            using (var connection = new NpgsqlConnection(_config.GetConnectionString("Default")))
            {
                string sql = @"INSERT INTO login_attempt_demo_attempts (
                                    employee_id_email,
                                    ip_address,
                                    cookie,
                                    step_number,
                                    attempt_time
                                ) 
                                VALUES (
                                      @employee_id_email, @ip_address, @cookie, @step_number, NOW()
                                )";

                var affectedRows = await connection.ExecuteAsync(sql, attempt);

                //var newAttempt = await connection.InsertAsync(attempt);
                
                if (affectedRows > 0)
                {
                    attemptAdded = true;
                }
            }

            return attemptAdded;
        }
    }
}
