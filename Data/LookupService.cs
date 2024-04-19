using Dapper;
using LoginAttemptDemo.Models;
using Npgsql;

namespace LoginAttemptDemo.Data
{
    public class LookupService
    {
        private readonly IConfiguration _config;
        public LookupService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<UserContainerModel> UserLookup(string email = "", string employee_id = "")
        {
            var user = new UserContainerModel();
            var whereClause = "";
            if (!string.IsNullOrEmpty(email))
            {
                whereClause = $"WHERE personal_email = '{email.ToLower()}'";
            }
            else
            {
                whereClause = $"WHERE employee_id = {Int32.Parse(employee_id)}";
            }

            using (var connection = new NpgsqlConnection(_config.GetConnectionString("Default")))
            {
                try
                {
                    var sql = $@"SELECT 
                                    id
                                    ,first_name
                                    ,last_name
                                    ,work_email
                                    ,personal_email
                                    ,employee_id
                                    ,date_of_birth
                                    ,last4_ssn
                                    ,created_at 
                                FROM login_attempt_demo_users
                                {whereClause}";

                    var userData = await connection.QueryFirstAsync<UserModel>(sql);
                    var userLoaded = user.load(userData);

                    if(!userLoaded)
                    {
                        user.notFound();
                    }
                 
                    return user;
                } 
                catch (Exception ex)
                {
                    return user;
                }
            }
        }

        public async Task<ResetAuthCodeReturnObject> ValidateAuthorizationCode(string codeEntered, UserContainerModel user)
        {
            var returnObj = new ResetAuthCodeReturnObject();
            var storedCode = "";
            DateTime expirationDate = DateTime.Now;

            var parameters = new { employee_id = user.User.employee_id };
            var sql = @$"SELECT
                            id,
                            employee_id,
                            work_email,
                            code,
                            expiration_date
                        FROM login_attempt_demo_authcodes
                        WHERE employee_id = @employee_id
                        ORDER BY expiration_date desc";

            using (var connection = new NpgsqlConnection(_config.GetConnectionString("Default")))
            {
                var resetAuthCodeList = await connection.QueryAsync<ResetAuthCodeDefaultModel>(sql, parameters);
                //var resetAuthCodeList = await connection.GetListAsync<ResetAuthCodeDefaultModel>(whereClause);
                var resetAuthCode = resetAuthCodeList.First();
                if (resetAuthCode != null)
                {
                    storedCode = resetAuthCode.code;
                    expirationDate = resetAuthCode.expiration_date;

                    if (expirationDate > DateTime.Now)
                    {
                        if (storedCode == codeEntered)
                        {
                            returnObj.Success = true;
                        }
                        else
                        {
                            returnObj.IsIncorrect = true;
                            returnObj.Message = "<strong>Validation Failed.</strong> Authorization code is incorrect.";
                        }
                    }
                    else
                    {
                        returnObj.IsExpired = true;
                        returnObj.Message = "<strong>Validation Failed.</strong> Authorization code has expired.";
                    }

                    return returnObj;
                }
            }

            return returnObj;
        }

    }
}
