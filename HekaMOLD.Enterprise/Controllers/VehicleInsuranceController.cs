using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Logistics;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using System;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    public class VehicleInsuranceController : Controller
    {
        // GET: VehicleInsurance
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetVehicleInsuranceList()
        {
            VehicleInsuranceModel[] result = new VehicleInsuranceModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                result = bObj.GetVehicleInsuranceList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetSelectables()
        {
            VehicleModel[] vehicles = new VehicleModel[0];
            VehicleInsuranceTypeModel[] vehicleInsuranceTypes = new VehicleInsuranceTypeModel[0];
            FirmModel[] fims = new FirmModel[0];
            ForexTypeModel[] forexTypes = new ForexTypeModel[0];


            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                vehicles = bObj.GetVehicleList();
                vehicleInsuranceTypes = bObj.GetVehicleInsuranceTypeList();
                fims = bObj.GetFirmList();
                forexTypes = bObj.GetForexTypeList();
            }

            var jsonResult = Json(new
            {
                Vehicles = vehicles,
                VehicleInsuranceTypes = vehicleInsuranceTypes,
                Firms= fims,
                ForexTypes = forexTypes
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            VehicleInsuranceModel model = null;
            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                model = bObj.GetVehicleInsurance(rid);
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
                    result = bObj.DeleteVehicleInsurance(rid);
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
        public JsonResult SaveModel(VehicleInsuranceModel model)
        {
            try
            {
                BusinessResult result = null;
                using (DefinitionsBO bObj = new DefinitionsBO())
                {
                    if (!string.IsNullOrEmpty(model.StartDateStr))
                        model.StartDate = DateTime.ParseExact(model.StartDateStr, "dd.MM.yyyy",
                            System.Globalization.CultureInfo.GetCultureInfo("tr"));

                    if (!string.IsNullOrEmpty(model.EndDateStr))
                        model.EndDate = DateTime.ParseExact(model.EndDateStr, "dd.MM.yyyy",
                            System.Globalization.CultureInfo.GetCultureInfo("tr"));

                    result = bObj.SaveOrUpdateVehicleInsurance(model);
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