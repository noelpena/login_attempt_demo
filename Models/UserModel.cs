using System.ComponentModel.DataAnnotations;

namespace LoginAttemptDemo.Models
{
    public class UserModel
    {
        [Key]
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string work_email { get; set; }
        public string personal_email { get; set; }
        public int employee_id { get; set; }
        public int password { get; set; }
        public DateTime created_at { get; set; }
    }
}
