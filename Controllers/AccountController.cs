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
using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.Linq;

namespace Core.Controllers
{
    public class AccountController : Controller
    {
        [BindProperty]
        public LoginModel Input { get; set; }

        [BindProperty]
        public UsersViewModel UserView { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> Login(LoginModel model, string ReturnUrl = "/") {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            model.ReturnUrl = ReturnUrl;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model, UserService Svc, CrytoUtilsExtensions Cryto){
            if (ModelState.IsValid) {
                Users user = Svc.GetUser(Input.User.Username); //AuthenticateUser(Input.Email, Input.Password);
                if (user == null) {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    model.Message = "Invalid login attempt.";
                    return View(model);
                }

                if (!user.Enabled){
                    ModelState.AddModelError(string.Empty, "Login account Disabled.");
                    model.Message = "Login account Disabled.";
                    return View(model);
                }

                if (!Cryto.Decrypt(user.Password).Equals(Input.User.Password)){
                    ModelState.AddModelError(string.Empty, "Login Failed. Invalid password.");
                    model.Message = "Login Failed. Invalid password.";
                    return View(model);
                }

                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Sid, user.Uuid),
                    new Claim(ClaimTypes.UserData, user.Username),
                    new Claim(ClaimTypes.Actor, user.Id.ToString())
                };

                if (string.IsNullOrEmpty(model.Password)) {
                    if (user.ToChange) {
                        model.ToChange = 1;
                        return View(model);
                    }
                }
                else {
                    user.UpdatePassword(Cryto.Encrypt(model.Password));
                }

                foreach (var roles in user.GetRoles()) {
                    claims.Add(new Claim(ClaimTypes.Role, roles.Role.Name));
                }

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60),
                    IsPersistent = true,
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                if (!string.IsNullOrEmpty(Input.ReturnUrl.Trim()))
                    return LocalRedirect(Input.ReturnUrl.Trim());

                user.UpdateLastAccess();
                model.User.Password = null;

                return LocalRedirect("/");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [Authorize]
        public IActionResult AccessDenied(LoginModel model, string ReturnUrl = "/") {
            string comp = Dns.GetHostName().Split(".").ToList().First();
            string ipaddr = Dns.GetHostAddresses(Dns.GetHostName())[0].ToString();

            model.ReturnUrl = ReturnUrl;
            return View(model);
        }

        [Route("/accounts")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Index() {
            return View();
        }

        [Route("/accounts/security")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Security() {
            return View();
        }

        [Route("/accounts/users")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Users(UsersViewModel model, UserService service) {
            model.Users = service.GetUsers();
            return View(model);
        }

        [Authorize]
        [Route("/accounts/users/{uuid}")]
        public IActionResult UsersView(string uuid, UsersViewModel model, string status = "") {
            model.User = new UserService().GetUserByUuid(uuid);
            model.User.Password = null;

            if (status.Equals("403"))
                model.Message = "Action Failed. Invalid Password";
            if (status.Equals("ok"))
                model.Message = "Password changed successfully";

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public IActionResult ChangePassword(UserService service, CrytoUtilsExtensions Cryto) {
            Users user = service.GetUser(UserView.User.Username);
            if (!Cryto.Decrypt(user.Password).Equals(UserView.User.Password)) 
                return LocalRedirect("/accounts/users/" + user.Uuid + "?status=403");

            user.UpdatePassword(Cryto.Encrypt(UserView.Password));

            if (user.Id.Equals(int.Parse(HttpContext.User.FindFirst(ClaimTypes.Actor).Value)))
                return LocalRedirect("/Account/Logout");

            return LocalRedirect("/accounts/users/" + user.Uuid + "?status=ok");
        }

        [Authorize]
        public string ResetPassword(string uuid) {
            new UserService().GetUserByUuid(uuid).ResetPassword();
            return "success";
        }

        [Authorize]
        public string EnableAccount(string uuid, int opts) {
            bool option = opts != 0;
            new UserService().GetUserByUuid(uuid).EnableAccount(option);
            return "success";
        }

    }
}
