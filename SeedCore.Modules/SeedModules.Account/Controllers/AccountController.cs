using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OrchardCore.Modules;
using OrchardCore.Users;
using OrchardCore.Users.Events;
using SeedModules.Account.Models;

namespace SeedModules.Account.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IUser> _signInManager;
        private readonly IEnumerable<ILoginFormEvent> _accountEvents;
        private readonly ILogger _logger;

        public AccountController(
            SignInManager<IUser> signInManager,
            ILogger<AccountController> logger,
            IEnumerable<ILoginFormEvent> accountEvents,
            IStringLocalizer<AccountController> stringLocalizer)
        {
            _signInManager = signInManager;
            _logger = logger;
            _accountEvents = accountEvents;

            T = stringLocalizer;
        }

        IStringLocalizer T { get; set; }

        [AppendAntiforgeryToken]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return this.Spa("login.html");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm]LoginModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation(1, "User logged in.");
                    await _accountEvents.InvokeAsync(a => a.LoggedInAsync(model.UserName), _logger);
                    return RedirectTo(returnUrl);
                }
                //if (result.RequiresTwoFactor)
                //{
                //    return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                //}
                //if (result.IsLockedOut)
                //{
                //    _logger.LogWarning(2, "User account locked out.");
                //    return View("Lockout");
                //}
                else
                {
                    ModelState.AddModelError(string.Empty, T["Invalid login attempt."]);
                    await _accountEvents.InvokeAsync(a => a.LoggingInFailedAsync(model.UserName), _logger);
                    return this.Spa("login.html");
                }
            }

            return this.Spa("login.html");
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string returnUrl)
        {
            await _signInManager.SignOutAsync();
            return RedirectTo(returnUrl);
        }

        private IActionResult RedirectTo(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return Redirect("~/");
            }
        }
    }
}