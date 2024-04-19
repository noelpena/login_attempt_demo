using Dapper;

namespace LoginAttemptDemo.Models
{
    public class UserModel
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string work_email { get; set; }
        public string personal_email { get; set; }
        public int employee_id { get; set; }
        public int last4_ssn { get; set; }
        public DateTime date_of_birth { get; set; }
        public int password { get; set; }
        public DateTime created_at { get; set; }
    }

    public class UserContainerModel
    {
        public UserModel User { get; set; } = new UserModel();
        public bool success { get; set; }
        public bool failed { get; set; }
        public string? errorMessage { get; set; }
        public void notFound()
        {
            success = false;
            failed = true;
            errorMessage = "User lookup failed. Email address or Employee ID is incorrect.";
        }
        public bool load(dynamic userData)
        {
            bool returnVal = true;
            try
            {
                User.employee_id = userData.employee_id;
                User.personal_email = userData.personal_email;
                User.first_name = userData.first_name;
                User.last_name = userData.last_name;
                User.work_email = userData.work_email;
                User.date_of_birth = userData.date_of_birth;
                User.last4_ssn = userData.last4_ssn;
                success = true;
                failed = false;
            }
            catch
            {
                returnVal = false;
                success = false;
                failed = true;
            }

            return returnVal;
        }
    }
}

