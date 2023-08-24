using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Request;
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
    public class PIRequestController : Controller
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
        public JsonResult GetNextReceiptNo()
        {
            string receiptNo = "";

            using (RequestBO bObj = new RequestBO())
            {
                receiptNo = bObj.GetNextRequestNo(Convert.ToInt32(Request.Cookies["PlantId"].Value));
            }

            var jsonResult = Json(new { Result=!string.IsNullOrEmpty(receiptNo), ReceiptNo=receiptNo }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetSelectables()
        {
            ItemModel[] items = new ItemModel[0];
            UnitTypeModel[] units = new UnitTypeModel[0];
            ItemRequestCategoryModel[] categories = new ItemRequestCategoryModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                items = bObj.GetItemList();
                units = bObj.GetUnitTypeList();
            }

            using (RequestBO bObj = new RequestBO())
            {
                categories = bObj.GetRequestCategoryList();
            }

            var jsonResult = Json(new { Items = items, Units = units, Categories = categories }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetItemRequestList(string dt1, string dt2)
        {
            ItemRequestModel[] result = new ItemRequestModel[0];

            using (RequestBO bObj = new RequestBO())
            {
                result = bObj.GetItemRequestList(dt1, dt2);
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            ItemRequestModel model = null;
            using (RequestBO bObj = new RequestBO())
            {
                model = bObj.GetItemRequest(rid);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteModel(int rid)
        {
            try
            {
                BusinessResult result = null;
                using (RequestBO bObj = new RequestBO())
                {
                    result = bObj.DeleteItemRequest(rid);
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
        public JsonResult SaveModel(ItemRequestModel model)
        {
            try
            {
                BusinessResult result = null;
                using (RequestBO bObj = new RequestBO())
                {
                    model.PlantId = Convert.ToInt32(Request.Cookies["PlantId"].Value);

                    result = bObj.SaveOrUpdateItemRequest(model);
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
        public JsonResult ApprovePoRequest(int rid)
        {
            BusinessResult result = new BusinessResult();

            using (RequestBO bObj = new RequestBO())
            {
                result = bObj.ApprovePoRequest(rid, Convert.ToInt32(Request.Cookies["UserId"].Value));
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult CreatePurchaseOrder(int rid)
        {
            BusinessResult result = new BusinessResult();

            using (RequestBO bObj = new RequestBO())
            {
                result = bObj.CreatePurchaseOrder(rid, Convert.ToInt32(Request.Cookies["UserId"].Value));
            }

            return Json(result);
        }
    }
}