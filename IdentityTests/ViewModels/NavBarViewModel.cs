using IdentityTests.Models;
namespace IdentityTests.ViewModels
{
    public class NavBarViewModel
    {
        public bool IsLoggedIn { get; set; }
        public string UserName { get; set; }
        public string UserFullName { get; set; }
        public Roles CurrentRole { get; set; }
    }
}
