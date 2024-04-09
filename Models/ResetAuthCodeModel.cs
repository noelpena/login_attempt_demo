namespace LoginAttemptDemo.Models
{
    public class ResetAuthCodeModel
    {
        public int id { get; set; }
        public int employee_id { get; set; }
        public string work_email { get; set; }
        public string code { get; set; }
        public DateTime expiration_date { get; set; }
    }
}
