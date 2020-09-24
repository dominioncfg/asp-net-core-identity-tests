using System.ComponentModel.DataAnnotations;

namespace IdentityTests.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email  { get; set; }
    }
}
