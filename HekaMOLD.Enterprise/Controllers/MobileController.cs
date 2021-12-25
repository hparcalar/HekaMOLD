using HekaMOLD.Enterprise.Controllers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HekaMOLD.Enterprise.Helpers;
using HekaMOLD.Business.Models.DataTransfer.Receipt;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.Constants;
using System.Web.Configuration;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.DataTransfer.Maintenance;
using HekaMOLD.Business.Models.DataTransfer.Summary;
using HekaMOLD.Enterprise.Controllers.Attributes;
using HekaMOLD.Business.Models.DataTransfer.Warehouse;

namespace HekaMOLD.Enterprise.Controllers
{
    [MobileAuthFilter]
    public class MobileController : Controller
    {
        // GET: Mobile
        public ActionResult Index()
        {
            if (Request.Cookies.AllKeys.Contains("ShowAs"))
            {
                var showAs = Request.Cookies["ShowAs"].Value;
                if (showAs == "MobileProductionUser")
                    return RedirectToAction("Production");
                else if (showAs == "MobileMechanicUser")
                    return RedirectToAction("Mechanic");
                else if (showAs == "MobileWarehouseUser")
                    return RedirectToAction("Warehouse");
            }
            else
            {
                if (this.IsGranted("MobileProductionUser"))
                    return RedirectToAction("Production");
                else if (this.IsGranted("MobileMechanicUser"))
                    return RedirectToAction("Mechanic");
                else if (this.IsGranted("MobileWarehouseUser"))
                    return RedirectToAction("Warehouse");
            }

            return View();
        }

        [HttpGet]
        public JsonResult GetSelectables(string action)
        {
            WarehouseModel[] warehouses = new WarehouseModel[0];
            FirmModel[] firms = new FirmModel[0];
            ShiftModel[] shifts = new ShiftModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                warehouses = bObj.GetWarehouseList();
                firms = bObj.GetFirmList();
                shifts = bObj.GetShiftList();
            }

