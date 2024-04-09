using Dapper;
using LoginAttemptDemo.Models;
using MySql.Data.MySqlClient;

namespace LoginAttemptDemo.Data
{
    public class LookupService
    {
        private readonly IConfiguration _config;
        public LookupService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<List<UserModel>> UserLookup(string email = "", string employee_id = "")
        {
            var users = new List<UserModel>();


            using (var connection = new MySqlConnection(_config.GetConnectionString("Default")))
            {
                try
                {
                    var sql = "SELECT * FROM users";
                    users = connection.Query<UserModel>(sql).ToList();
                } 
                catch (Exception ex)
                {

                }
            }


            return users;
        }
    }
}
