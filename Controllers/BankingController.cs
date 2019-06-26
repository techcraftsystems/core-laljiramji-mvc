using System;
using System.Collections.Generic;
using Core.Services;
using Core.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class BankingController : Controller
    {
        // GET: /<controller>/
        [Route("bank")]
        public IActionResult Index(BankingIndexViewModel model, AccountsService service)
        {
            model.Banks = service.GetBanks();
            return View(model);
        }

        [Route("bank/{code}")]
        public IActionResult Main(string code, BankingMainViewModel model, AccountsService service)
        {
            model.Banks = service.GetBanks();
            model.Bank = service.GetBank(code);
            return View(model);
        }

        public JsonResult GetStationsReconciles(int year, int mnth, string accounts, AccountsService service, string filter = "")
        {
            DateTime date1 = new DateTime(year, mnth, 1);
            DateTime date2 = date1.AddMonths(1).AddDays(-1);

            List<BankingReconcileModel> reconciles = service.GetBankingReconciles(date1, date2, accounts, filter);
            return Json(reconciles);
        }

    }
}
