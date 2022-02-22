using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Logistics;
using HekaMOLD.Business.UseCases;
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

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                vehicles = bObj.GetVehicleList();

            }
            using (UsersBO bObj = new UsersBO())
            {
                drivers = bObj.GetDriverList();

            }

            var jsonResult = Json(new
            {
                Vehicles = vehicles,
                Drivers = drivers
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
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