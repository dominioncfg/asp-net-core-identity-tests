using IdentityTests.Configuration;
using IdentityTests.Models;
using IdentityTests.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace IdentityTests.Components
{
    public class NavBarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var userPrincipal = User as ClaimsPrincipal;
            string strRole = userPrincipal?.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?.Value;
            string fullName = userPrincipal?.Claims.FirstOrDefault(claim => claim.Type == ConfigurationConstants.Claims.FullName)?.Value;

            Roles role = Roles.RegularUser;
            if (!string.IsNullOrEmpty(strRole))
            {
                bool parsed = Enum.TryParse(strRole, out role);
            }

            NavBarViewModel viewModel = new NavBarViewModel()
            {
                IsLoggedIn = User.Identity.IsAuthenticated,
                UserName = User.Identity.Name,                
                CurrentRole = role,
                UserFullName = fullName,                
            };
            return View(viewModel);
        }
    }
}
