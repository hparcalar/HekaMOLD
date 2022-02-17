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
    }
}