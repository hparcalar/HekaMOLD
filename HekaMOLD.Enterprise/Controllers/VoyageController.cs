using HekaMOLD.Business.Models.DataTransfer.Logistics;
using HekaMOLD.Business.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    public class VoyageController : Controller
    {
        // GET: Voyage
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List()
        {
            return View();
        }
        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            VoyageModel model = null;
            using (VoyageBO bObj = new VoyageBO())
            {
                model = bObj.GetVoyage(rid);
            }

            var jsonResponse = Json(model, JsonRequestBehavior.AllowGet);
            jsonResponse.MaxJsonLength = int.MaxValue;

            return jsonResponse;
        }

        [HttpGet]
        public JsonResult GetVoyageList()
        {
            VoyageModel[] result = new VoyageModel[0];

            using (VoyageBO bObj = new VoyageBO())
            {
                result = bObj.GetVoyageList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetVoyageDetailList()
        {
            VoyageDetailModel[] result = new VoyageDetailModel[0];

            using (VoyageBO bObj = new VoyageBO())
            {
                result = bObj.GetVoyageDetailList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
    }
}