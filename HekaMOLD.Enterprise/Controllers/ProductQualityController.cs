using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.DataTransfer.Quality;
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
    public class ProductQualityController : Controller
    {
        #region PLAN WORKS
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult PrintProductPlan()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetPlanList()
        {
            ProductQualityPlanModel[] result = new ProductQualityPlanModel[0];

            using (QualityBO bObj = new QualityBO())
            {
                result = bObj.GetProductPlanList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetProductPlanView()
        {
            ProductQualityPlanModel[] result = new ProductQualityPlanModel[0];

            using (QualityBO bObj = new QualityBO())
            {
                result = bObj.GetProductPlanView();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindPlanModel(int rid)
        {
            ProductQualityPlanModel model = null;
            using (QualityBO bObj = new QualityBO())
            {
                model = bObj.GetProductPlan(rid);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeletePlanModel(int rid)
        {
            try
            {
                BusinessResult result = null;
                using (QualityBO bObj = new QualityBO())
                {
                    result = bObj.DeleteProductPlan(rid);
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
        public JsonResult SavePlanModel(ProductQualityPlanModel model)
        {
            try
            {
                BusinessResult result = null;
                using (QualityBO bObj = new QualityBO())
                {
                    result = bObj.SaveOrUpdateProductPlan(model);
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
        #endregion
    }
}