using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace IdentityTests.ViewModels
{
    public class SignInViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email  { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [BindNever]
        public string ReturnUrl { get; set; }

        public bool RememberMe { get; set; }

    }
}
