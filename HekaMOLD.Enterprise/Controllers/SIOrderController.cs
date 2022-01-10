﻿using HekaMOLD.Business.Models.Constants;
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

        public ActionResult OpenDetails()
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
            PaymentPlanModel[] paymentPlans = new PaymentPlanModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                items = bObj.GetItemList();
                units = bObj.GetUnitTypeList();
                firms = bObj.GetFirmList();
                forexes = bObj.GetForexTypeList();
                paymentPlans = bObj.GetPaymentPlanList();
            }

            var jsonResult = Json(new { Items = items, Units = units, Firms = firms, 
                Forexes = forexes,
                PaymentPlans = paymentPlans,
            }, JsonRequestBehavior.AllowGet);
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
        public JsonResult GetOpenOrderDetailList()
        {
            ItemOrderDetailModel[] result = new ItemOrderDetailModel[0];

            using (OrdersBO bObj = new OrdersBO())
            {
                result = bObj.GetOpenOrderDetailList(ItemOrderType.Sale);

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

            var jsonResponse = Json(model, JsonRequestBehavior.AllowGet);
            jsonResponse.MaxJsonLength = int.MaxValue;

            return jsonResponse;
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
                    if (!Request.Cookies.AllKeys.Contains("PlantId") || Request.Cookies["PlantId"] == null)
                        throw new Exception("Sisteme yeniden giriş yapmanız gerekmektedir.");

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

        [HttpPost]
        public JsonResult ToggleOrderDetailStatus(int detailId)
        {
            BusinessResult result = null;

            using (OrdersBO bObj = new OrdersBO())
            {
                result = bObj.ToggleOrderDetailStatus(detailId);
            }

            return Json(new { Status = result.Result ? 1 : 0, ErrorMessage = result.ErrorMessage });
        }

        [HttpGet]
        [FreeAction]
        public JsonResult GetOpenOrderDetails()
        {
            ItemOrderDetailModel[] result = new ItemOrderDetailModel[0];

            using (OrdersBO bObj = new OrdersBO())
            {
                result = bObj.GetOpenSaleOrderDetails(Convert.ToInt32(Request.Cookies["PlantId"].Value));
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
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