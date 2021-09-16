using HekaMOLD.Business.Models.DataTransfer.Order;
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
    public class PlanningController : Controller
    {
        // GET: Planning
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetMachineList()
        {
            MachineModel[] result = new MachineModel[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                result = bObj.GetMachineList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetProductionPlans()
        {
            MachinePlanModel[] result = new MachinePlanModel[0];

            using (PlanningBO bObj = new PlanningBO())
            {
                result = bObj.GetProductionPlans();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetWaitingPlans()
        {
            ItemOrderDetailModel[] result = new ItemOrderDetailModel[0];

            using (PlanningBO bObj = new PlanningBO())
            {
                result = bObj.GetWaitingSaleOrders();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult ReOrderPlan(MachinePlanModel model)
        {
            BusinessResult result = new BusinessResult();

            using (PlanningBO bObj = new PlanningBO())
            {
                result = bObj.ReOrderPlan(model);
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult DeletePlan(int rid)
        {
            BusinessResult result = new BusinessResult();

            using (PlanningBO bObj = new PlanningBO())
            {
                result = bObj.DeletePlan(rid);
            }

            return Json(result);
        }
        
        [HttpPost]
        public JsonResult SaveModel(ItemOrderDetailModel model)
        {
            BusinessResult result = new BusinessResult();

            using (PlanningBO bObj = new PlanningBO())
            {
                result = bObj.CreateMachinePlan(model);
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult CopyPlan(int fromPlanId, int quantity,
            int firmId, int targetMachineId)
        {
            BusinessResult result = new BusinessResult();

            using (PlanningBO bObj = new PlanningBO())
            {
                result = bObj.CopyFromWorkOrder(fromPlanId,
                    quantity, firmId, targetMachineId);
            }

            return Json(result);
        }
    }
}