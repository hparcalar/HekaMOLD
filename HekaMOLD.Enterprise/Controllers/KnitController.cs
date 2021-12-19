using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    public class KnitController : Controller
    {
        // GET: Knit
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetKnitList()
        {
            ItemModel[] result = new ItemModel[0];
            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                result = bObj.GetItemList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
    }
}