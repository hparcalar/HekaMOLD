using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Order;
using HekaMOLD.Business.Models.DataTransfer.Request;
using HekaMOLD.Business.Models.Operational;
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
    public class SIOrderController : Controller
    {
        // GET: SIOrder
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetNextOrderNo()
        {
            string receiptNo = "";

            using (OrdersBO bObj = new OrdersBO())
            {
                receiptNo = bObj.GetNextOrderNo(Convert.ToInt32(Request.Cookies["PlantId"].Value), ItemOrderType.Sale);
            }

            var jsonResult = Json(new { Result = !string.IsNullOrEmpty(receiptNo), ReceiptNo = receiptNo }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetSelectables()
        {
            ItemModel[] items = new ItemModel[0];
            UnitTypeModel[] units = new UnitTypeModel[0];
            FirmModel[] firms = new FirmModel[0];
            ForexTypeModel[] forexes = new ForexTypeModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                items = bObj.GetItemList();
                units = bObj.GetUnitTypeList();
                firms = bObj.GetFirmList();
                forexes = bObj.GetForexTypeList();
            }

            var jsonResult = Json(new { Items = items, Units = units, Firms = firms, Forexes = forexes }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetItemOrderList()
        {
            ItemOrderModel[] result = new ItemOrderModel[0];

            using (OrdersBO bObj = new OrdersBO())
            {
                result = bObj.GetItemOrderList(ItemOrderType.Sale);

                foreach (var item in result)
                {
                    if (item.OrderStatus == (int)OrderStatusType.Approved || item.OrderStatus == (int)OrderStatusType.Created)
                        item.OrderStatusStr = "Planlanması bekleniyor";
                }
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            ItemOrderModel model = null;
            using (OrdersBO bObj = new OrdersBO())
            {
                model = bObj.GetItemOrder(rid);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteModel(int rid)
        {
            try
            {
                BusinessResult result = null;
                using (OrdersBO bObj = new OrdersBO())
                {
                    result = bObj.DeleteItemOrder(rid);
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
        public JsonResult SaveModel(ItemOrderModel model)
        {
            try
            {
                BusinessResult result = null;
                using (OrdersBO bObj = new OrdersBO())
                {
                    model.PlantId = Convert.ToInt32(Request.Cookies["PlantId"].Value);

                    model.OrderType = (int)ItemOrderType.Sale;
                    result = bObj.SaveOrUpdateItemOrder(model);
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
        public JsonResult ApproveOrderPrice(int rid)
        {
            BusinessResult result = new BusinessResult();

            using (OrdersBO bObj = new OrdersBO())
            {
                result = bObj.ApproveItemOrderPrice(rid, Convert.ToInt32(Request.Cookies["UserId"].Value));
            }

            return Json(result);
        }

        #region CALCULATIONS
        [HttpPost]
        public JsonResult CalculateRow(ItemOrderDetailModel model)
        {
            ItemOrderDetailModel result = new ItemOrderDetailModel();

            using (OrdersBO bObj = new OrdersBO())
            {
                result = bObj.CalculateOrderDetail(model);
            }

            return Json(result);
        }
        #endregion
    }
}