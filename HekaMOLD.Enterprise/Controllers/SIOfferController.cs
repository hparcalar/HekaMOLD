using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Offers;
using HekaMOLD.Business.Models.DataTransfer.Production;
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
    public class SIOfferController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetNextOfferNo()
        {
            string receiptNo = "";

            using (OffersBO bObj = new OffersBO())
            {
                receiptNo = bObj.GetNextOfferNo();
            }

            var jsonResult = Json(new { Result = !string.IsNullOrEmpty(receiptNo), ReceiptNo = receiptNo }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetSelectables()
        {
            ItemModel[] items = new ItemModel[0];
            FirmModel[] firms = new FirmModel[0];
            RouteModel[] routes = new RouteModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                items = bObj.GetItemList();
                firms = bObj.GetFirmList();
                routes = bObj.GetRouteList();
            }

            var jsonResult = Json(new { Items = items, Firms = firms, Routes = routes }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetItemOfferList()
        {
            ItemOfferModel[] result = new ItemOfferModel[0];

            using (OffersBO bObj = new OffersBO())
            {
                result = bObj.GetItemOfferList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetProcessList(int routeId)
        {
            ItemOfferDetailRoutePricingModel[] result = new ItemOfferDetailRoutePricingModel[0];

            using (OffersBO bObj = new OffersBO())
            {
                result = bObj.GetPricingsByRoute(routeId);
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            ItemOfferModel model = null;
            using (OffersBO bObj = new OffersBO())
            {
                model = bObj.GetItemOffer(rid);
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
                using (OffersBO bObj = new OffersBO())
                {
                    result = bObj.DeleteItemOffer(rid);
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
        public JsonResult SaveModel(ItemOfferModel model)
        {
            try
            {
                BusinessResult result = null;
                using (OffersBO bObj = new OffersBO())
                {
                    if (!Request.Cookies.AllKeys.Contains("PlantId") || Request.Cookies["PlantId"] == null)
                        throw new Exception("Sisteme yeniden giriş yapmanız gerekmektedir.");

                    model.PlantId = Convert.ToInt32(Request.Cookies["PlantId"].Value);

                    model.OfferType = 1;
                    result = bObj.SaveOrUpdateItemOffer(model);
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
        public JsonResult CreateSaleOrder(int rid)
        {
            try
            {
                BusinessResult result = null;
                using (OffersBO bObj = new OffersBO())
                {
                    result = bObj.CreateSaleOrder(rid);
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

        #region CALCULATIONS
        [HttpPost]
        public JsonResult CalculateRow(ItemOfferDetailModel model)
        {
            ItemOfferDetailModel result = new ItemOfferDetailModel();

            using (OffersBO bObj = new OffersBO())
            {
                result = bObj.CalculateOfferDetail(model);
            }

            return Json(result);
        }
        #endregion
    }
}