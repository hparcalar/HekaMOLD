using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases.Core.Base;
using HekaMOLD.Enterprise.Controllers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    [UserAuthFilter]
    public class SystemSettingsController : Controller
    {
        // GET: SystemSettings
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetSettingsList()
        {
            SystemParameterModel[] result = new SystemParameterModel[0];

            using (CoreSystemBO bObj = new CoreSystemBO())
            {
                var plantId = Convert.ToInt32(Request.Cookies["PlantId"].Value);
                result = bObj.GetAllParameters(plantId);
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult SaveSettings(SystemParameterModel[] model)
        {
            BusinessResult result = new BusinessResult();

            using (CoreSystemBO bObj = new CoreSystemBO())
            {
                result = bObj.SetParameters(model);
            }

            return Json(new { Status = result.Result ? 1 : 0, ErrorMessage = result.ErrorMessage });
        }
    }
}