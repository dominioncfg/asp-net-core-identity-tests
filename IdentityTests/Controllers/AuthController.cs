using IdentityTests.Models;
using IdentityTests.Services.Email;
using IdentityTests.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityTests.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailService _emailService;

        public AuthController(
                                UserManager<IdentityUser> userManager, 
                                SignInManager<IdentityUser> signInManager, 
                                IEmailService emailService
                             )
        {
            this._userManager = userManager;
            this._emailService = emailService;
            this._signInManager = signInManager;
        }

        private async Task SendConfirmationEmailAsync(IdentityUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.ActionLink(nameof(ConfirmEmail), "Auth", new { userId = user.Id, @token = token });
            string body = $"<a href='{confirmationLink}'>Confirm Email</a> or Copy this Url {confirmationLink}.";
            await _emailService.SendEmailAsync("info@mydomain.com", user.Email, "Confirm your email address", body);
        }

        #region Sign Up
        [HttpGet]
        public IActionResult SignUp()
        {
            return View(new SignUpViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser == null)
                {
                    var user = new IdentityUser
                    {
                        Email = model.Email,
                        UserName = model.Email
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        user = await _userManager.FindByEmailAsync(model.Email);
                        await SendConfirmationEmailAsync(user);

                        await _userManager.AddToRoleAsync(user, Roles.RegularUser.ToString());

                        return RedirectToAction(nameof(SignIn));
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(SignUp), string.Join(". ", result.Errors.Select(x => x.Description)));
                    }
                }
                else
                {
                    ModelState.AddModelError(nameof(model.Email), "User Already Exists");
                }
            }

            return View(model);
        }
        #endregion

        #region Confirm Email
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(SignIn));
                }
                else
                {
                    return BadRequest("Invalid Token");
                }
            }
            return NotFound("User does not exist");
        }
        #endregion

        #region Sign In
        [HttpGet]
        public IActionResult SignIn([FromQuery(Name = "ReturnUrl")] string returnUrl = null)
        {
            var viewModel = new SignInViewModel()
            {
                ReturnUrl = returnUrl,
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model, [FromQuery(Name = "ReturnUrl")] string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                string userName = model.Email;
                var result = await _signInManager.PasswordSignInAsync(userName, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    var existingUser = await _userManager.FindByEmailAsync(model.Email);
                    bool isConfirmationIssue = false;
                    if (existingUser != null)
                    {
                        bool isConfirmed = await _userManager.IsEmailConfirmedAsync(existingUser);
                        if (!isConfirmed)
                        {
                            await this.SendConfirmationEmailAsync(existingUser);
                            isConfirmationIssue = true;
                        }
                    }

                    if (!isConfirmationIssue)
                    {
                        ModelState.AddModelError("Login", "Cannot login.");
                    }

                }
            }
            return View(model);
        }
        #endregion

        #region Sign Out
        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(SignIn));
        }
        #endregion

        #region Access Denied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        #endregion
    }
}
