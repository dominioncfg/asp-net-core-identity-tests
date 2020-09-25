using IdentityTests.Configuration;
using IdentityTests.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityTests.Infra
{
    public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<AppIdentityUser, AppIdentityRole>
    {
        public AppClaimsPrincipalFactory(UserManager<AppIdentityUser> userManager, RoleManager<AppIdentityRole> roleManager, IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {

        }
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AppIdentityUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim(ConfigurationConstants.Claims.FullName, $"{user.FirstName} {user.LastName}"));
            return identity;
        }
    }
}
