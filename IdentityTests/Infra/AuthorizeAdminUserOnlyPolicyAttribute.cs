using Microsoft.AspNetCore.Authorization;

namespace IdentityTests.Infra
{
    public class AuthorizeAdminUserOnlyPolicyAttribute : AuthorizeAttribute
    {
        public AuthorizeAdminUserOnlyPolicyAttribute() : base("AdminUserOnlyPolicy") { }
    }
}
