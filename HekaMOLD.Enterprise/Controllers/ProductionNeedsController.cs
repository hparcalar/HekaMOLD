using HekaMOLD.Business.Models.DataTransfer.Production;
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
    [UserAuthFilter]
    public class ProductionNeedsController : Controller
    {
        // GET: ProductionNeeds
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetProductionNeeds()
        {
            WorkOrderItemNeedsModel[] result = new WorkOrderItemNeedsModel[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                result = bObj.GetWorkOrderItemNeeds(new Business.Models.Filters.BasicRangeFilter());
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetProductionNeedsSummary()
        {
            WorkOrderItemNeedsModel[] result = new WorkOrderItemNeedsModel[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                result = bObj.GetWorkOrderItemNeedsSummary(new Business.Models.Filters.BasicRangeFilter());
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult CalculateProductionNeeds()
        {
            BusinessResult result = new BusinessResult();

            using (ProductionBO bObj = new ProductionBO())
            {
                result = bObj.CalculateWorkOrderNeeds();
            }

            return Json(result);
        }
    }
}