using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Logistics;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using System;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    public class VehicleController : Controller
    {
        // GET: Vehicle
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
            VehicleTypeModel[] vehicleTypes = new VehicleTypeModel[0];
            ForexTypeModel[] forexTypes = new ForexTypeModel[0];
            FirmModel[] firms = new FirmModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                vehicleTypes = bObj.GetVehicleTypeList();
                forexTypes = bObj.GetForexTypeList();
                firms = bObj.GetFirmList();
            }

            var jsonResult = Json(new
            {
                VehicleTypes = vehicleTypes,
                ForexTypes = forexTypes,
                Firms = firms
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        
        [HttpGet]
        public JsonResult GetVehicleCanBePlanedList()
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
        public JsonResult GetVehicleList()
        {
            VehicleModel[] result = new VehicleModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                result = bObj.GetVehicleList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetVehicleTowingList()
        {
            VehicleModel[] result = new VehicleModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                result = bObj.GetVehicleTowingList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetVehicleTrailerList()
        {
            VehicleModel[] result = new VehicleModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                result = bObj.GetVehicleTrailerList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            VehicleModel model = null;
            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                model = bObj.GetVehicle(rid);
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
                    result = bObj.DeleteVehicle(rid);
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
        public JsonResult SaveModel(VehicleModel model)
        {
            try
            {
                BusinessResult result = null;
                using (DefinitionsBO bObj = new DefinitionsBO())
                {
                    if (!string.IsNullOrEmpty(model.ContractStartDateStr))     
                        model.ContractStartDate = DateTime.ParseExact(model.ContractStartDateStr, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
                    if (!string.IsNullOrEmpty(model.ContractEndDateStr))
                        model.ContractEndDate = DateTime.ParseExact(model.ContractEndDateStr, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
                    result = bObj.SaveOrUpdateVehicle(model);
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