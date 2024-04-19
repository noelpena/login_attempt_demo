namespace LoginAttemptDemo.Models
{
    public class ResetPasswordResponse
    {
        public string ResponseMessage { get; set; }
        public bool ResetSuccessful { get; set; }
        public bool APICallSuccessful { get; set; } = true;
        public string ResetType { get; set; }
    }
}
