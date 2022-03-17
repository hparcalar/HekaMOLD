using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using HekaMOLD.Enterprise.Controllers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    [BossAuthFilter]
    public class BossController : Controller
    {
        // GET: Boss
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SettingsBoss()
        {
            return View();
        }

        public ActionResult ProductionStatus()
        {
            return View();
        }

        public ActionResult Warehouse()
        {
            return View();
        }

        public ActionResult RedirectSaleReceipts()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ChangeReceiptStatus(int itemReceiptDetailId, int otrStatus)
        {
            try
            {
                BusinessResult result = null;
                using (ReceiptBO bObj = new ReceiptBO())
                {
                    result = bObj.ChangeOTRStatus(itemReceiptDetailId, otrStatus);
                }

                if (result.Result)
                    return Json(new { Status = 1 });
                else
                    throw new Exception(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                return Json(new { Status = 0, ErrorMessage = ex.Message });
            }
        }
    }
}