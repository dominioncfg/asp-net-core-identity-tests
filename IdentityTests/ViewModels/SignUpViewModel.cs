using System.ComponentModel.DataAnnotations;

namespace IdentityTests.ViewModels
{
    public class SignUpViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email  { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
