using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Logistics;
using HekaMOLD.Business.UseCases;
using System;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    public class VoyageController : Controller
    {
        // GET: Voyage
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List()
        {
            return View();
        }
        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            VoyageModel model = null;
            using (VoyageBO bObj = new VoyageBO())
            {
                model = bObj.GetVoyage(rid);
            }

            var jsonResponse = Json(model, JsonRequestBehavior.AllowGet);
            jsonResponse.MaxJsonLength = int.MaxValue;

            return jsonResponse;
        }

        [HttpGet]
        public JsonResult GetSelectables()
        {
            VehicleModel[] towinfVehicles = new VehicleModel[0];
            VehicleModel[] trailerVehicles = new VehicleModel[0];

            DriverModel[] drivers = new DriverModel[0];
            RotaModel[] rotas = new RotaModel[0];
            FirmModel[] firms = new FirmModel[0];
            CustomsDoorModel[] cDoors = new CustomsDoorModel[0];
            ForexTypeModel[] forexTypes = new ForexTypeModel[0];
            CityModel[] citys = new CityModel[0];
            CountryModel[] countrys = new CountryModel[0];
            CustomsModel[] customs = new CustomsModel[0];
            ItemLoadModel [] waitingLoads = new ItemLoadModel[0];


            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                towinfVehicles = bObj.GetVehicleTowingList();
                trailerVehicles = bObj.GetVehicleCanBePlanedList();
                firms = bObj.GetFirmList();
                cDoors = bObj.GetCustomsDoorList();
                forexTypes = bObj.GetForexTypeList();
                citys = bObj.GetCityList();
                countrys = bObj.GetCountryList();
                customs = bObj.GetCustomsList();
            }
            using (UsersBO bObj = new UsersBO())
            {
                drivers = bObj.GetDriverList();
                rotas = bObj.GetRotaList();
            }
            using (PlanningBO bObj = new PlanningBO())
            {
                waitingLoads = bObj.GetWaitingLoads();
            }
            var jsonResult = Json(new
            {
                TowinfVehicles = towinfVehicles,
                TrailerVehicles = trailerVehicles,
                Drivers = drivers,
                Rotas = rotas,
                Firms = firms,
                CDoors = cDoors,
                ForexTypes = forexTypes,
                Citys = citys,
                Countrys = countrys,
                Customs = customs,
                WaitingLoads = waitingLoads
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetVoyageList()
        {
            VoyageModel[] result = new VoyageModel[0];

            using (VoyageBO bObj = new VoyageBO())
            {
                result = bObj.GetVoyageList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetVoyageDetailList()
        {
            VoyageDetailModel[] result = new VoyageDetailModel[0];

            using (VoyageBO bObj = new VoyageBO())
            {
                result = bObj.GetVoyageDetailList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetVoyageCode(string strParam)
        {
            string voyageCode = "";

            using (VoyageBO bObj = new VoyageBO())
            {
                voyageCode = bObj.GetVoyageCode(strParam);
            }
            var jsonResult = Json(new { Result = !string.IsNullOrEmpty(voyageCode), VoyageCode = voyageCode }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetNextRecord(int Id)
        {
            int nextNo = 0;

            using (VoyageBO bObj = new VoyageBO())
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

            using (VoyageBO bObj = new VoyageBO())
            {
                nextNo = bObj.GetBackRecord(Convert.ToInt32(Request.Cookies["PlantId"].Value), Id);
            }

            var jsonResult = Json(new { Result = nextNo, NextNo = nextNo }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
    }
}