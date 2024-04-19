using Dapper;

namespace LoginAttemptDemo.Models
{
    public class ResetAttemptModel
    {
        public int attempt_id { get; set; }
        public string employee_id_email { get; set; }
        public string ip_address { get; set; }
        public string cookie{ get; set; }
        public int step_number { get; set; }
        public DateTime attempt_time { get; set; }
        public string reset_response { get; set; } = "";
    }
}
