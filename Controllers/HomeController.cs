using System.Collections.Generic;
using System.Diagnostics;
using Core.ViewModel;
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Core.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index(StationsService svc)
        {
            IndexViewModel model = new IndexViewModel();
            model.Pending = new List<Stations>(svc.GetPendingPush());
            model.Updated = new List<Stations>(svc.GetUpdatedPush());

            return View(model);
        }

        public IActionResult Placeholder()
        {
            ViewData["Message"] = "Your application description page.";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
