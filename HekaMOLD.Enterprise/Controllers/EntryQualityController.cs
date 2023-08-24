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
    public class EntryQualityController : Controller
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

        public ActionResult PrintEntryPlan()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetPlanList()
        {
            EntryQualityPlanModel[] result = new EntryQualityPlanModel[0];

            using (QualityBO bObj = new QualityBO())
            {
                result = bObj.GetEntryPlanList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetEntryPlanView()
        {
            EntryQualityPlanModel[] result = new EntryQualityPlanModel[0];

            using (QualityBO bObj = new QualityBO())
            {
                result = bObj.GetEntryPlanView();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindPlanModel(int rid)
        {
            EntryQualityPlanModel model = null;
            using (QualityBO bObj = new QualityBO())
            {
                model = bObj.GetEntryPlan(rid);
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
                    result = bObj.DeleteEntryPlan(rid);
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
        public JsonResult SavePlanModel(EntryQualityPlanModel model)
        {
            try
            {
                BusinessResult result = null;
                using (QualityBO bObj = new QualityBO())
                {
                    result = bObj.SaveOrUpdateEntryPlan(model);
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
            FirmModel[] firms = new FirmModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                items = bObj.GetItemList();
                firms = bObj.GetFirmList();
            }

            var jsonResult = Json(new
            {
                Items = items,
                Firms = firms,
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetPlanFormList(string dt1, string dt2)
        {
            EntryQualityDataModel[] result = new EntryQualityDataModel[0];

            using (QualityBO bObj = new QualityBO())
            {
                result = bObj.GetEntryFormList(dt1, dt2);
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindPlanFormModel(int rid, int? sid)
        {
            EntryQualityDataModel model = null;
            EntryQualityPlanModel[] plans = null;
            using (QualityBO bObj = new QualityBO())
            {
                model = bObj.GetEntryForm(rid);
                plans = bObj.GetEntryPlanList();

                if (model != null && (model.ItemId != null || (sid ?? 0) > 0))
                {
                    int itemId = (sid ?? 0) > 0 ? sid.Value : model.ItemId.Value;
                    using (DefinitionsBO defBO = new DefinitionsBO())
                    {
                        var itemModel = defBO.GetItem(itemId);
                        if (itemModel.ItemGroupId > 0)
                        {
                            var groupPlans = bObj.GetEntryPlanListByItemGroup(itemModel.ItemGroupId.Value);
                            if (groupPlans.Length > 0)
                                plans = groupPlans;
                        }
                    }
                }
            }

            return Json(new { Model = model, Plans = plans }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeletePlanFormModel(int rid)
        {
            try
            {
                BusinessResult result = null;
                using (QualityBO bObj = new QualityBO())
                {
                    result = bObj.DeleteEntryForm(rid);
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
        public JsonResult SavePlanFormModel(EntryQualityDataModel model)
        {
            try
            {
                BusinessResult result = null;
                using (QualityBO bObj = new QualityBO())
                {
                    result = bObj.SaveOrUpdateEntryForm(model);
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