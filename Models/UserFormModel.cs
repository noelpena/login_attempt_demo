using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace LoginAttemptDemo.Models
{
    public class UserFormModel
    {
        [Display(Name = "Employee ID or Personal Email")]
        public string employeeID { get; set; }

        [Display(Name = "Work Email")]
        public string workEmail { get; set; }
    }

    public class UserAuthCode
    {
        [Required]
        [Display(Name = "Authorization Code")]
        [RegularExpression(@"^[A-Za-z0-9]{6}$", ErrorMessage = "Field must be 6 alphanumeric characters.")]
        public string AuthCode { get; set; }
    }

    public class UserVerification
    {
        [Required]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Only numbers are allowed.")]
        [Display(Name = "Last 4 Of SSN")]
        public string SSNLast4 { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        public DateTime? DOB { get; set; } = null;

        [Display(Name = "Reset Type")]
        public string ResetType { get; set; } = "";
    }

    public enum ResetType
    {
        password,
        mfa,
        both
    }
}