            var jsonResult = Json(new
            {
                Warehouses = warehouses,
                Firms = firms,
                Shifts = shifts,
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        #region HOME SCREENS
        public ActionResult Warehouse()
        {
            return View();
        }

        public ActionResult Production()
        {
            return View();
        }

        public ActionResult Mechanic()
        {
            return View();
        }
        #endregion

        #region ITEM ENTRY
        public ActionResult ItemEntry()
        {
            return View();
        }

        public ActionResult ItemEntryList()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetItemEntryData(int rid)
        {
            ItemReceiptModel model = new ItemReceiptModel();

            using (ReceiptBO bObj = new ReceiptBO())
            {
                model = bObj.GetItemReceipt(rid);
            }

            var jsonResult = Json(model, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetItemEntryList()
        {
            ItemReceiptModel[] result = new ItemReceiptModel[0];
            int receiptType = (int)ItemReceiptType.ItemBuying;
            int receiptCategory = (int)ReceiptCategoryType.Purchasing;

            using (ReceiptBO bObj = new ReceiptBO())
            {
                int? rType = receiptType == 0 ? (int?)null : receiptType;

                result = bObj.GetItemReceiptList(
                        (ReceiptCategoryType)receiptCategory,
                        (ItemReceiptType?)rType
                    )
                    .OrderByDescending(d => d.ReceiptDate)
                    .ToArray();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult SaveItemEntry(ItemReceiptModel model)
        {
            try
            {
                BusinessResult result = null;
                using (ReceiptBO bObj = new ReceiptBO())
                {
                    model.PlantId = Convert.ToInt32(Request.Cookies["PlantId"].Value);
                    if (model.Id == 0)
                    {
                        model.CreatedDate = DateTime.Now;
                        model.CreatedUserId = Convert.ToInt32(Request.Cookies["UserId"].Value);
                        model.ReceiptType = (int)ItemReceiptType.ItemBuying;
                        model.ReceiptStatus = (int)ReceiptStatusType.Created;

                        foreach (var item in model.Details)
                        {
                            item.TaxIncluded = false;
                            item.TaxRate = 0;
                        }
                    }

                    result = bObj.SaveOrUpdateItemReceipt(model);
                }

                if (result.Result)
                    return Json(new { Status = 1, RecordId = result.RecordId });
                else
                    throw new Exception(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                return Json(new { Status = 0, ErrorMessage = ex.Message });
            }
        }
        #endregion

        #region PRODUCTION
        public ActionResult ManageMachines()
        {
            return View();
        }

        public ActionResult ManageShifts()
        {
            return View();
        }

        public ActionResult ProductPlanList()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SaveShift(ShiftModel model)
        {
            BusinessResult result = new BusinessResult();

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                if (!string.IsNullOrEmpty(model.StartTimeStr))
                {
                    var startData = model.StartTimeStr.Split(':');
                    model.StartTime = TimeSpan.FromMinutes(Convert.ToInt32(startData[0]) * 60 + Convert.ToInt32(startData[1]));
                }

                if (!string.IsNullOrEmpty(model.EndTimeStr))
                {
                    var endData = model.EndTimeStr.Split(':');
                    model.EndTime = TimeSpan.FromMinutes(Convert.ToInt32(endData[0]) * 60 + Convert.ToInt32(endData[1]));
                }

                result = bObj.SaveOrUpdateShift(model);
            }

            return Json(result);
        }

        [HttpPost]
        [FreeAction]
        public JsonResult ToggleMachineStatus(int machineId)
        {
            BusinessResult result = new BusinessResult();

            using (ProductionBO bObj = new ProductionBO())
            {
                result = bObj.ToggleMachineStatus(machineId);
            }

            return Json(result);
        }
        public ActionResult ProductionStatus()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ToggleWorkOrderStatus(int workOrderDetailId)
        {
            BusinessResult result = new BusinessResult();

            using (PlanningBO bObj = new PlanningBO())
            {
                int userId = Convert.ToInt32(Request.Cookies["UserId"].Value);
                result = bObj.ToggleWorkOrderStatus(workOrderDetailId, userId);
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult HoldWorkOrder(int workOrderDetailId)
        {
            BusinessResult result = new BusinessResult();

            using (PlanningBO bObj = new PlanningBO())
            {
                int userId = Convert.ToInt32(Request.Cookies["UserId"].Value);
                result = bObj.HoldWorkOrder(workOrderDetailId);
            }

            return Json(result);
        }

        public ActionResult ProductEntry()
        {
            return View();
        }

        public ActionResult ProductInformation()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetMachineWorkList(int machineId)
        {
            WorkOrderDetailModel[] data = new WorkOrderDetailModel[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                data = bObj.GetActiveWorkOrderListOnMachine(machineId);
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult SaveProductEntry(int workOrderDetailId, int inPackageQuantity, string barcode, bool printLabel, int printerId)
        {
            BusinessResult result = new BusinessResult();

            int userId = Convert.ToInt32(Request.Cookies["UserId"].Value);
            string serialTypeConfig = WebConfigurationManager.AppSettings["WorkOrderSerialType"];

            WorkOrderSerialType serialType = WorkOrderSerialType.SingleProduct;
            if (serialTypeConfig == "ProductPackage")
                serialType = WorkOrderSerialType.ProductPackage;

            using (ProductionBO bObj = new ProductionBO())
            {
                result = bObj.AddProductEntry(workOrderDetailId, userId, serialType, inPackageQuantity, barcode);
            }

            // UPDATE USER STATS
            using (ProductionBO bObj = new ProductionBO())
            {
                int? machineId = bObj.GetMachineByWorkOrderDetail(workOrderDetailId);
                if (machineId != null)
                    bObj.UpdateUserHistory(machineId ?? 0, 0);
            }

            if (printLabel == true)
            {
                using (ProductionBO bObj = new ProductionBO())
                {
                    var pqResult = bObj.AddToPrintQueue(new PrinterQueueModel
                    {
                        PrinterId = printerId,
                        RecordType = (int)RecordType.SerialItem,
                        RecordId = result.RecordId,
                        CreatedDate = DateTime.Now,
                    });
                }
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult DeleteProductEntry(int id)
        {
            BusinessResult result = new BusinessResult();

            using (ProductionBO bObj = new ProductionBO())
            {
                result = bObj.DeleteProductEntry(id);
            }

            return Json(result);
        }

        public ActionResult PostureEntry()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetPostureList(int machineId)
        {
            ProductionPostureModel[] dataList = new ProductionPostureModel[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                dataList = bObj.GetPostureList(new Business.Models.Filters.BasicRangeFilter
                {
                    StartDate = "01.01.1998",
                    EndDate = "01.01.2099",
                    MachineId = machineId,
                }).OrderByDescending(d => d.Id).ToArray();
            }

            var jsonResult = Json(dataList, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult SavePosture(ProductionPostureModel model)
        {
            BusinessResult result = new BusinessResult();

            int userId = Convert.ToInt32(Request.Cookies["UserId"].Value);

            using (ProductionBO bObj = new ProductionBO())
            {
                if (model.Id == 0)
                {
                    model.CreatedUserId = userId;
                }

                result = bObj.SaveOrUpdatePosture(model);
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult FinishPosture(EndPostureParam model)
        {
            BusinessResult result = new BusinessResult();

            int userId = Convert.ToInt32(Request.Cookies["UserId"].Value);

            using (ProductionBO bObj = new ProductionBO())
            {
                model.UserId = userId;

                result = bObj.FinishPosture(model);
            }

            return Json(result);
        }

        public ActionResult OngoingPostures()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetOngoingPostureList()
        {
            ProductionPostureModel[] dataList = new ProductionPostureModel[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                dataList = bObj.GetOngoingPostures().OrderByDescending(d => d.Id).ToArray();
            }

            var jsonResult = Json(dataList, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult ProductEntryList()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetIncidentList(int machineId)
        {
            IncidentModel[] dataList = new IncidentModel[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                dataList = bObj.GetIncidentList(new Business.Models.Filters.BasicRangeFilter
                {
                    StartDate = "01.01.1998",
                    EndDate = "01.01.2099",
                    MachineId = machineId,
                }).OrderByDescending(d => d.Id).ToArray();
            }

            var jsonResult = Json(dataList, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult SaveIncident(IncidentModel model)
        {
            BusinessResult result = new BusinessResult();

            int userId = Convert.ToInt32(Request.Cookies["UserId"].Value);

            using (ProductionBO bObj = new ProductionBO())
            {
                if (model.Id == 0)
                    model.CreatedUserId = userId;

                result = bObj.SaveOrUpdateIncident(model);
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult StartIncident(IncidentStatusParam model)
        {
            BusinessResult result = new BusinessResult();

            int userId = Convert.ToInt32(Request.Cookies["UserId"].Value);

            using (ProductionBO bObj = new ProductionBO())
            {
                model.UserId = userId;

                result = bObj.StartIncident(model);
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult FinishIncident(IncidentStatusParam model)
        {
            BusinessResult result = new BusinessResult();

            int userId = Convert.ToInt32(Request.Cookies["UserId"].Value);

            using (ProductionBO bObj = new ProductionBO())
            {
                model.UserId = userId;

                result = bObj.FinishIncident(model);
            }

            return Json(result);
        }

        public ActionResult FaultEntry()
        {
            return View();
        }

        public ActionResult OngoingFaults()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetOngoingFaultList()
        {
            IncidentModel[] dataList = new IncidentModel[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                dataList = bObj.GetOngoingIncidents().OrderByDescending(d => d.Id).ToArray();
            }

            var jsonResult = Json(dataList, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region PRODUCT PICK UP TO CONFIRMATION
        public ActionResult ProductPickup()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetApprovedSerials()
        {
            WorkOrderSerialModel[] result = new WorkOrderSerialModel[0];
            WorkOrderSerialSummary[] resultSum = new WorkOrderSerialSummary[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                result = bObj.GetApprovedSerials();
                resultSum = bObj.GetApprovedSerialsSummary();
            }

            var jsonResult = Json(new
            {
                Serials = result,
                Summaries = resultSum,
            }, JsonRequestBehavior.AllowGet);

            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetProductsForPickup()
        {
            WorkOrderSerialModel[] result = new WorkOrderSerialModel[0];
            WorkOrderSerialSummary[] resultSum = new WorkOrderSerialSummary[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                result = bObj.GetSerialsWaitingForPickup();
                resultSum = bObj.GetSerialsWaitingForPickupSummary();
            }

            var jsonResult = Json(new { 
                Serials = result,
                Summaries = resultSum,
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult SaveProductPickup(ItemReceiptModel receiptModel, WorkOrderSerialModel[] model)
        {
            try
            {
                BusinessResult result = null;

                receiptModel.PlantId = Convert.ToInt32(Request.Cookies["PlantId"].Value);
                if (receiptModel.Id == 0)
                {
                    receiptModel.CreatedDate = DateTime.Now;
                    receiptModel.CreatedUserId = Convert.ToInt32(Request.Cookies["UserId"].Value);
                }

                int wrId = 0;
                using (DefinitionsBO bObj = new DefinitionsBO())
                {
                    wrId = bObj.GetWarehouseList()
                        .Where(d => d.WarehouseType == (int)WarehouseType.ProductWarehouse)
                        .Select(d => d.Id)
                        .FirstOrDefault();
                }

                receiptModel.InWarehouseId = wrId;

                using (ProductionBO bObj = new ProductionBO())
                {
                    result = bObj.MakeSerialPickupForProductWarehouse(receiptModel, model);
                    //result = bObj.ApproveProducedSerials(model, wrId); //receiptModel.InWarehouseId
                }

                if (result.Result)
                    return Json(new { Status = 1, RecordId = result.RecordId });
                else
                    throw new Exception(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                return Json(new { Status = 0, ErrorMessage = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteSerials(WorkOrderSerialModel[] model)
        {
            BusinessResult result = null;

            try
            {
                int userId = Convert.ToInt32(Request.Cookies["UserId"].Value);

                using (ProductionBO bObj = new ProductionBO())
                {
                    result = bObj.DeleteSerials(model, userId);
                }
            }
            catch (Exception)
            {

            }

            return Json(new { Status = result.Result ? 1 : 0, ErrorMessage=result.ErrorMessage });
        }

        #endregion

        #region PRODUCT WASTAGE
        public ActionResult WastageEntry()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SaveWastageEntry(ProductWastageModel model)
        {
            BusinessResult result = new BusinessResult();

            int userId = Convert.ToInt32(Request.Cookies["UserId"].Value);
 
            using (ProductionBO bObj = new ProductionBO())
            {
                if (model.Id <= 0)
                    model.CreatedUserId = userId;

                result = bObj.SaveOrUpdateWastage(model);
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult DeleteWastageEntry(int id)
        {
            BusinessResult result = new BusinessResult();

            using (ProductionBO bObj = new ProductionBO())
            {
                result = bObj.DeleteWastage(id);
            }

            return Json(result);
        }
        #endregion

        #region PRODUCT DELIVERY
        public ActionResult ProductDelivery()
        {
            return View();
        }

        public ActionResult ProductDeliveryHistory()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetProductsForDelivery()
        {
            ItemSerialModel[] result = new ItemSerialModel[0];
            ItemSerialSummary[] resultSum = new ItemSerialSummary[0];
            PalletModel[] pallets = new PalletModel[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                result = bObj.GetSerialsWaitingForDelivery();
                resultSum = bObj.GetSerialsWaitingForDeliverySummary();
            }

            result = result.Where(d => d.PalletId != null).ToArray();

            pallets = result.GroupBy(d => new { Id= d.PalletId, d.PalletNo })
                .Select(d => new PalletModel
                {
                    Id = d.Key.Id ?? 0,
                    PalletNo = d.Key.PalletNo,
                    BoxCount = d.Count(),
                    Quantity = d.Sum(m => m.FirstQuantity) ?? 0,
                }).ToArray();

            var jsonResult = Json(new
            {
                Serials = result,
                Summaries = resultSum,
                Pallets = pallets,
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetBoxesWithoutPallet()
        {
            ItemSerialModel[] boxes = new ItemSerialModel[0];
            PalletModel[] pallets = new PalletModel[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                boxes = bObj.GetBoxesWithoutPallet();
                pallets = bObj.GetWaitingPallets();
            }

            var jsonResult = Json(new
            {
                Boxes = boxes,
                Pallets = pallets,
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetBoxesOfPallet(int palletId)
        {
            ItemSerialModel[] boxes = new ItemSerialModel[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                boxes = bObj.GetBoxesInPallet(palletId);
            }

            var jsonResult = Json(new
            {
                Boxes = boxes,
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetProductDeliveryHistory()
        {
            ItemReceiptModel[] result = new ItemReceiptModel[0];

            using (ReceiptBO bObj = new ReceiptBO())
            {
                result = bObj.GetItemReceiptList(ReceiptCategoryType.Sales, ItemReceiptType.ItemSelling);
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult SaveProductDelivery(ItemReceiptModel receiptModel, 
            PalletModel[] model, 
            int[] orderDetails = null)
        {
            try
            {
                BusinessResult result = null;

                receiptModel.PlantId = Convert.ToInt32(Request.Cookies["PlantId"].Value);
                if (receiptModel.Id == 0)
                {
                    receiptModel.CreatedDate = DateTime.Now;
                    receiptModel.CreatedUserId = Convert.ToInt32(Request.Cookies["UserId"].Value);
                }

                using (ProductionBO bObj = new ProductionBO())
                {
                    List<ItemSerialModel> serials = new List<ItemSerialModel>();

                    foreach (var item in model)
                    {
                        var boxes = bObj.GetBoxesInPallet(item.Id);
                        if (boxes != null && boxes.Length > 0)
                            serials.AddRange(boxes);
                    }

                    result = bObj.CreateSerialDelivery(receiptModel, serials.ToArray(), orderDetails);
                }

                if (result.Result)
                {
                    foreach (var item in model)
                    {
                        using (ProductionBO bObj = new ProductionBO())
                        {
                            bObj.DeletePallet(item.Id);
                        }
                    }
                }

                if (result.Result)
                    return Json(new { Status = 1, RecordId = result.RecordId });
                else
                    throw new Exception(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                return Json(new { Status = 0, ErrorMessage = ex.Message });
            }
        }

        [HttpGet]
        [FreeAction]
        public JsonResult GetDeliveryPlanList(int cursor)
        {
            DeliveryPlanModel[] result = new DeliveryPlanModel[0];
            DateTime planDate = DateTime.Now.Date.AddDays(cursor);

            using (DeliveryBO bObj = new DeliveryBO())
            {
                result = bObj.GetOpenDeliveryPlans(planDate);
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult ManagePallets()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CreatePallet()
        {
            try
            {
                BusinessResult result = null;

                using (ProductionBO bObj = new ProductionBO())
                {
                    result = bObj.CreatePallet();
                }

                if (result.Result)
                    return Json(new { Status = 1, RecordId = result.RecordId });
                else
                    throw new Exception(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                return Json(new { Status = 0, ErrorMessage = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeletePallet(int palletId)
        {
            try
            {
                BusinessResult result = null;

                using (ProductionBO bObj = new ProductionBO())
                {
                    result = bObj.DeletePallet(palletId);
                }

                if (result.Result)
                    return Json(new { Status = 1, RecordId = result.RecordId });
                else
                    throw new Exception(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                return Json(new { Status = 0, ErrorMessage = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult AddToPallet(int itemSerialId, int palletId)
        {
            try
            {
                BusinessResult result = null;

                using (ProductionBO bObj = new ProductionBO())
                {
                    result = bObj.AddToPallet(itemSerialId, palletId);
                }

                if (result.Result)
                    return Json(new { Status = 1, RecordId = result.RecordId });
                else
                    throw new Exception(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                return Json(new { Status = 0, ErrorMessage = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult RemoveFromPallet(int itemSerialId)
        {
            try
            {
                BusinessResult result = null;

                using (ProductionBO bObj = new ProductionBO())
                {
                    result = bObj.RemoveFromPallet(itemSerialId);
                }

                if (result.Result)
                    return Json(new { Status = 1, RecordId = result.RecordId });
                else
                    throw new Exception(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                return Json(new { Status = 0, ErrorMessage = ex.Message });
            }
        }
        #endregion

        #region MAINTENANCE
        public ActionResult MachineEquipments()
        {
            return View();
        }

        public ActionResult MaintenanceSchedule()
        {
            return View();
        }

        public ActionResult PostureHistory()
        {
            return View();
        }

        public ActionResult IncidentHistory()
        {
            return View();
        }

        [HttpPost]
        [FreeAction]
        public JsonResult SaveEquipmentCategory(string name)
        {
            BusinessResult result = new BusinessResult();

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                var existingCat = bObj.GetEquipmentCategory(0);
                if (existingCat == null || existingCat.Id <= 0)
                {
                    result = bObj.SaveOrUpdateEquipmentCategory(new EquipmentCategoryModel
                    {
                        EquipmentCategoryCode = name,
                        EquipmentCategoryName = name,
                        IsCritical = true,
                    });
                }
            }

            return Json(new { Status=result.Result ? 1 : 0, RecordId=result.RecordId, ErrorMessage=result.ErrorMessage });
        }
        #endregion

        #region ITEM DELIVERY TO PRODUCTION
        public ActionResult ItemDelivery()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SaveItemDelivery(int itemReceiptDetailId, decimal quantity)
        {
            try
            {
                BusinessResult result = null;

                int defaultMachineId = 0;

                using (DefinitionsBO bObj = new DefinitionsBO())
                {
                    defaultMachineId = bObj.GetMachineList()[0].Id;
                }

                using (PlanningBO bObj = new PlanningBO())
                {
                    result = bObj.CreateItemDeliveryToProduction(itemReceiptDetailId, defaultMachineId, quantity);
                }

                if (result.Result)
                    return Json(new { Status = 1, RecordId = result.RecordId });
                else
                    throw new Exception(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                return Json(new { Status = 0, ErrorMessage = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetItemsForDelivery()
        {
            ItemReceiptDetailModel[] result = new ItemReceiptDetailModel[0];

            using (ReceiptBO bObj = new ReceiptBO())
            {
                result = bObj.GetOpenItemEntries();
            }

            var jsonResult = Json(new
            {
                Serials = result,
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        public ActionResult SettingsMobile()
        {
            return View();
        }
    }
}