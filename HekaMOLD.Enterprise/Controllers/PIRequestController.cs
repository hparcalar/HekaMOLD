using HekaMOLD.Enterprise.Controllers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    [UserAuthFilter]
    public class PIRequestController : Controller
    {
        // GET: PIRequest
        public ActionResult Index()
        {
            return View();
        }
    }
}