using System;
using Core.Models;
using Core.Services;
using Core.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers 
{
    [Authorize]
    public class SuppliersController : Controller 
    {
        private readonly CoreService Core = new CoreService();

        [Route("/core/suppliers")]
        public IActionResult Index() {
            return View(Core.GetSuppliers());
        }

        [Route("/core/suppliers/{uuid}")]
        public IActionResult Supplier(string uuid, SuppliersViewModel model) {
            model.Supplier = Core.GetSupplier(uuid);

            return View(model);
        }

        public JsonResult GetSupplierExpenses(long supp, string start, string stop, string filter = "") {
            if (string.IsNullOrWhiteSpace(filter))
                filter = "";
            return Json(Core.GetCoreExpenses(DateTime.Parse(start), DateTime.Parse(stop), new Suppliers(supp), filter));
        }
    }
}
