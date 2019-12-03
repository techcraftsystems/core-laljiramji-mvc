using System.Collections.Generic;
using System.Diagnostics;
using Core.ViewModel;
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Core.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index(StationsService svc)
        {

            IndexViewModel model = new IndexViewModel {
                Pending = new List<Stations>(svc.GetPendingPush()),
                Updated = new List<Stations>(svc.GetUpdatedPush())
            };

            return View(model);
        }

        public IActionResult Placeholder() {
            ViewData["Message"] = "Your application description page.";
            return View();
        }

        public IActionResult Contact() {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy() {
            return View();
        }

        [AllowAnonymous]
        [Route("/core/tools")]
        public IActionResult Tools()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public string GetCurrentUser(HttpContext context) {
            return context.User.FindFirst(ClaimTypes.UserData).Value;
        }
    }
}
