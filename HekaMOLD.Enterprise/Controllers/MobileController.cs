using HekaMOLD.Enterprise.Controllers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HekaMOLD.Enterprise.Helpers;

namespace HekaMOLD.Enterprise.Controllers
{
    [MobileAuthFilter]
    public class MobileController : Controller
    {
        // GET: Mobile
        public ActionResult Index()
        {
            if (this.IsGranted("MobileProductionUser"))
                return RedirectToAction("Production");
            else if (this.IsGranted("MobileMechanicUser"))
                return RedirectToAction("Mechanic");
            else if (this.IsGranted("MobileWarehouseUser"))
                return RedirectToAction("Warehouse");

            return View();
        }

        #region HOME SCREENS
        public ActionResult Warehouse()
        {
            return View();
        }

        public ActionResult Production()
        {
            return View();
        }

        public ActionResult Mechanic()
        {
            return View();
        }
        #endregion

        #region ITEM ENTRY
        public ActionResult ItemEntry()
        {
            return View();
        }
        #endregion
    }
}