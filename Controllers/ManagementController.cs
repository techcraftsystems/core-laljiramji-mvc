using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Core.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ManagementController : Controller
    {
        [Route("management")]
        public IActionResult Index() {
            return View();
        }

        [Route("management/finance")]
        public IActionResult Finance() {
            return View();
        }
    }
}
