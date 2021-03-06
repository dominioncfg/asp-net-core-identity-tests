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
        private readonly UserManager<AppIdentityUser> _userManager;
        private readonly SignInManager<AppIdentityUser> _signInManager;
        private readonly IEmailService _emailService;

        public AuthController(
                                UserManager<AppIdentityUser> userManager,
                                SignInManager<AppIdentityUser> signInManager,
                                IEmailService emailService
                             )
        {
            this._userManager = userManager;
            this._emailService = emailService;
            this._signInManager = signInManager;
        }

        private async Task SendConfirmationEmailAsync(AppIdentityUser user)
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            IActionResult result = View(model);
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser == null)
                {
                    var user = new AppIdentityUser
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Age = model.Age,
                    };

                    var creteResult = await _userManager.CreateAsync(user, model.Password);

                    if (creteResult.Succeeded)
                    {
                        user = await _userManager.FindByEmailAsync(model.Email);

                        await SendConfirmationEmailAsync(user);
                        await _userManager.AddToRoleAsync(user, Roles.RegularUser.ToString());

                        result = RedirectToAction(nameof(SignIn));
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(SignUp), string.Join(". ", creteResult.Errors.Select(x => x.Description)));
                    }
                }
                else
                {
                    ModelState.AddModelError(nameof(model.Email), "User Already Exists");
                }
            }

            return result;
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInViewModel model, [FromQuery(Name = "ReturnUrl")] string returnUrl = null)
        {
            IActionResult result = View(model);

            if (ModelState.IsValid)
            {
                string email = model.Email;
                var user = await _userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    var signInResult = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);
                    if (signInResult.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            result = LocalRedirect(returnUrl);
                        }
                        else
                        {
                            result = RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        bool isConfirmed = await _userManager.IsEmailConfirmedAsync(user);
                        if (!isConfirmed)
                        {
                            await this.SendConfirmationEmailAsync(user);
                            ModelState.AddModelError("Login", "Your Account is not Confirmed! Check your Email!");
                        }
                        else
                        {
                            ModelState.AddModelError("Login", "Invalid Password.");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("Email", "Invalid User Account.");
                }
            }
            return result;
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

        #region Forgot Password
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            var viewModel = new ForgotPasswordViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel viewModel)
        {
            IActionResult result = View(viewModel);
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(viewModel.Email);
                if (user == null)
                {
                    ModelState.AddModelError("Email", "There is no account for that email");
                }
                else
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var callback = Url.ActionLink(nameof(ResetPassword), "Auth", new { token, email = user.Email });
                    string body = $"<a href='{callback}'>Click to reset your password</a> or open in the browser {callback}";
                    await _emailService.SendEmailAsync("info@mydomain.com", user.Email, "Reset Your Account Password", body);
                    result = View("ForgotPasswordConfirmation");
                }
            }
            return result;
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            var viewModel = new ResetPasswordViewModel() { Email = email, Token = token };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel)
        {
            IActionResult result = View(viewModel);

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(viewModel.Email);
                if (user != null)
                {
                    var resetPassResult = await _userManager.ResetPasswordAsync(user, viewModel.Token, viewModel.Password);
                    if (!resetPassResult.Succeeded)
                    {
                        foreach (var error in resetPassResult.Errors)
                        {
                            ModelState.TryAddModelError(error.Code, error.Description);
                        }
                    }
                    else
                    {
                        result = RedirectToAction(nameof(SignIn));
                    }
                }
                else
                {
                    ModelState.AddModelError("Email", "There is no account for that email");
                }
            }

            return result;
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
