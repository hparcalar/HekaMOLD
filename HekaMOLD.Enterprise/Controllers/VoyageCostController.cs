using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Logistics;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    public class VoyageCostController : Controller
    {
        // GET: VoyageCost
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List()
        {
            return View();
        }

        [HttpGet]
        public JsonResult BindModel(int rid, int vid)
        {
            VoyageCostModel model = null;
            using (VoyageBO bObj = new VoyageBO())
            {
                model = bObj.GetVoyageCost(rid, vid);
            }

            var jsonResponse = Json(model, JsonRequestBehavior.AllowGet);
            jsonResponse.MaxJsonLength = int.MaxValue;

            return jsonResponse;
        }
        [HttpGet]
        public JsonResult BindModelByVoyage(int rid)
        {
            VoyageCostModel model = null;
            using (VoyageBO bObj = new VoyageBO())
            {
                model = bObj.GetVoyageCostByVoyage(rid);
            }

            var jsonResponse = Json(model, JsonRequestBehavior.AllowGet);
            jsonResponse.MaxJsonLength = int.MaxValue;

            return jsonResponse;
        }
        [HttpGet]
        public JsonResult GetSelectables()
        {
            VehicleModel[] towingVehicles = new VehicleModel[0];
            VehicleModel[] trailerVehicles = new VehicleModel[0];

            DriverModel[] drivers = new DriverModel[0];
            ForexTypeModel[] forexTypes = new ForexTypeModel[0];
            UnitTypeModel[] unitTypes = new UnitTypeModel[0];
            CountryModel[] countrys = new CountryModel[0];
            ItemLoadModel[] waitingLoads = new ItemLoadModel[0];
            CostCategoryModel[] costCategorys = new CostCategoryModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                towingVehicles = bObj.GetVehicleTowingList();
                trailerVehicles = bObj.GetVehicleCanBePlanedList();
                forexTypes = bObj.GetForexTypeList();
                countrys = bObj.GetCountryList();
                costCategorys = bObj.GetCostCategoryList();
                unitTypes = bObj.GetUnitTypeList();
            }
            using (UsersBO bObj = new UsersBO())
            {
                drivers = bObj.GetDriverList();
            }
            using (PlanningBO bObj = new PlanningBO())
            {
                waitingLoads = bObj.GetWaitingLoads();
            }
            var jsonResult = Json(new
            {
                TowingVehicles = towingVehicles,
                TrailerVehicles = trailerVehicles,
                Drivers = drivers,
                ForexTypes = forexTypes,
                Countrys = countrys,
                WaitingLoads = waitingLoads,
                CostCategorys = costCategorys,
                UnitTypes = unitTypes
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult SaveModel(VoyageCostModel model)
        {
            try
            {
                BusinessResult result = null;
                using (VoyageBO bObj = new VoyageBO())
                {

                    int userId = Convert.ToInt32(Request.Cookies["UserId"].Value);
                    result = bObj.SaveOrUpdateVoyageCost(model,userId);
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
        public JsonResult GetVoyageCostList()
        {
            VoyageCostModel[] result = new VoyageCostModel[0];

            using (VoyageBO bObj = new VoyageBO())
            {
                result = bObj.GetVoyageCostList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
    }
}