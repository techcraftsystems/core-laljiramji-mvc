using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers 
{
    [Authorize]
    public class SuppliersController : Controller 
    {
        [Route("core/suppliers")]
        public IActionResult Index() {
            return View();
        }

        [Route("core/suppliers/{uuid}")]
        public IActionResult Supplier(string uuid) {
            return View();
        }
    }
}
