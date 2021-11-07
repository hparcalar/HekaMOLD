﻿using HekaMOLD.Business.Models.DataTransfer.Production;
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
    public class DeliveryController : Controller
    {
        // GET: Delivery
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Planning()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetDateList()
        {
            //MachineModel[] result = new MachineModel[0];

            //using (ProductionBO bObj = new ProductionBO())
            //{
            //    result = bObj.GetMachineList();
            //}

            var jsonResult = Json(null, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetProductionPlans()
        {
            DeliveryPlanModel[] result = new DeliveryPlanModel[0];

            using (DeliveryBO bObj = new DeliveryBO())
            {
                result = bObj.GetDeliveryPlans();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetWaitingPlans()
        {
            WorkOrderDetailModel[] result = new WorkOrderDetailModel[0];

            using (DeliveryBO bObj = new DeliveryBO())
            {
                result = bObj.GetWaitingWorkOrders();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetPlanDetail(int workOrderDetailId)
        {
            WorkOrderDetailModel result = new WorkOrderDetailModel();

            using (PlanningBO bObj = new PlanningBO())
            {
                result = bObj.GetWorkOrderDetail(workOrderDetailId);
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult ReOrderPlan(DeliveryPlanModel model)
        {
            BusinessResult result = new BusinessResult();

            using (DeliveryBO bObj = new DeliveryBO())
            {
                if (!string.IsNullOrEmpty(model.PlanDateStr))
                    model.PlanDate = DateTime.ParseExact(model.PlanDateStr, "dd.MM.yyyy", System.Globalization.CultureInfo
                        .GetCultureInfo("tr"));

                result = bObj.ReOrderPlan(model);
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult DeletePlan(int rid)
        {
            BusinessResult result = new BusinessResult();

            using (DeliveryBO bObj = new DeliveryBO())
            {
                result = bObj.DeletePlan(rid);
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult SaveModel(WorkOrderDetailModel model)
        {
            BusinessResult result = new BusinessResult();

            using (DeliveryBO bObj = new DeliveryBO())
            {
                if (!string.IsNullOrEmpty(model.DeliveryPlanDateStr))
                    model.DeliveryPlanDate = DateTime.ParseExact(model.DeliveryPlanDateStr, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                result = bObj.CreateDeliveryPlan(model);
            }

            return Json(result);
        }
    }
}