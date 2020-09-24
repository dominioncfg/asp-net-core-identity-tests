using IdentityTests.Infra;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityTests.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult AnyLoggedOnUser()
        {
            return View();
        }

        [AuthorizeAnyAppRolePolicy]
        public IActionResult AnyUserOfAnyRole()
        {
            return View();
        }

        [AuthorizeAdminUserOnlyPolicy]
        public IActionResult OnlyAdminRole()
        {
            return View();
        }


    }
}
