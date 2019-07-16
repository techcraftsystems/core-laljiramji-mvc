using System;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers 
{
    [Authorize]
    public class SuppliersController : Controller 
    {
        private readonly CoreService Core = new CoreService();

        [Route("/suppliers")]
        public IActionResult Index() {
            return View(Core.GetSuppliers());
        }

        [Route("/suppliers/{uuid}")]
        public IActionResult Supplier(string uuid) {
            return View();
        }
    }
}
