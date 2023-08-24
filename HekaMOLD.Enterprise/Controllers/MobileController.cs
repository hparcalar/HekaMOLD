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
using HekaMOLD.Business.Models.Virtual;

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
                if (string.IsNullOrEmpty(model.DocumentNo))
                    throw new Exception("Belge numarası boş bırakılamaz.");

                if ((model.FirmId ?? 0) <= 0)
                    throw new Exception("Firma seçmelisiniz.");

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
        public JsonResult SearchBarcodeForApproveSerial(string barcode)
        {
            WorkOrderSerialModel result = new WorkOrderSerialModel();

            using (ProductionBO bObj = new ProductionBO())
            {
                result = bObj.SearchBarcodeForApproveSerial(barcode);
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
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

        [HttpGet]
        public JsonResult SearchBarcodeForPickup(string barcode)
        {
            WorkOrderSerialModel result = new WorkOrderSerialModel();

            using (ProductionBO bObj = new ProductionBO())
            {
                result = bObj.SearchBarcodeForPickup(barcode);
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult SaveProductPickup(ItemReceiptModel receiptModel, WorkOrderSerialModel[] model)
        {
            try
            {
                BusinessResult result = null;

                //receiptModel.PlantId = Convert.ToInt32(Request.Cookies["PlantId"].Value);
                //if (receiptModel.Id == 0)
                //{
                //    receiptModel.CreatedDate = DateTime.Now;
                //    receiptModel.CreatedUserId = Convert.ToInt32(Request.Cookies["UserId"].Value);
                //}

                int wrId = 0;
                using (DefinitionsBO bObj = new DefinitionsBO())
                {
                    wrId = bObj.GetWarehouseList()
                        .Where(d => d.WarehouseType == (int)WarehouseType.ProductWarehouse)
                        .Select(d => d.Id)
                        .FirstOrDefault();
                }

                using (ProductionBO bObj = new ProductionBO())
                {
                    //result = bObj.MakeSerialPickupForProductWarehouse(receiptModel, model);
                    result = bObj.ApproveProducedSerials(model, wrId); //receiptModel.InWarehouseId
                }

                if (result.Result)
                {
                    //using (ProductionBO bObj = new ProductionBO())
                    //{
                    //    bObj.CreateNotification(new NotificationModel
                    //    {
                    //        UserId = null,
                    //        NotifyType = (int)NotifyType.ProductPickupComplete,
                    //        CreatedDate = DateTime.Now,
                    //        IsProcessed = false,
                    //        Message = string.Format("{0:HH:mm}", DateTime.Now) + ": Ürün teslim alma işlemi yapıldı.",
                    //    });
                    //}
                    return Json(new { Status = 1, RecordId = result.RecordId });
                }
                else
                    throw new Exception(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                return Json(new { Status = 0, ErrorMessage = ex.Message });
            }
        }

        [HttpPost]
        [FreeAction]
        public JsonResult UpdateWorkOrderSerial(int serialId, decimal newQuantity)
        {
            BusinessResult result = null;

            using (ProductionBO bObj = new ProductionBO())
            {
                result = bObj.UpdateSerialQuantity(serialId, newQuantity);
            }

            return Json(result);
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

        #region COUNTING SERIALS
        public ActionResult CountingForm()
        {
            return View();
        }
        [HttpPost]
        public JsonResult AddCountingBarcode(string barcode, int warehouseId, int readCount=1, int decreaseCount=0)
        {
            BusinessResult result = null;

            for (int i = 0; i < readCount; i++)
            {
                using (ReceiptBO bObj = new ReceiptBO())
                {
                    result = bObj.AddBarcodeToCounting(
                        barcode, 
                        warehouseId,
                        i == (readCount - 1) ? decreaseCount : 0
                        );
                }
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult DeleteCountingBarcode(int serialId)
        {
            BusinessResult result = null;

            using (ReceiptBO bObj = new ReceiptBO())
            {
                result = bObj.RemoveBarcodeFromCounting(serialId);
            }

            return Json(result);
        }

        [HttpGet]
        public JsonResult GetActiveCountingData(int warehouseId)
        {
            CountingReceiptDetailModel[] details = new CountingReceiptDetailModel[0];
            CountingReceiptSerialModel[] serials = new CountingReceiptSerialModel[0];

            using (ReceiptBO bObj = new ReceiptBO())
            {
                details = bObj.GetActiveCountingDetails(warehouseId);
                serials = bObj.GetActiveCountingSerials(warehouseId);
            }

            var jsonResult = Json(new
            {
                Details = details,
                Serials = serials,
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
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

            using (ProductionBO bObj = new ProductionBO())
            {
                result = bObj.GetSerialsWaitingForDelivery();
                resultSum = bObj.GetSerialsWaitingForDeliverySummary();
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
        public JsonResult GetItemSerialByBarcode(string barcode)
        {
            ItemSerialModel result = new ItemSerialModel();

            using (ReportingBO bObj = new ReportingBO())
            {
                result = bObj.GetSerialByBarcode(barcode);
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
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
            ItemSerialModel[] model, 
            int[] orderDetails = null, DeliveryPlanModel[] deliveryPlans = null, PalletCountInfo[] palletCounts = null)
        {
            try
            {
                BusinessResult result = null;

                int wrId = 0;
                using (DefinitionsBO bObj = new DefinitionsBO())
                {
                    wrId = bObj.GetWarehouseList()
                        .Where(d => d.WarehouseType == (int)WarehouseType.ProductWarehouse)
                        .Select(d => d.Id)
                        .FirstOrDefault();
                }

                receiptModel.PlantId = Convert.ToInt32(Request.Cookies["PlantId"].Value);
                if (receiptModel.Id == 0)
                {
                    receiptModel.CreatedDate = DateTime.Now;
                    receiptModel.InWarehouseId = wrId;
                    receiptModel.CreatedUserId = Convert.ToInt32(Request.Cookies["UserId"].Value);
                }

                using (ProductionBO bObj = new ProductionBO())
                {
                    result = bObj.CreateSerialDelivery(receiptModel, model, orderDetails, deliveryPlans, palletCounts);
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

        #region EQUIPMENT CATEGORY
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

        [HttpPost]
        public JsonResult DeleteEquipmentCategory(int rid)
        {
            BusinessResult result = new BusinessResult();

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                result = bObj.DeleteEquipmentCategory(rid);
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult UpdateEquipmentCategory(EquipmentCategoryModel model)
        {
            BusinessResult result = new BusinessResult();

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                result = bObj.SaveOrUpdateEquipmentCategory(model);
            }

            return Json(result);
        }

        [HttpGet]
        public JsonResult GetEquipmentCategory(int rid)
        {
            EquipmentCategoryModel data = new EquipmentCategoryModel();

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                data = bObj.GetEquipmentCategory(rid);
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region EQUIPMENTS
        [HttpPost]
        public JsonResult UpdateEquipment(EquipmentModel model)
        {
            BusinessResult result = new BusinessResult();

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                result = bObj.SaveOrUpdateEquipment(model);
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult DeleteEquipment(int rid)
        {
            BusinessResult result = new BusinessResult();

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                result = bObj.DeleteEquipment(rid);
            }

            return Json(result);
        }

        [HttpGet]
        public JsonResult GetEquipment(int rid)
        {
            EquipmentModel data = new EquipmentModel();

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                data = bObj.GetEquipment(rid);
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetEquipmentHeader(int machineId, int equipmentCategoryId)
        {
            string machineName = "", equipmentCategoryName = "";

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                var dbMac = bObj.GetMachine(machineId);
                var dbEqCat = bObj.GetEquipmentCategory(equipmentCategoryId);

                if (dbMac != null && dbMac.Id > 0)
                    machineName = dbMac.MachineName;
                if (dbEqCat != null && dbEqCat.Id > 0)
                    equipmentCategoryName = dbEqCat.EquipmentCategoryName;
            }

            var jsonResult = Json(new { 
                MachineName = machineName,
                EquipmentCategoryName = equipmentCategoryName,
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetAllEquipments()
        {
            EquipmentModel[] data = new EquipmentModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                data = bObj.GetEquipmentList();
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

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

        #region ITEM LABELS
        public ActionResult PrintItemLabel()
        {
            return View();
        }
        #endregion

        public ActionResult SettingsMobile()
        {
            return View();
        }
    
        public ActionResult WarehouseManagement()
        {
            return View();
        }

        public ActionResult ProductQualityApprovement()
        {
            return View();
        }
        public ActionResult ProductionInformation()
        {
            return View();
        }
        public ActionResult WarehouseStates()
        {
            return View();
        }
        public ActionResult FinishedProductReport()
        {
            return View();
        }
    }
}