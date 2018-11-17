using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;

using Core.Models;
using Core.Services;
using Core.Extensions;
using Core.ViewModel;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Core.Controllers
{
    public class AccountController : Controller
    {
        [BindProperty]
        public LoginModel Input { get; set; }
        public string ReturnLink { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> Login()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return View(new LoginModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model, UserServices Svc, CrytoUtilsExtensions Cryto){
            if (ModelState.IsValid)
            {
                Users user = Svc.GetUser(Input.User.Username); //AuthenticateUser(Input.Email, Input.Password);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    model.Message = "Invalid login attempt.";
                    return View(model);
                }

                if (!user.Enabled){
                    ModelState.AddModelError(string.Empty, "Login account Disabled.");
                    model.Message = "Login account Disabled.";
                    return View(model);
                }

                if(user.ToChange){
                    //Redirect Page to Change Password, Before Contininuing.
                }

                if (!Cryto.Decrypt(user.Password).Equals(Input.User.Password)){
                    ModelState.AddModelError(string.Empty, "Login Failed. Invalid password.");
                    model.Message = "Login Failed. Invalid password.";
                    return View(model);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.UserData, user.Username),
                    new Claim(ClaimTypes.Role, "Administrator"),
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60),
                    IsPersistent = true,
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return LocalRedirect("/");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
