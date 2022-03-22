using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Logistics;
using HekaMOLD.Business.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    public class DriverAccountController : Controller
    {
        // GET: DriverAccount
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
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
        [HttpGet]
        public JsonResult GetDriverAccountList()
        {
            DriverAccountModel [] result = new DriverAccountModel[0];

            using (DriverAccountBO bObj = new DriverAccountBO())
            {
                result = bObj.GetDriverAccountList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            DriverAccountModel model = null;
            using (DriverAccountBO bObj = new DriverAccountBO())
            {
                model = bObj.GetDriverAccount(rid);
            }

            var jsonResponse = Json(model, JsonRequestBehavior.AllowGet);
            jsonResponse.MaxJsonLength = int.MaxValue;
            return jsonResponse;
        }
    }
}