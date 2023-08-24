using HekaMOLD.Business.Models.Constants;
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
        public JsonResult GetPlanFormList(string dt1, string dt2)
        {
            ProductQualityDataModel[] result = new ProductQualityDataModel[0];

            using (QualityBO bObj = new QualityBO())
            {
                result = bObj.GetProductFormList(dt1,dt2);
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

        public ActionResult ScrapList()
        {
            return View();
        }

        public ActionResult ConditionalApproveList()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ApproveSerials(WorkOrderSerialModel[] model)
        {
            BusinessResult result = null;
            int plantId = Convert.ToInt32(Request.Cookies["PlantId"].Value);
            int userId = Convert.ToInt32(Request.Cookies["UserId"].Value);

            using (QualityBO bObj = new QualityBO())
            {
                result = bObj.ApproveSerials(model, plantId, userId);
            }

            //if (result.Result == true)
            //{
            //    using (ProductionBO bObj = new ProductionBO())
            //    {
            //        bObj.CreateNotification(new NotificationModel
            //        {
            //            UserId = null,
            //            NotifyType = (int)NotifyType.ProductPickupComplete,
            //            CreatedDate = DateTime.Now,
            //            Title = "Ürün Teslim Alma Bildirimi",
            //            IsProcessed = false,
            //            Message = string.Format("{0:HH:mm}", DateTime.Now) + ": Ürün teslim alma işlemi yapıldı.",
            //        });
            //    }
            //}

            return Json(new { Status = result.Result ? 1 : 0, ErrorMessage = result.ErrorMessage });
        }

        [HttpPost]
        public JsonResult DenySerials(WorkOrderSerialModel[] model)
        {
            BusinessResult result = null;
            int userId = Convert.ToInt32(Request.Cookies["UserId"].Value);

            using (QualityBO bObj = new QualityBO())
            {
                result = bObj.DenySerials(model, userId);
            }

            return Json(new { Status = result.Result ? 1 : 0, ErrorMessage = result.ErrorMessage });
        }

        [HttpPost]
        public JsonResult WaitSerials(WorkOrderSerialModel[] model)
        {
            BusinessResult result = null;
            int userId = Convert.ToInt32(Request.Cookies["UserId"].Value);

            using (QualityBO bObj = new QualityBO())
            {
                result = bObj.WaitSerials(model, userId);
            }

            return Json(new { Status = result.Result ? 1 : 0, ErrorMessage = result.ErrorMessage });
        }

        [HttpPost]
        public JsonResult SendToWastage(WorkOrderSerialModel[] model, string explanation)
        {
            BusinessResult result = null;
            int userId = Convert.ToInt32(Request.Cookies["UserId"].Value);

            using (QualityBO bObj = new QualityBO())
            {
                result = bObj.SendToWastage(model, userId, explanation);
            }

            return Json(new { Status = result.Result ? 1 : 0, ErrorMessage = result.ErrorMessage });
        }

        [HttpPost]
        public JsonResult ConditionalApprove(WorkOrderSerialModel[] model)
        {
            BusinessResult result = null;
            int plantId = Convert.ToInt32(Request.Cookies["PlantId"].Value);
            int userId = Convert.ToInt32(Request.Cookies["UserId"].Value);

            using (QualityBO bObj = new QualityBO())
            {
                result = bObj.ConditionalApproveSerials(model, plantId, userId);
            }

            return Json(new { Status = result.Result ? 1 : 0, ErrorMessage = result.ErrorMessage });
        }
        [HttpPost]
        public JsonResult DeleteSerials(WorkOrderSerialModel[] model)
        {
            BusinessResult result = null;

            try
            {
                int userId = Convert.ToInt32(Request.Cookies["UserId"].Value);

                using (ProductionBO bObj = new ProductionBO())
                {
                    result = bObj.DeleteSerials(model, userId);
                }
            }
            catch (Exception)
            {

            }

            return Json(new { Status = result.Result ? 1 : 0, ErrorMessage = result.ErrorMessage });
        }
        #endregion

        #region CONDITIONAL APPROVE & SCRAP REPORTS
        [HttpGet]
        public JsonResult GetConditionalApprovedList(string dt1, string dt2)
        {
            WorkOrderSerialModel[] data = new WorkOrderSerialModel[0];

            using (QualityBO bObj = new QualityBO())
            {
                data = bObj.GetConditionalApprovedSerials(dt1,dt2);
            }

            var jsonResp = Json(data, JsonRequestBehavior.AllowGet);
            jsonResp.MaxJsonLength = int.MaxValue;
            return jsonResp;
        }

        [HttpGet]
        public JsonResult GetScrapList(string dt1, string dt2)
        {
            ProductWastageModel[] data = new ProductWastageModel[0];

            using (QualityBO bObj = new QualityBO())
            {
                data = bObj.GetScrapList(new Business.Models.Filters.BasicRangeFilter { }, dt1,dt2);
            }

            var jsonResp = Json(data, JsonRequestBehavior.AllowGet);
            jsonResp.MaxJsonLength = int.MaxValue;
            return jsonResp;
        }
        #endregion
    }
}