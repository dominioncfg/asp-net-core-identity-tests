using IdentityTests.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IdentityTests.Components
{
    public class NavBarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            NavBarViewModel viewModel = new NavBarViewModel()
            {
                IsLoggedIn = User.Identity.IsAuthenticated,
                UserName = User.Identity.Name
            };
            return View(viewModel);
        }
    }
}
