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

namespace HekaMOLD.Enterprise.Controllers
{
    [MobileAuthFilter]
    public class MobileController : Controller
    {
        // GET: Mobile
        public ActionResult Index()
        {
            if (this.IsGranted("MobileProductionUser"))
                return RedirectToAction("Production");
            else if (this.IsGranted("MobileMechanicUser"))
                return RedirectToAction("Mechanic");
            else if (this.IsGranted("MobileWarehouseUser"))
                return RedirectToAction("Warehouse");

            return View();
        }

        [HttpGet]
        public JsonResult GetSelectables(string action)
        {
            WarehouseModel[] warehouses = new WarehouseModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                warehouses = bObj.GetWarehouseList();
            }

            var jsonResult = Json(new
            {
                Warehouses = warehouses
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
                result = bObj.ToggleWorkOrderStatus(workOrderDetailId);
            }

            return Json(result);
        }

        public ActionResult ProductEntry()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SaveProductEntry(int workOrderDetailId)
        {
            BusinessResult result = new BusinessResult();

            int userId = Convert.ToInt32(Request.Cookies["UserId"].Value);
            string serialTypeConfig = WebConfigurationManager.AppSettings["WorkOrderSerialType"];

            WorkOrderSerialType serialType = WorkOrderSerialType.SingleProduct;
            if (serialTypeConfig == "ProductPackage")
                serialType = WorkOrderSerialType.ProductPackage;

            using (ProductionBO bObj = new ProductionBO())
            {
                result = bObj.AddProductEntry(workOrderDetailId, userId, serialType);
            }

            return Json(result);
        }

        public ActionResult PostureEntry()
        {
            return View();
        }

        public ActionResult OngoingPostures()
        {
            return View();
        }

        public ActionResult ProductEntryList()
        {
            return View();
        }

        public ActionResult FaultEntry()
        {
            return View();
        }

        public ActionResult OngoingFaults()
        {
            return View();
        }
        #endregion
    }
}