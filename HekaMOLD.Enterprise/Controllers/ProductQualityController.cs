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

        #region DATA WORKS
        public ActionResult IndexData()
        {
            return View();
        }

        public ActionResult ListData()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetSelectablesOfData()
        {
            ItemModel[] items = new ItemModel[0];
            MachineModel[] machines = new MachineModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                items = bObj.GetItemListJustNames();
                machines = bObj.GetMachineList();
            }

            var jsonResult = Json(new
            {
                Items = items,
                Machines = machines,
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetPlanFormList()
        {
            ProductQualityDataModel[] result = new ProductQualityDataModel[0];

            using (QualityBO bObj = new QualityBO())
            {
                result = bObj.GetProductFormList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindPlanFormModel(int rid)
        {
            ProductQualityDataModel model = null;
            ProductQualityPlanModel[] plans = null;
            using (QualityBO bObj = new QualityBO())
            {
                model = bObj.GetProductForm(rid);
                plans = bObj.GetProductPlanList();
            }

            return Json(new { Model=model, Plans = plans }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeletePlanFormModel(int rid)
        {
            try
            {
                BusinessResult result = null;
                using (QualityBO bObj = new QualityBO())
                {
                    result = bObj.DeleteProductForm(rid);
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
        public JsonResult SavePlanFormModel(ProductQualityDataModel model)
        {
            try
            {
                BusinessResult result = null;
                using (QualityBO bObj = new QualityBO())
                {
                    result = bObj.SaveOrUpdateProductForm(model);
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

        #region SERIAL APPROVAL
        public ActionResult SerialApproval()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ApproveSerials(WorkOrderSerialModel[] model)
        {
            BusinessResult result = null;

            using (QualityBO bObj = new QualityBO())
            {
                result = bObj.ApproveSerials(model);
            }

            return Json(new { Status = result.Result ? 1 : 0, ErrorMessage = result.ErrorMessage });
        }

        [HttpPost]
        public JsonResult DenySerials(WorkOrderSerialModel[] model)
        {
            BusinessResult result = null;

            using (QualityBO bObj = new QualityBO())
            {
                result = bObj.DenySerials(model);
            }

            return Json(new { Status = result.Result ? 1 : 0, ErrorMessage = result.ErrorMessage });
        }

        [HttpPost]
        public JsonResult WaitSerials(WorkOrderSerialModel[] model)
        {
            BusinessResult result = null;

            using (QualityBO bObj = new QualityBO())
            {
                result = bObj.WaitSerials(model);
            }

            return Json(new { Status = result.Result ? 1 : 0, ErrorMessage = result.ErrorMessage });
        }
        #endregion
    }
}