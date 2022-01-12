using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Logistics;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using System;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    public class VehicleCareController : Controller
    {
        // GET: VehicleCare
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetVehicleCareList()
        {
            VehicleCareModel[] result = new VehicleCareModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                result = bObj.GetVehicleCareList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetSelectables()
        {
            VehicleModel[] vehicles = new VehicleModel[0];
            VehicleCareTypeModel[] vehicleCareTypes = new VehicleCareTypeModel[0];
            FirmModel[] fims = new FirmModel[0];
            ForexTypeModel[] forexTypes = new ForexTypeModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                vehicles = bObj.GetVehicleList();
                vehicleCareTypes = bObj.GetVehicleCareTypeList();
                fims = bObj.GetFirmList();
                forexTypes = bObj.GetForexTypeList();
            }

            var jsonResult = Json(new
            {
                Vehicles = vehicles,
                VehicleCareTypes = vehicleCareTypes,
                Firms = fims,
                ForexTypes = forexTypes
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            VehicleCareModel model = null;
            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                model = bObj.GetVehicleCare(rid);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteModel(int rid)
        {
            try
            {
                BusinessResult result = null;
                using (DefinitionsBO bObj = new DefinitionsBO())
                {
                    result = bObj.DeleteVehicleCare(rid);
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
        public JsonResult SaveModel(VehicleCareModel model)
        {
            try
            {
                BusinessResult result = null;
                using (DefinitionsBO bObj = new DefinitionsBO())
                {
                    result = bObj.SaveOrUpdateVehicleCare(model);
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

    }
}