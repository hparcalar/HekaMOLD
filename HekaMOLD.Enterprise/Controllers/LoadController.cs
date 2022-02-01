using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Logistics;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
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
        public ActionResult ExportList()
        {
            return View();
        }
        public ActionResult ImportList()
        {
            return View();
        }
        public ActionResult DomesticList()
        {
            return View();
        }
        public ActionResult TransitList()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetNextloadCode(int directionId)
        {
            string loadCode = "";

            using (LoadBO bObj = new LoadBO())
            {
                loadCode = bObj.GetNextLoadCode( directionId);
            }

            var jsonResult = Json(new { Result = !string.IsNullOrEmpty(loadCode), LoadCode = loadCode }, JsonRequestBehavior.AllowGet);
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
            CustomsModel[] customs = new CustomsModel[0];
            UserModel[] users = new UserModel[0];
            CityModel[] citys = new CityModel[0];
            CountryModel[] countrys = new CountryModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                items = bObj.GetItemList();
                units = bObj.GetUnitTypeList();
                firms = bObj.GetFirmList();
                forexes = bObj.GetForexTypeList();
                customs = bObj.GetCustomsList();
                citys = bObj.GetCityList();
                countrys = bObj.GetCountryList();
            }
            using (UsersBO bObj =new UsersBO())
            {
                users = bObj.GetUserList();
            }

            var jsonResult = Json(new { 
                Items = items, 
                Units = units, 
                Firms = firms, 
                Forexes = forexes, 
                Customs = customs , 
                Users = users,
                Citys = citys,
                Countrys = countrys
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetItemLoadList()
        {
            ItemLoadModel[] result = new ItemLoadModel[0];

            using (LoadBO bObj = new LoadBO())
            {
                result = bObj.GetItemLoadList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetItemLoadExportList()
        {
            ItemLoadModel[] result = new ItemLoadModel[0];

            using (LoadBO bObj = new LoadBO())
            {
                result = bObj.GetItemLoadExportList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetItemLoadImportList()
        {
            ItemLoadModel[] result = new ItemLoadModel[0];

            using (LoadBO bObj = new LoadBO())
            {
                result = bObj.GetItemLoadImportList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetItemLoadDomesticList()
        {
            ItemLoadModel[] result = new ItemLoadModel[0];

            using (LoadBO bObj = new LoadBO())
            {
                result = bObj.GetItemLoadDomesticList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetItemLoadTransitList()
        {
            ItemLoadModel[] result = new ItemLoadModel[0];

            using (LoadBO bObj = new LoadBO())
            {
                result = bObj.GetItemLoadTransitList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            ItemLoadModel model = null;
            using (LoadBO bObj = new LoadBO())
            {
                model = bObj.GetLoad(rid);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteModel(int rid)
        {
            try
            {
                BusinessResult result = null;
                using (LoadBO bObj = new LoadBO())
                {
                    result = bObj.DeleteLoad(rid);
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
        public JsonResult SaveModel(ItemLoadModel model)
        {
            try
            {
                BusinessResult result = null;
                using (LoadBO bObj = new LoadBO())
                {
                    model.PlantId = Convert.ToInt32(Request.Cookies["PlantId"].Value);

                    //model.OrderType = (int)ItemOrderType.Purchase;
                    result = bObj.SaveOrUpdateLoad(model);
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
        public JsonResult GetNextReceiptNo()
        {
            string receiptNo = "";

            using (RequestBO bObj = new RequestBO())
            {
                receiptNo = bObj.GetNextRequestNo(Convert.ToInt32(Request.Cookies["PlantId"].Value));
            }

            var jsonResult = Json(new { Result = !string.IsNullOrEmpty(receiptNo), ReceiptNo = receiptNo }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult ApproveLoad(int rid)
        {
            BusinessResult result = new BusinessResult();

            using (LoadBO bObj = new LoadBO())
            {
                result = bObj.ApproveLoad(rid, Convert.ToInt32(Request.Cookies["UserId"].Value));
            }

            return Json(result);
        }

    }
}