using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using HekaMOLD.Enterprise.Controllers.Attributes;

namespace HekaMOLD.Enterprise.Controllers
{
    public class MaintenanceController : Controller
    {
        #region INCIDENT CATEGORY
        public ActionResult IncidentCategory()
        {
            return View();
        }

        public ActionResult ListIncidentCategory()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetIncidentCategoryList()
        {
            IncidentCategoryModel[] result = new IncidentCategoryModel[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                result = bObj.GetIncidentCategoryList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindIncidentCategory(int rid)
        {
            IncidentCategoryModel model = null;
            using (ProductionBO bObj = new ProductionBO())
            {
                model = bObj.GetIncidentCategory(rid);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteIncidentCategory(int rid)
        {
            try
            {
                BusinessResult result = null;
                using (ProductionBO bObj = new ProductionBO())
                {
                    result = bObj.DeleteIncidentCategory(rid);
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
        public JsonResult SaveIncidentCategory(IncidentCategoryModel model)
        {
            try
            {
                BusinessResult result = null;
                using (ProductionBO bObj = new ProductionBO())
                {
                    result = bObj.SaveOrUpdateIncidentCategory(model);
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

        #region POSTURE CATEGORY
        public ActionResult PostureCategory()
        {
            return View();
        }

        public ActionResult ListPostureCategory()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetPostureCategoryList()
        {
            PostureCategoryModel[] result = new PostureCategoryModel[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                result = bObj.GetPostureCategoryList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindPostureCategory(int rid)
        {
            PostureCategoryModel model = null;
            using (ProductionBO bObj = new ProductionBO())
            {
                model = bObj.GetPostureCategory(rid);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeletePostureCategory(int rid)
        {
            try
            {
                BusinessResult result = null;
                using (ProductionBO bObj = new ProductionBO())
                {
                    result = bObj.DeletePostureCategory(rid);
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
        public JsonResult SavePostureCategory(PostureCategoryModel model)
        {
            try
            {
                BusinessResult result = null;
                using (ProductionBO bObj = new ProductionBO())
                {
                    result = bObj.SaveOrUpdatePostureCategory(model);
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

        #region INCIDENT
        public ActionResult Incident()
        {
            return View();
        }

        public ActionResult ListIncident()
        {
            return View();
        }

        [HttpGet]
        [FreeAction]
        public JsonResult GetIncidentList()
        {
            IncidentModel[] result = new IncidentModel[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                result = bObj.GetIncidentList(new Business.Models.Filters.BasicRangeFilter());
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindIncident(int rid)
        {
            IncidentModel model = null;
            using (ProductionBO bObj = new ProductionBO())
            {
                model = bObj.GetIncident(rid);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteIncident(int rid)
        {
            try
            {
                BusinessResult result = null;
                using (ProductionBO bObj = new ProductionBO())
                {
                    result = bObj.DeleteIncident(rid);
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
        public JsonResult SaveIncident(IncidentModel model)
        {
            try
            {
                BusinessResult result = null;
                using (ProductionBO bObj = new ProductionBO())
                {
                    result = bObj.SaveOrUpdateIncident(model);
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

        #region POSTURE
        public ActionResult Posture()
        {
            return View();
        }

        public ActionResult ListPosture()
        {
            return View();
        }

        [HttpGet]
        [FreeAction]
        public JsonResult GetPostureList()
        {
            ProductionPostureModel[] result = new ProductionPostureModel[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                result = bObj.GetPostureList(new Business.Models.Filters.BasicRangeFilter());
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindPosture(int rid)
        {
            ProductionPostureModel model = null;
            using (ProductionBO bObj = new ProductionBO())
            {
                model = bObj.GetPosture(rid);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeletePosture(int rid)
        {
            try
            {
                BusinessResult result = null;
                using (ProductionBO bObj = new ProductionBO())
                {
                    result = bObj.DeletePosture(rid);
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
        public JsonResult SavePosture(ProductionPostureModel model)
        {
            try
            {
                BusinessResult result = null;
                using (ProductionBO bObj = new ProductionBO())
                {
                    result = bObj.SaveOrUpdatePosture(model);
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

        #region SUMMARY DATA
        [FreeAction]
        public JsonResult GetIncidentsOfMachine(int machineId, string startDate, string endDate)
        {
            IncidentModel[] data = new IncidentModel[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                data = bObj.GetIncidentList(new Business.Models.Filters.BasicRangeFilter
                {
                    MachineId = machineId,
                    StartDate = startDate,
                    EndDate = endDate,
                });
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [FreeAction]
        public JsonResult GetPosturesOfMachine(int machineId, string startDate, string endDate)
        {
            ProductionPostureModel[] data = new ProductionPostureModel[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                data = bObj.GetPostureList(new Business.Models.Filters.BasicRangeFilter
                {
                    MachineId = machineId,
                    StartDate = startDate,
                    EndDate = endDate,
                });
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region REPORTS
        public ActionResult ReportMaintenance()
        {
            return View();
        }
        #endregion
    }
}