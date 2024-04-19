using Dapper;
using LoginAttemptDemo.Models;
using Npgsql;
using System;
using System.Runtime.InteropServices;

namespace LoginAttemptDemo.Data
{
    public class ResetPasswordService
    {
        private readonly IConfiguration _config;
        public ResetPasswordService(IConfiguration config)
        {
            _config = config;
        }
        public async Task<ResetPasswordResponse> ResetPassword(UserVerification userVerification, UserModel userModel)
        {
            var userVerified = await VerifyUser(userVerification, userModel);
            var returnData = new ResetPasswordResponse();

            if (userVerified)
            {
                // API Call to Reset user's password
                returnData.APICallSuccessful = true;

                returnData.ResetSuccessful = true;
                returnData.ResponseMessage = "Your password has been successfully reset";                
            }
            else
            {               
                returnData.ResetSuccessful = false;
                returnData.APICallSuccessful = false;
                returnData.ResponseMessage = "The information you entered does not match the information we have on file. Please try again or contact the administrator";                
            }

            return returnData;
        }

        public async Task<bool> VerifyUser(UserVerification userVerification, UserModel user)
        {
            var userVerified = false;
            var last4_ssn = Int32.Parse(userVerification.SSNLast4);
            var parameters = new { dob = userVerification.DOB, last4_ssn = last4_ssn };
            var whereClause = @"WHERE date_of_birth = @dob 
                                AND last4_ssn = @last4_ssn";            

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
                                {whereClause}
                                LIMIT 1";

                    var userData = await connection.QueryAsync<UserModel>(sql, parameters);
                    if(userData.Count() > 0)
                    {
                        if(user.date_of_birth == userData.First().date_of_birth && user.last4_ssn == userData.First().last4_ssn)
                        {
                            userVerified = true;
                        }
                    }

                    return userVerified;
                }
                catch (Exception ex)
                {
                    return userVerified;
                }
            }
        }
    }
}
