using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Order;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.Models.Virtual;
using HekaMOLD.Business.UseCases;
using HekaMOLD.Enterprise.Controllers.Attributes;
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

        public ActionResult PrintTemplate()
        {
            return View();
        }

        [HttpGet]
        [FreeAction]
        public JsonResult GetMachineList()
        {
            MachineModel[] result = new MachineModel[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                result = bObj.GetPlannableMachineList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        [FreeAction]
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
        [FreeAction]
        public JsonResult GetSerialsOfPlan(int? planId)
        {
            WorkOrderDetailModel result = new WorkOrderDetailModel();

            using (PlanningBO bObj = new PlanningBO())
            {
                result = bObj.GetWorkOrderDetailWithSerials(planId ?? 0);
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        [FreeAction]
        public JsonResult GetProductionPlanViews(string date)
        {
            DateTime dtDate = DateTime.ParseExact(date, "dd.MM.yyyy", 
                System.Globalization.CultureInfo.GetCultureInfo("tr"));

            MachinePlanModel[] result = new MachinePlanModel[0];

            using (PlanningBO bObj = new PlanningBO())
            {
                result = bObj.GetProductionPlanViews(dtDate);
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
        [FreeAction]
        public JsonResult PrintItemLabel(int itemId, decimal? quantity, int labelCount)
        {
            BusinessResult result = new BusinessResult();

            int plantId = Convert.ToInt32(Request.Cookies["PlantId"].Value);
            int userId = Convert.ToInt32(Request.Cookies["UserId"].Value);
            int workOrderDetailId = 0;
            int printerId = 0;

            using (ProductionBO bObj = new ProductionBO())
            {
                printerId = Convert.ToInt32(bObj.GetParameter("DefaultProductPrinter",
                        Convert.ToInt32(Request.Cookies["PlantId"].Value)).PrmValue);

                var bRes = bObj.SaveOrUpdateWorkOrder(new WorkOrderModel
                {
                    WorkOrderType = (int)WorkOrderType.Casual,
                    PlantId = plantId,
                    CreatedDate = DateTime.Now,
                    DocumentNo = bObj.GetNextWorkOrderNo(),
                    FirmId = null,
                    WorkOrderDate = DateTime.Now,
                    WorkOrderStatus = (int)WorkOrderStatusType.Completed,
                    Details = new WorkOrderDetailModel[]
                    {
                        new WorkOrderDetailModel
                        {
                            CompleteQuantity = 0,
                            CreatedDate = DateTime.Now,
                            ItemId = itemId,
                            Quantity = quantity * labelCount,
                            QualityStatus = (int)QualityStatusType.Waiting,
                            InPackageQuantity = Convert.ToInt32(quantity ?? 0),
                            NewDetail = true,
                            WorkOrderStatus = (int)WorkOrderStatusType.Completed,
                        }
                    }
                });

                if (!bRes.Result)
                    result.Result = false;
                else
                {
                    workOrderDetailId = bRes.DetailRecordId;
                }
            }

            if (workOrderDetailId > 0)
            {
                int wrId = 0;
                using (DefinitionsBO bObj = new DefinitionsBO())
                {
                    wrId = bObj.GetWarehouseList()
                        .Where(d => d.WarehouseType == (int)WarehouseType.ProductWarehouse)
                        .Select(d => d.Id)
                        .FirstOrDefault();
                }

                for (int i = 0; i < labelCount; i++)
                {
                    int serialId = 0;
                    using (ProductionBO bObj = new ProductionBO())
                    {
                        var sRes = bObj.AddProductEntry(workOrderDetailId, userId, WorkOrderSerialType.ProductPackage,
                            Convert.ToInt32(quantity ?? 0), "", true, wrId);
                        if (sRes.Result)
                            serialId = sRes.RecordId;
                    }

                    using (ProductionBO bObj = new ProductionBO())
                    {
                        result = bObj.AddToPrintQueue(new PrinterQueueModel
                        {
                            PrinterId = printerId,
                            RecordType = (int)RecordType.SerialItem,
                            RecordId = serialId,
                            CreatedDate = DateTime.Now,
                        });
                    }

                    //using (PlanningBO bObj = new PlanningBO())
                    //{
                    //    int printerId = Convert.ToInt32(bObj.GetParameter("DefaultProductPrinter",
                    //        Convert.ToInt32(Request.Cookies["PlantId"].Value)).PrmValue);

                    //    var model = new PrinterQueueModel();
                    //    model.AllocatedPrintData = Newtonsoft.Json.JsonConvert.SerializeObject(new
                    //    {
                    //        ItemId = itemId,
                    //        Code = "",
                    //        Quantity = quantity,
                    //    });
                    //    model.PrinterId = printerId;
                    //    model.RecordType = 8;
                    //    model.RecordId = itemId;
                    //    result = bObj.AddToPrintQueue(model);
                    //}
                }
            }

            using (ProductionBO bObj = new ProductionBO())
            {
                bObj.CreateNotification(new NotificationModel
                {
                    UserId = null,
                    NotifyType = (int)NotifyType.ManuelProductLabelPrinted,
                    CreatedDate = DateTime.Now,
                    IsProcessed = false,
                    Title = "Manuel ürün etiketi yazdırıldı",
                    Message = string.Format("{0:HH:mm}", DateTime.Now) + ": Ürün etiketi yazdırma işlemi yapıldı.",
                });
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult AllocateAndPrintLabel(PrinterQueueModel model, int labelCount, int workOrderDetailId)
        {
            BusinessResult result = new BusinessResult();

            for (int i = 0; i < labelCount; i++)
            {
                string allocatedCode = "";
                using (PlanningBO bObj = new PlanningBO())
                {
                    var allocResult = bObj.AllocateCode((int)RecordType.SerialItem);
                    if (allocResult.Result)
                        allocatedCode = allocResult.Code;
                }

                if (!string.IsNullOrEmpty(allocatedCode))
                {
                    using (PlanningBO bObj = new PlanningBO())
                    {
                        model.AllocatedPrintData = Newtonsoft.Json.JsonConvert.SerializeObject(new
                        {
                            WorkOrderDetailId = workOrderDetailId,
                            Code = allocatedCode,
                        });
                        result = bObj.AddToPrintQueue(model);
                    }
                }
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
        public JsonResult UpdateLastPlanView()
        {
            BusinessResult result = new BusinessResult();

            int plantId = Convert.ToInt32(Request.Cookies["PlantId"].Value);

            using (PlanningBO bObj = new PlanningBO())
            {
                result = bObj.SavePlanView(plantId);
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult EditPlan(WorkOrderDetailModel model)
        {
            BusinessResult result = new BusinessResult();

            using (PlanningBO bObj = new PlanningBO())
            {
                result = bObj.EditWorkOrder(model);
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult CreateTrialPlan(TrialPlanModel model)
        {
            BusinessResult result = new BusinessResult();

            using (ProductionBO bObj = new ProductionBO())
            {
                result = bObj.SaveOrUpdateWorkOrder(new WorkOrderModel
                {
                    CreatedDate = DateTime.Now,
                    FirmId = null,
                    PlantId = Convert.ToInt32(Request.Cookies["PlantId"].Value),
                    TrialFirmName = model.TrialFirmExplanation,
                    WorkOrderDate = DateTime.Now,
                    WorkOrderNo = bObj.GetNextWorkOrderNo(),
                    WorkOrderStatus = (int)WorkOrderStatusType.Planned,
                    WorkOrderType = (int)WorkOrderType.TrialProduction,
                    Details = new WorkOrderDetailModel[] { 
                        new WorkOrderDetailModel
                        {
                            TrialProductName = model.TrialProductExplanation,
                            Quantity = model.Quantity,
                            WorkOrderType = (int)WorkOrderType.TrialProduction,
                        }
                    }
                });
            }

            if (result.Result)
            {
                using (PlanningBO bObj = new PlanningBO())
                {
                    result = bObj.CreateMachinePlan(result.DetailRecordId, model.MachineId);
                }
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult CompletePlan(int id)
        {
            BusinessResult result = new BusinessResult();

            using (PlanningBO bObj = new PlanningBO())
            {
                result = bObj.CompleteWorkOrder(id);
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