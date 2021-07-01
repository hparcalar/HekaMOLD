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
    public class WorkOrderController : Controller
    {
        // GET: Firm
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetWorkOrderDetailList()
        {
            WorkOrderDetailModel[] result = new WorkOrderDetailModel[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                result = bObj.GetWorkOrderDetailList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        /// <summary>
        /// TO-DO
        /// </summary>
        /// <param name="rid"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            WorkOrderModel model = null;
            //using (DefinitionsBO bObj = new DefinitionsBO())
            //{
            //    model = bObj.GetFirm(rid);
            //}

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// TO-DO
        /// </summary>
        /// <param name="rid"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteModel(int rid)
        {
            try
            {
                BusinessResult result = null;
                //using (DefinitionsBO bObj = new DefinitionsBO())
                //{
                //    result = bObj.DeleteFirm(rid);
                //}

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

        /// <summary>
        /// TO-DO
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveModel(FirmModel model)
        {
            try
            {
                BusinessResult result = null;
                //using (DefinitionsBO bObj = new DefinitionsBO())
                //{
                //    result = bObj.SaveOrUpdateFirm(model);
                //}

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