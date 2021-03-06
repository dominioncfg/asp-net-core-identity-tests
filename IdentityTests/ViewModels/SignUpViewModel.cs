using System.ComponentModel.DataAnnotations;

namespace IdentityTests.ViewModels
{
    public class SignUpViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }
       
        [Required]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required]
        public int Age { get; set; }
    }
}
