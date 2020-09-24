using Microsoft.AspNetCore.Authorization;

namespace IdentityTests.Infra
{
    public class AuthorizeAnyAppRolePolicyAttribute : AuthorizeAttribute
    {
        public AuthorizeAnyAppRolePolicyAttribute() : base("UserOfAnyRolePolicy") { }
    }
}
