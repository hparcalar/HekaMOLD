using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Logistics;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using System;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    public class LPlanningController : Controller
    {
        // GET: LPlanning
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Planning()
        {
            return View();
        }
        public ActionResult List()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetVehicleList()
        {
            VehicleModel[] result = new VehicleModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                result = bObj.GetVehicleCanBePlanedList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetSelectables()
        {
            VehicleModel[] vehicles = new VehicleModel[0];
            DriverModel[] drivers = new DriverModel[0];
            RotaModel[] rotas = new RotaModel[0];
            FirmModel[] firms = new FirmModel[0];
            CustomsDoorModel[] cDoors = new CustomsDoorModel[0];
            ForexTypeModel[] forexTypes = new ForexTypeModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                vehicles = bObj.GetVehicleList();
                firms = bObj.GetFirmList();
                cDoors = bObj.GetCustomsDoorList();
                forexTypes = bObj.GetForexTypeList();
            }
            using (UsersBO bObj = new UsersBO())
            {
                drivers = bObj.GetDriverList();

            }
            using (UsersBO bObj = new UsersBO())
            {
                rotas = bObj.GetRotaList();
            }
            var jsonResult = Json(new
            {
                Vehicles = vehicles,
                Drivers = drivers,
                Rotas = rotas,
                Firms = firms,
                CDoors = cDoors,
                ForexTypes = forexTypes
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult SaveModel(VoyageModel model)
        {
            try
            {
                BusinessResult result = null;
                using (VoyageBO bObj = new VoyageBO())
                {
                    model.PlantId = Convert.ToInt32(Request.Cookies["PlantId"].Value);

                    int userId = Convert.ToInt32(Request.Cookies["UserId"].Value);
                    result = bObj.SaveOrUpdateVoyage(model, userId);
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
        public JsonResult GetWaitingLoads()
        {
            ItemLoadModel[] result = new ItemLoadModel[0];

            using (PlanningBO bObj = new PlanningBO())
            {
                result = bObj.GetWaitingLoads();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetNextVoyageCode(int Id)
        {
            string voyageCode = "";

            using (VoyageBO bObj = new VoyageBO())
            {
                voyageCode = bObj.GetNextVoyageCode(Id);
            }

            var jsonResult = Json(new { Result = !string.IsNullOrEmpty(voyageCode), VoyageCode = voyageCode }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
    }
}