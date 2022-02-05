using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Order;
using HekaMOLD.Business.Models.DataTransfer.Receipt;
using HekaMOLD.Business.Models.DataTransfer.Reporting;
using HekaMOLD.Business.Models.Dictionaries;
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
    public class ItemReceiptController : Controller
    {
        // GET: PIRequest
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetNextReceiptNo(int receiptType)
        {
            string receiptNo = "";

            using (ReceiptBO bObj = new ReceiptBO())
            {
                receiptNo = bObj.GetNextReceiptNo(Convert.ToInt32(Request.Cookies["PlantId"].Value), (ItemReceiptType)receiptType);
            }

            var jsonResult = Json(new { Result=!string.IsNullOrEmpty(receiptNo), ReceiptNo=receiptNo }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetSelectables(int receiptCategory)
        {
            ItemModel[] items = new ItemModel[0];
            UnitTypeModel[] units = new UnitTypeModel[0];
            FirmModel[] firms = new FirmModel[0];
            ForexTypeModel[] forexes = new ForexTypeModel[0];
            WarehouseModel[] warehouses = new WarehouseModel[0];
            PlantModel[] plants = new PlantModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                items = bObj.GetItemList();
                units = bObj.GetUnitTypeList();
                firms = bObj.GetFirmList();
                forexes = bObj.GetForexTypeList();
                warehouses = bObj.GetWarehouseList();
                plants = bObj.GetPlantList();
            }

            Dictionary<int, string> receiptTypes =
                DictItemReceiptType.GetReceiptTypes((ReceiptCategoryType)receiptCategory);

            var jsonResult = Json(new { 
                Items = items, Units = units, 
                Firms = firms, Forexes=forexes,
                Warehouses = warehouses,
                Plants = plants,
                ReceiptTypes = receiptTypes.Select(d => new { 
                    Id=d.Key,
                    Text=d.Value
                }).ToArray()
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetReceiptTypes(int receiptCategory)
        {
            Dictionary<int, string> receiptTypes =
                DictItemReceiptType.GetReceiptTypes((ReceiptCategoryType)receiptCategory);

            var jsonResult = Json(new
            {
                ReceiptTypes = receiptTypes.Select(d => new {
                    Id = d.Key,
                    Text = d.Value
                }).ToArray()
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetReceiptList(int receiptCategory, int receiptType)
        {
            ItemReceiptModel[] result = new ItemReceiptModel[0];

            using (ReceiptBO bObj = new ReceiptBO())
            {
                int? rType = receiptType == 0 ? (int?)null : receiptType;

                result = bObj.GetItemReceiptList(
                        (ReceiptCategoryType)receiptCategory,
                        (ItemReceiptType?)rType
                    );
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            ItemReceiptModel model = null;
            using (ReceiptBO bObj = new ReceiptBO())
            {
                model = bObj.GetItemReceipt(rid);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteModel(int rid)
        {
            try
            {
                BusinessResult result = null;
                using (ReceiptBO bObj = new ReceiptBO())
                {
                    result = bObj.DeleteItemReceipt(rid);
                }

                if (result.Result)
                    return Json(new { Status = 1 });
                else
                    throw new Exception(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                return Json(new { Status = 0, ErrorMessage = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult PrintMaterialLabel(int receiptDetailId)
        {
            BusinessResult result = null;

            using (ReceiptBO bObj = new ReceiptBO())
            {
                var printerId = bObj.GetParameter("DefaultProductPrinter", Convert.ToInt32(Request.Cookies["PlantId"].Value));

                result = bObj.AddToPrintQueue(new PrinterQueueModel
                {
                    PrinterId = Convert.ToInt32(printerId),
                    RecordType = (int)RecordType.ItemReceiptDetail,
                    RecordId = receiptDetailId,
                    CreatedDate = DateTime.Now,
                    
                });
            }

            return Json(new { Status = result.Result ? 1 : 0, ErrorMessage = result.ErrorMessage });
        }

        [HttpPost]
        public JsonResult SaveModel(ItemReceiptModel model)
        {
            try
            {
                BusinessResult result = null;
                using (ReceiptBO bObj = new ReceiptBO())
                {
                    model.PlantId = Convert.ToInt32(Request.Cookies["PlantId"].Value);

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

        [HttpPost]
        public JsonResult TestPrintDelivery(int receiptId)
        {
            string outputFile = Session.SessionID + ".pdf";

            using (ReportingBO bObj = new ReportingBO())
            {
                var reportData = (List<DeliverySerialListModel>)bObj.PrepareReportData(receiptId, ReportType.DeliverySerialList);

                bObj.ExportReportAsPdf<List<DeliverySerialListModel>>(1, reportData, Server.MapPath("~/Outputs") + "/",
                    Session.SessionID + ".pdf");
            }

            return Json(new { Status = 1, Path = outputFile });
        }

        [HttpGet]
        public JsonResult GetRelatedOrderList(int receiptId)
        {
            ItemOrderModel[] result = new ItemOrderModel[0];

            using (OrdersBO bObj = new OrdersBO())
            {
                result = bObj.GetRelatedOrders(receiptId);
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        #region CALCULATIONS
        [HttpPost]
        public JsonResult CalculateRow(ItemReceiptDetailModel model)
        {
            ItemReceiptDetailModel result = new ItemReceiptDetailModel();

            using (ReceiptBO bObj = new ReceiptBO())
            {
                result = bObj.CalculateReceiptDetail(model);
            }

            return Json(result);
        }
        #endregion
    }
}