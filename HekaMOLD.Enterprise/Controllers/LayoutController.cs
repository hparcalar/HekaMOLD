using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HekaMOLD.Business.Models.DataTransfer.Layout;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using HekaMOLD.Enterprise.Controllers.Attributes;
using HekaMOLD.Enterprise.Controllers.Filters;
using System.IO;

namespace HekaMOLD.Enterprise.Controllers
{
    [UserAuthFilter]
    public class LayoutController : Controller
    {
        // GET: Layout
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult BindModel()
        {
            LayoutItemModel[] model = new LayoutItemModel[0];
            using (LayoutBO bObj = new LayoutBO())
            {
                model = bObj.GetLayoutItemList();
            }

            var jsonResponse = Json(model, JsonRequestBehavior.AllowGet);
            jsonResponse.MaxJsonLength = int.MaxValue;
            return jsonResponse;
        }
    }
}