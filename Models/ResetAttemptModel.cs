using System.ComponentModel.DataAnnotations;

namespace LoginAttemptDemo.Models
{
    public class ResetAttemptModel
    {
        [Key]
        public int attempt_id { get; set; }
        public int employee_id { get; set; }
        public string ip_address { get; set; }
        public string cookie{ get; set; }
        public int step_number { get; set; }
        public DateTime attempt_time { get; set; }
        public string reset_response { get; set; }
    }
}
