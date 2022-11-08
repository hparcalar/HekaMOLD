using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Files;
using HekaMOLD.Business.Models.DataTransfer.Maintenance;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using HekaMOLD.Business.UseCases.Core;
using HekaMOLD.Business.UseCases.Core.Base;
using HekaMOLD.Enterprise.Controllers.Attributes;
using HekaMOLD.Enterprise.Controllers.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    /// <summary>
    /// COMMON DATA - JSON API
    /// </summary>
    [UserAuthFilter]
    public class CommonController : Controller
    {
        #region NOTIFICATIONS
        [FreeAction]
        [HttpGet]
        public JsonResult GetNotifications()
        {
            NotificationModel[] data = new NotificationModel[0];
            using (UsersBO bObj = new UsersBO())
            {
                if (Request.Cookies.AllKeys.Contains("UserId"))
                    data = bObj.GetAwaitingNotifications(Convert.ToInt32(Request.Cookies["UserId"].Value), topRecords: 5);
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetAllNotifications()
        {
            NotificationModel[] data = new NotificationModel[0];
            using (UsersBO bObj = new UsersBO())
            {
                data = bObj.GetAwaitingNotifications(Convert.ToInt32(Request.Cookies["UserId"].Value));
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        [FreeAction]
        public JsonResult GetProfileImage()
        {
            UserModel data = new UserModel();
            using (UsersBO bObj = new UsersBO())
            {
                data = bObj.GetUser(Convert.ToInt32(Request.Cookies["UserId"].Value));
            }

            var jsonResult = Json(data.ProfileImageBase64, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult SetNotifyAsSeen(int notificationId)
        {
            BusinessResult result = new BusinessResult();

            using (UsersBO bObj = new UsersBO())
            {
                result = bObj.SetNotifyAsSeen(Convert.ToInt32(Request.Cookies["UserId"].Value), notificationId);
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult SetNotifyAsPushed(int notificationId)
        {
            BusinessResult result = new BusinessResult();

            using (UsersBO bObj = new UsersBO())
            {
                result = bObj.SetNotifyAsPushed(Convert.ToInt32(Request.Cookies["UserId"].Value), notificationId);
            }

            return Json(result);
        }
        #endregion

        #region RECORD INFORMATION

        [HttpGet]
        public JsonResult GetRecordInformation(int id, string dataType)
        {
            RecordInformationModel data = new RecordInformationModel();
            using (CoreRecordTracingBO bObj = new CoreRecordTracingBO())
            {
                data = bObj.GetRecordInformation(id, dataType);
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region FOREX DATA

        [HttpGet]
        public JsonResult GetForexRate(string forexCode, string forexDate)
        {
            ForexHistoryModel data = new ForexHistoryModel();

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                data = bObj
                    .GetForexValue(forexCode, DateTime.ParseExact(forexDate, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr")));
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ATTACHMENTS
        [FreeAction]
        [HttpGet]
        public JsonResult GetAttachments(int recordId, int recordType)
        {
            AttachmentModel[] data = new AttachmentModel[0];
            using (FilesBO bObj = new FilesBO())
            {
                data = bObj.GetAttachmentList(recordId, (RecordType)recordType);
                foreach (var item in data)
                {
                    item.BinaryContent = null;
                }
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [FreeAction]
        [HttpGet]
        public FileContentResult ShowAttachment(int attachmentId)
        {
            AttachmentModel data = new AttachmentModel();

            using (FilesBO bObj = new FilesBO())
            {
                data = bObj.GetAttachment(attachmentId);
            }

            return File(data.BinaryContent, data.ContentType);
        }

        [HttpPost]
        public JsonResult AddAttachment(int recordId, int recordType, string description,
            HttpPostedFileBase file)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (file == null || file.ContentLength == 0)
                    throw new Exception("Yüklemek için bir dosya seçiniz.");

                var byteReader = new MemoryStream();
                file.InputStream.CopyTo(byteReader);

                AttachmentModel attachmentData = new AttachmentModel
                {
                    BinaryContent = byteReader.ToArray(),
                    ContentType = file.ContentType,
                    Description = description,
                    CreatedDate = DateTime.Now,
                    CreatedUserId = Convert.ToInt32(Request.Cookies["UserId"].Value),
                    FileName = file.FileName
                };

                using (FilesBO bObj = new FilesBO())
                {
                    result = bObj.AddAttachment(recordId, (RecordType)recordType, attachmentData);
                }
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult SaveAttachments(int recordId, int recordType, AttachmentModel[] data)
        {
            BusinessResult result = new BusinessResult();

            using (FilesBO bObj = new FilesBO())
            {
                result = bObj.SaveAttachments(recordId, (RecordType)recordType, data);
            }

            return Json(result);
        }
        #endregion

        #region LIST OF SELECTIONS
        [HttpGet]
        [FreeAction]
        public JsonResult GetMachineList()
        {
            MachineModel[] data = new MachineModel[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                data = bObj.GetMachineList();
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [FreeAction]
        public JsonResult GetPlannableMachineList()
        {
            MachineModel[] data = new MachineModel[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                data = bObj.GetPlannableMachineList();
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [FreeAction]
        public JsonResult GetShiftList()
        {
            ShiftModel[] data = new ShiftModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                data = bObj.GetShiftList();
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [FreeAction]
        public JsonResult GetProductionChiefs()
        {
            UserModel[] data = new UserModel[0];

            using (UsersBO bObj = new UsersBO())
            {
                data = bObj.GetProductionChiefList();
            }

            var jsonResponse = Json(data, JsonRequestBehavior.AllowGet);
            jsonResponse.MaxJsonLength = int.MaxValue;
            return jsonResponse;
        }

        [FreeAction]
        public JsonResult GetWarehouseList()
        {
            WarehouseModel[] data = new WarehouseModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                data = bObj.GetWarehouseList();
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [FreeAction]
        public JsonResult GetEquipmentCategoryList()
        {
            EquipmentCategoryModel[] data = new EquipmentCategoryModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                data = bObj.GetEquipmentCategoryList(true);
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [FreeAction]
        public JsonResult GetFirmList()
        {
            FirmModel[] data = new FirmModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                data = bObj.GetFirmList();
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [FreeAction]
        public JsonResult GetIncidentCategoryList()
        {
            IncidentCategoryModel[] data = new IncidentCategoryModel[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                data = bObj.GetIncidentCategoryList();
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [FreeAction]
        public JsonResult GetPostureCategoryList()
        {
            PostureCategoryModel[] data = new PostureCategoryModel[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                data = bObj.GetPostureCategoryList();
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [FreeAction]
        public JsonResult GetMachineQueue(int machineId)
        {
            MachinePlanModel[] data = new MachinePlanModel[0];

            using (PlanningBO bObj = new PlanningBO())
            {
                data = bObj.GetMachineQueue(machineId);
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region PRODUCTION DATA
        [FreeAction]
        public JsonResult GetActiveWorkOrderOnMachine(int machineId)
        {
            MachinePlanModel data = new MachinePlanModel();

            using (ProductionBO bObj = new ProductionBO())
            {
                data = bObj.GetActiveWorkOrderOnMachine(machineId);
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [FreeAction]
        public JsonResult GetHistoryWorkOrderOnMachine(int workOrderDetailId)
        {
            MachinePlanModel data = new MachinePlanModel();

            using (ProductionBO bObj = new ProductionBO())
            {
                data = bObj.GetHistoryWorkOrderOnMachine(workOrderDetailId);
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [FreeAction]
        public JsonResult GetMoldTestOfProduct(string productCode)
        {
            MoldTestModel data = new MoldTestModel();

            using (MoldBO bObj = new MoldBO())
            {
                data = bObj.FindMoldTestByProduct(productCode);
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [FreeAction]
        public JsonResult GetHistoryWorkOrderListOnMachine(int machineId)
        {
            WorkOrderDetailModel[] data = new WorkOrderDetailModel[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                data = bObj.GetHistoryWorkOrderListOnMachine(machineId);
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region QUALITY
        [FreeAction]
        public JsonResult FindMoldTestByProduct(string productCode)
        {
            MoldTestModel data = new MoldTestModel();

            using (MoldBO bObj = new MoldBO())
            {
                data = bObj.FindMoldTestByProduct(productCode);
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region MACHINE DEVICE SERVICE
        [HttpGet]
        [FreeAction]
        public JsonResult GetMachineStatus(int id)
        {
            MachineStatusType data = MachineStatusType.Stopped;

            using (ProductionBO bObj = new ProductionBO())
            {
                data = bObj.GetMachineStatus(id);
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [FreeAction]
        public JsonResult StartMachineCycle(int id)
        {
            BusinessResult result = null;
            using (ProductionBO bObj = new ProductionBO())
            {
                result = bObj.StartMachineCycle(id);
            }

            return Json(result);
        }

        [HttpPost]
        [FreeAction]
        public JsonResult StopMachineCycle(int id)
        {
            BusinessResult result = null;
            using (ProductionBO bObj = new ProductionBO())
            {
                result = bObj.StopMachineCycle(id);
            }

            return Json(result);
        }
        #endregion

        #region PRINTING
        [HttpGet]
        [FreeAction]
        public JsonResult GetReportTemplateList(string reportTypes)
        {
            ReportTemplateModel[] data = new ReportTemplateModel[0];

            int[] templateTypes = reportTypes.Split(',').Select(d => Convert.ToInt32(d)).ToArray();

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                data = bObj.GetReportTemplateList()
                    .Where(d => templateTypes.Contains(d.ReportType.Value)).ToArray();
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [FreeAction]
        public JsonResult PrintSerial(int id)
        {
            using (ProductionBO bObj = new ProductionBO())
            {
                var printerId =
                        Convert.ToInt32(bObj.GetParameter("DefaultProductPrinter",
                            Convert.ToInt32(Request.Cookies["PlantId"].Value)).PrmValue);
                var dbPrinter = bObj.GetPrinter(printerId);

                bObj.PrintProductLabel(id,printerId, dbPrinter.AccessPath);
            }

            return Json(new { Status = 1 });
        }

        [HttpGet]
        [FreeAction]
        public JsonResult GetPrintQueueList(int printerId)
        {
            PrinterQueueModel[] data = new PrinterQueueModel[0];
            using (CoreSystemBO bObj = new CoreSystemBO())
            {
                data = bObj.GetPrinterQueue(printerId);
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [FreeAction]
        public JsonResult SetQueueAsPrinted(int id)
        {
            using (ProductionBO bObj = new ProductionBO())
            {
                bObj.SetElementAsPrinted(id);
            }

            return Json(new { Status = 1 });
        }

        [HttpGet]
        [FreeAction]
        public JsonResult GetCurrentShift()
        {
            ShiftModel data = new ShiftModel();
            using (ProductionBO bObj = new ProductionBO())
            {
                data = bObj.GetCurrentShift();
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        [FreeAction]
        public JsonResult GetWorkOrderSerial(int id)
        {
            WorkOrderSerialModel data = new WorkOrderSerialModel();
            using (ProductionBO bObj = new ProductionBO())
            {
                data = bObj.GetWorkOrderSerial(id);
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        [FreeAction]
        public JsonResult GetItemSerial(int id)
        {
            WorkOrderSerialModel data = new WorkOrderSerialModel();
            using (ReceiptBO bObj = new ReceiptBO())
            {
                var itemSerial = bObj.GetItemSerial(id);
                if (itemSerial != null)
                {
                    data.CreatedDate = itemSerial.CreatedDate;
                    data.CreatedDateStr = itemSerial.CreatedDateStr;
                    data.CreatedUserId = itemSerial.CreatedUserId;
                    data.FirmCode = itemSerial.FirmCode;
                    data.FirmName = itemSerial.FirmName;
                    data.FirstQuantity = itemSerial.FirstQuantity;
                    data.Id = itemSerial.Id;
                    data.InPackageQuantity = itemSerial.InPackageQuantity;
                    data.ItemId = itemSerial.ItemId;
                    data.ItemNo = itemSerial.ItemNo;
                    data.ItemName = itemSerial.ItemName;
                    data.ItemReceiptDetailId = itemSerial.ItemReceiptDetailId;
                    data.LiveQuantity = itemSerial.LiveQuantity;
                    data.SerialNo = itemSerial.SerialNo;
                    data.ShiftCode = "";
                    data.ShiftName = "";
                }
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        [FreeAction]
        public JsonResult GetItem(int id)
        {
            ItemModel data = new ItemModel();
            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                data = bObj.GetItem(id);
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        [FreeAction]
        public JsonResult GetWorkOrderDetail(int id)
        {
            WorkOrderDetailModel data = new WorkOrderDetailModel();
            using (ProductionBO bObj = new ProductionBO())
            {
                data = bObj.GetWorkOrderDetail(id);
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [FreeAction]
        public JsonResult PrintMoldLabel(int id)
        {
            string moldCode = "";
            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                var dbMold = bObj.GetMold(id);
                moldCode = dbMold.MoldName;
            }

            using (ProductionBO bObj = new ProductionBO())
            {
                bObj.PrintMoldLabel(id, Server.MapPath("~/Outputs/"), moldCode + ".pdf");
            }

            return Json(new { Status = 1, FileName= moldCode + ".pdf" });
        }

        [HttpGet]
        [FreeAction]
        public JsonResult GetDefaultSerialPrinter()
        {
            int printerId = 0;

            try
            {
                using (ProductionBO bObj = new ProductionBO())
                {
                    printerId =
                        Convert.ToInt32(bObj.GetParameter("DefaultProductPrinter", 
                            Convert.ToInt32(Request.Cookies["PlantId"].Value)).PrmValue);
                }
            }
            catch (Exception)
            {

            }

            return Json(printerId, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        [FreeAction]
        public JsonResult GetPrinterList()
        {
            SystemPrinterModel[] data = new SystemPrinterModel[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                data = bObj.GetPrinterList();
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion
    }
}