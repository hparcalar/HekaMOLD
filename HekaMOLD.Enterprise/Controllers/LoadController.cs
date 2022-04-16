using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Logistics;
using HekaMOLD.Business.Models.DataTransfer.Reporting;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using System;
using System.Collections.Generic;
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
        public ActionResult Finance()
        {
            return View();
        }
        public ActionResult IndexExport()
        {
            return View();
        }
        public ActionResult IndexImport()
        {
            return View();
        }
        public ActionResult IndexDomestic()
        {
            return View();
        }
        public ActionResult IndexTransit()
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
        public ActionResult Calendar()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetNextloadCode(int directionId)
        {
            string loadCode = "";

            using (LoadBO bObj = new LoadBO())
            {
                loadCode = bObj.GetNextLoadCode(directionId);
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
            FirmModel[] firmCustoms = new FirmModel[0];
            VehicleModel[] vehicleTrailers = new VehicleModel[0];
            VehicleModel[] vehicleTowinfs = new VehicleModel[0];
            ServiceItemModel[] serviceItems = new ServiceItemModel[0];
            DriverModel[] drivers = new DriverModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                items = bObj.GetItemList();
                units = bObj.GetUnitTypeList();
                firms = bObj.GetFirmList();
                forexes = bObj.GetForexTypeList();
                customs = bObj.GetCustomsList();
                citys = bObj.GetCityList();
                countrys = bObj.GetCountryList();
                firmCustoms = bObj.GetFirmCustomsList();
                vehicleTrailers = bObj.GetVehicleCanBePlanedList();
                vehicleTowinfs = bObj.GetVehicleTowingList();
                serviceItems = bObj.GetServiceItemList();
            }
            using (UsersBO bObj = new UsersBO())
            {
                users = bObj.GetUserList();
                drivers = bObj.GetDriverList();
            }

            var jsonResult = Json(new
            {
                Items = items,
                Units = units,
                Firms = firms,
                Forexes = forexes,
                Customs = customs,
                Users = users,
                Citys = citys,
                Countrys = countrys,
                FirmCustoms = firmCustoms,
                VehicleTrailers = vehicleTrailers,
                VehicleTowinfs = vehicleTowinfs,
                Drivers = drivers,
                ServiceItems = serviceItems
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetInvoiceSelectables()
        {
            FirmModel[] firms = new FirmModel[0];
            ForexTypeModel[] forexes = new ForexTypeModel[0];
            ServiceItemModel[] serviceItems = new ServiceItemModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                firms = bObj.GetFirmList();
                forexes = bObj.GetForexTypeList();
                serviceItems = bObj.GetServiceItemList();
            }

            var jsonResult = Json(new
            {
                Firms = firms,
                Forexes = forexes,
                ServiceItems = serviceItems
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        [HttpGet]
        public JsonResult GetLoadCalendarList()
        {
            LoadCalendarModel[] load = new LoadCalendarModel[0];

            using (LoadBO bObj = new LoadBO())
            {
                load = bObj.GetLoadCalendarList();
            }

            var jsonResult = Json(new
            {
                Load = load
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

                    int userId = Convert.ToInt32(Request.Cookies["UserId"].Value);
                    result = bObj.SaveOrUpdateLoad(model, userId);
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

        //[HttpPost]
        //public JsonResult ApproveLoad(int rid)
        //{
        //    BusinessResult result = new BusinessResult();

        //    using (LoadBO bObj = new LoadBO())
        //    {
        //        result = bObj.ApproveLoad(rid, Convert.ToInt32(Request.Cookies["UserId"].Value));
        //    }

        //    return Json(result);
        //}
        [HttpGet]
        public JsonResult GetNextRecord(int Id)
        {
            int nextNo = 0;

            using (LoadBO bObj = new LoadBO())
            {
                nextNo = bObj.GetNextRecord(Convert.ToInt32(Request.Cookies["PlantId"].Value), Id);
            }

            var jsonResult = Json(new { Result = nextNo, NextNo = nextNo }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        [HttpGet]
        public JsonResult GetBackRecord(int Id)
        {
            int nextNo = 0;

            using (LoadBO bObj = new LoadBO())
            {
                nextNo = bObj.GetBackRecord(Convert.ToInt32(Request.Cookies["PlantId"].Value), Id);
            }

            var jsonResult = Json(new { Result = nextNo, NextNo = nextNo }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        [HttpPost]
        public JsonResult TestPrintDelivery(int loadId)
        {
            string outputFile = Session.SessionID + ".pdf";

            using (ReportingBO bObj = new ReportingBO())
            {
                var reportData = (List<LoadCmrModel>)bObj.PrepareReportData(loadId, ReportType.Cmr);

                bObj.ExportReportAsPdf<List<LoadCmrModel>>(1, reportData, Server.MapPath("~/Outputs") + "/",
                    Session.SessionID + ".pdf");
            }

            return Json(new { Status = 1, Path = outputFile });
        }

        public JsonResult CancelledLoad(int rid)
        {
            BusinessResult result = new BusinessResult();

            using (LoadBO bObj = new LoadBO())
            {
                result = bObj.CancelledLoad(rid, Convert.ToInt32(Request.Cookies["UserId"].Value));
            }

            return Json(result);
        }
        #region CALCULATIONS
        [HttpPost]
        public JsonResult CalculateRow(LoadInvoiceModel model)
        {
            LoadInvoiceModel result = new LoadInvoiceModel();

            using (ReceiptBO bObj = new ReceiptBO())
            {
                //result = bObj.CalculateLoadInvoice(model);
            }

            return Json(result);
        }
        #endregion
    }
}