using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.DataTransfer.Receipt;
using HekaMOLD.Business.Models.DataTransfer.Summary;
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
    public class WorkOrderController : Controller
    {
        // GET: Firm
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetNextWorkOrderNo()
        {
            string workOrderNo = "";

            using (ProductionBO bObj = new ProductionBO())
            {
                workOrderNo = bObj.GetNextWorkOrderNo();
            }

            var jsonResult = Json(new { Result = !string.IsNullOrEmpty(workOrderNo), WorkOrderNo = workOrderNo }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetWorkOrderDetailList()
        {
            WorkOrderDetailModel[] result = new WorkOrderDetailModel[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                result = bObj.GetWorkOrderDetailList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetItemDeliveryList(int workOrderDetailId)
        {
            ItemReceiptDetailModel[] result = new ItemReceiptDetailModel[0];

            using (PlanningBO bObj = new PlanningBO())
            {
                result = bObj.GetDeliveredItems(workOrderDetailId);
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult DeleteItemDelivery(int receiptDetailId)
        {
            try
            {
                BusinessResult result = null;
                using (ReceiptBO bObj = new ReceiptBO())
                {
                    result = bObj.DeleteReceiptDetail(receiptDetailId);
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
        public JsonResult UpdateItemDelivery(ItemReceiptDetailModel model)
        {
            try
            {
                BusinessResult result = null;
                using (ReceiptBO bObj = new ReceiptBO())
                {
                    result = bObj.UpdateReceiptDetail(model);
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

        [HttpGet]
        public JsonResult GetSelectables()
        {
            ItemModel[] items = new ItemModel[0];
            UnitTypeModel[] units = new UnitTypeModel[0];
            FirmModel[] firms = new FirmModel[0];
            ForexTypeModel[] forexes = new ForexTypeModel[0];
            MoldModel[] molds = new MoldModel[0];
            MoldTestModel[] moldTests = new MoldTestModel[0];
            WorkOrderCategoryModel[] workCategories = new WorkOrderCategoryModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                items = bObj.GetItemList();
                units = bObj.GetUnitTypeList();
                firms = bObj.GetFirmList();
                forexes = bObj.GetForexTypeList();
                molds = bObj.GetMoldList();
                workCategories = bObj.GetWorkOrderCategoryList();
            }

            using (MoldBO bObj = new MoldBO())
            {
                moldTests = bObj.GetMoldTestList();
            }

            var jsonResult = Json(new { Items = items, Units = units, 
                Firms = firms, Forexes = forexes,
                Molds = molds, MoldTests = moldTests,
                WorkCategories = workCategories,
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        /// <summary>
        /// TO-DO
        /// </summary>
        /// <param name="rid"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            WorkOrderModel model = null;
            using (ProductionBO bObj = new ProductionBO())
            {
                model = bObj.GetWorkOrder(rid);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// TO-DO
        /// </summary>
        /// <param name="rid"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteModel(int rid)
        {
            try
            {
                BusinessResult result = null;
                using (ProductionBO bObj = new ProductionBO())
                {
                    result = bObj.DeleteWorkOrder(rid);
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

        /// <summary>
        /// TO-DO
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveModel(WorkOrderModel model)
        {
            try
            {
                BusinessResult result = null;
                using (ProductionBO bObj = new ProductionBO())
                {
                    int plantId = Convert.ToInt32(Request.Cookies["PlantId"].Value);
                    model.PlantId = plantId;

                    result = bObj.SaveOrUpdateWorkOrder(model);
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

        public ActionResult History()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetProductionHistory(string dt1, string dt2)
        {
            ProductionHistoryModel[] data = new ProductionHistoryModel[0];

            try
            {
                using (ReportingBO bObj = new ReportingBO())
                {
                    data = bObj.GetProductionHistory(new Business.Models.Filters.BasicRangeFilter
                    {
                        StartDate = dt1,
                        EndDate = dt2,
                    });
                }
            }
            catch (Exception)
            {

            }

            var jsonResponse = Json(data, JsonRequestBehavior.AllowGet);
            jsonResponse.MaxJsonLength = int.MaxValue;
            return jsonResponse;
        }
    }
}