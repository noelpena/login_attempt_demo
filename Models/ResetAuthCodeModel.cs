using System.Text;
using Dapper;

namespace LoginAttemptDemo.Models
{
    public class ResetAuthCodeModel
    {
        public int id { get; set; }
        public int employee_id { get; set; }
        public string work_email { get; set; }
        public string code { get; set; } = GenerateAuthorizationCode();
        public DateTime expiration_date { get; set; } = DateTime.Now.AddMinutes(10);


        public ResetAuthCodeModel(
            int _employeeId = -1,
            string _workEmail = ""
        )
        {
            employee_id = _employeeId;
            work_email = _workEmail;
        }

        public static string GenerateAuthorizationCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var codeBuilder = new StringBuilder();

            for (int i = 0; i < 6; i++)
            {
                int index = random.Next(chars.Length);
                codeBuilder.Append(chars[index]);
            }

            return codeBuilder.ToString();
        }
    }

    public class ResetAuthCodeDefaultModel
    {
        public int id { get; set; }
        public int employee_id { get; set; }
        public string work_email { get; set; }
        public string code { get; set; }
        public DateTime expiration_date { get; set; }
    }

        public class ResetAuthCodeReturnObject
    {
        public bool IsExpired { get; set; } = false;
        public bool IsIncorrect { get; set; } = false;
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "";
    }
}
