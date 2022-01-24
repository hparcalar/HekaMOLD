using HekaMOLD.Business.Models.Operational;
using System;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    public class LoadController : Controller
    {
        // GET: Load
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List()
        {
            return View();
        }

        //[HttpGet]
        //public JsonResult GetNextReceiptNo()
        //{
        //    string receiptNo = "";

        //    using (RequestBO bObj = new RequestBO())
        //    {
        //        receiptNo = bObj.GetNextRequestNo(Convert.ToInt32(Request.Cookies["PlantId"].Value));
        //    }

        //    var jsonResult = Json(new { Result = !string.IsNullOrEmpty(receiptNo), ReceiptNo = receiptNo }, JsonRequestBehavior.AllowGet);
        //    jsonResult.MaxJsonLength = int.MaxValue;
        //    return jsonResult;
        //}

        //[HttpGet]
        //public JsonResult GetSelectables()
        //{
        //    ItemModel[] items = new ItemModel[0];
        //    UnitTypeModel[] units = new UnitTypeModel[0];
        //    ItemRequestCategoryModel[] categories = new ItemRequestCategoryModel[0];

        //    using (DefinitionsBO bObj = new DefinitionsBO())
        //    {
        //        items = bObj.GetItemList();
        //        units = bObj.GetUnitTypeList();
        //    }

        //    using (RequestBO bObj = new RequestBO())
        //    {
        //        categories = bObj.GetRequestCategoryList();
        //    }

        //    var jsonResult = Json(new { Items = items, Units = units, Categories = categories }, JsonRequestBehavior.AllowGet);
        //    jsonResult.MaxJsonLength = int.MaxValue;
        //    return jsonResult;
        //}
        //[HttpPost]
        //public JsonResult CreateLoad(int rid)
        //{
        //    BusinessResult result = new BusinessResult();

        //    using (RequestBO bObj = new RequestBO())
        //    {
        //        result = bObj.CreatePurchaseOrder(rid, Convert.ToInt32(Request.Cookies["UserId"].Value));
        //    }

        //    return Json(result);
        //}
    }
}