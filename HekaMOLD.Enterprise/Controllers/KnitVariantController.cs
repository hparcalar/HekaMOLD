using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    public class KnitVariantController : Controller
    {
        // GET: KnitVariant
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult BindModel(int rid, int vid)
        {
            ItemVariantModel model= null;
            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                model = bObj.Get();
            }

            var jsonResponse = Json(model, JsonRequestBehavior.AllowGet);
            jsonResponse.MaxJsonLength = int.MaxValue;

            return jsonResponse;
        }
    }
}