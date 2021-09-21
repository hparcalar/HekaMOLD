using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using HekaMOLD.Enterprise.Controllers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    [UserAuthFilter]
    public class ShiftController : Controller
    {
        // GET: Warehouse
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetShiftList()
        {
            ShiftModel[] result = new ShiftModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                result = bObj.GetShiftList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            ShiftModel model = null;
            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                model = bObj.GetShift(rid);
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
                    result = bObj.DeleteShift(rid);
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
        public JsonResult SaveModel(ShiftModel model)
        {
            try
            {
                BusinessResult result = null;
                using (DefinitionsBO bObj = new DefinitionsBO())
                {
                    if (!string.IsNullOrEmpty(model.StartTimeStr))
                    {
                        var startData = model.StartTimeStr.Split(':');
                        model.StartTime = TimeSpan.FromMinutes(Convert.ToInt32(startData[0]) * 60 + Convert.ToInt32(startData[1]));
                    }

                    if (!string.IsNullOrEmpty(model.EndTimeStr))
                    {
                        var endData = model.EndTimeStr.Split(':');
                        model.EndTime = TimeSpan.FromMinutes(Convert.ToInt32(endData[0]) * 60 + Convert.ToInt32(endData[1]));
                    }

                    result = bObj.SaveOrUpdateShift(model);
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