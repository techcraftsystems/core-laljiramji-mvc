using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Core.Controllers
{
    public class SuppliersController : Controller
    {
        // GET: /<controller>/
        [Route("core/suppliers")]
        public IActionResult Main()
        {
            return View();
        }

        [Route("core/suppliers/{id}")]
        public IActionResult List(Int64 id)
        {
            return View();
        }
    }
}
