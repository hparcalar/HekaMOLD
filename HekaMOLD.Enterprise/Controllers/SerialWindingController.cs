using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.DataTransfer.Quality;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using HekaMOLD.Enterprise.Controllers.Attributes;
using HekaMOLD.Enterprise.Controllers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    [UserAuthFilter]
    public class SerialWindingController : Controller
    {
        // GET: SerialWinding
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        [HttpGet]
        [FreeAction]
        public JsonResult GetOpenWorkOrderList()
        {
            WorkOrderDetailModel[] result = new WorkOrderDetailModel[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                result = bObj.GetOpenWorkOrderDetailList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            SerialQualityWindingModel[] model = new SerialQualityWindingModel[0];
            using (QualityBO bObj = new QualityBO())
            {
                model = bObj.GetWindingList(rid);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetFaultsOfSerial(int rid)
        {
            SerialQualityWindingFaultModel[] model = new SerialQualityWindingFaultModel[0];
            using (QualityBO bObj = new QualityBO())
            {
                model = bObj.GetFaultsOfWinding(rid);
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
                    result = bObj.DeleteDye(rid);
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
        public JsonResult DeleteFault(int rid)
        {
            try
            {
                BusinessResult result = null;
                using (QualityBO bObj = new QualityBO())
                {
                    result = bObj.RemoveFault(rid);
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
        public JsonResult StartWinding(int workOrderDetailId)
        {
            try
            {
                BusinessResult result = null;
                using (QualityBO bObj = new QualityBO())
                {

                    result = bObj.StartWinding(workOrderDetailId, "",
                        Convert.ToInt32(Request.Cookies["UserId"].Value), null);
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

        [HttpPost]
        public JsonResult EndWinding(int windingId, decimal? meter, decimal? quantity)
        {
            try
            {
                BusinessResult result = null;
                using (QualityBO bObj = new QualityBO())
                {

                    result = bObj.EndWinding(windingId, meter, quantity);
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

        [HttpPost]
        public JsonResult AddFaultToWinding(int windingId, int faultTypeId, decimal? meter, decimal? quantity, bool isDotted)
        {
            try
            {
                BusinessResult result = null;
                using (QualityBO bObj = new QualityBO())
                {

                    result = bObj.AddFaultToWinding(windingId, faultTypeId, meter, quantity, isDotted,
                        Convert.ToInt32(Request.Cookies["UserId"].Value));
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

        [HttpPost]
        public JsonResult EndFaultAtWinding(int windingId, int faultTypeId, decimal? meter, decimal? quantity)
        {
            try
            {
                BusinessResult result = null;
                using (QualityBO bObj = new QualityBO())
                {
                    result = bObj.EndFaultAtWinding(windingId, faultTypeId, meter, quantity);
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