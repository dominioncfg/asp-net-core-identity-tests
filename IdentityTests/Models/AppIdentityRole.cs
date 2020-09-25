using Microsoft.AspNetCore.Identity;

namespace IdentityTests.Models
{
    public class AppIdentityRole : IdentityRole<long>
    {
        public AppIdentityRole() : base() { }
        public AppIdentityRole(string roleName) : base(roleName) { }
    }
}
