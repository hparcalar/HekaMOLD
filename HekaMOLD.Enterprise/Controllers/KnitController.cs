using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using System;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    public class KnitController : Controller
    {
        // GET: Knit
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List()
        {
            return View();
        }
        public ActionResult Variant()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetKnitList()
        {
            ItemModel[] result = new ItemModel[0];
            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                result = bObj.GetKnitList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetSelectables()
        {
            ItemQualityTypeModel[] qualityType = new ItemQualityTypeModel[0];
            WeavingDraftModel[] weavingDrafts = new WeavingDraftModel[0];
            YarnRecipeModel[] yarnRecipes = new YarnRecipeModel[0];
            FirmModel[] firms = new FirmModel[0];
            ItemVariantModel[] itemVariants = new ItemVariantModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                qualityType = bObj.GetItemQualityTypeList();
                weavingDrafts = bObj.GetWeavingDraftList();
                yarnRecipes = bObj.GetYarnRecipeList();
                firms = bObj.GetFirmList();
                itemVariants = bObj.GetItemVariantList();
            }

            var jsonResult = Json(new
            {
                QualityType = qualityType,
                WeavingDrafts = weavingDrafts,
                YarnRecipes = yarnRecipes,
                Firms = firms,
                ItemVariants = itemVariants
            }, JsonRequestBehavior.AllowGet); ;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            ItemModel model = null;
            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                model = bObj.GetKnit(rid);
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
                    result = bObj.DeleteKnit(rid);
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
        public JsonResult SaveModel(ItemModel model)
        {
            try
            {
                BusinessResult result = null;
                using (DefinitionsBO bObj = new DefinitionsBO())
                {
                    model.ItemType = (int)ItemType.Attempt;
                    result = bObj.SaveOrUpdateKnit(model);
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
        public JsonResult CalculateRow(KnitYarnModel model)
        {
            KnitYarnModel result = new KnitYarnModel();

            using (OrdersBO bObj = new OrdersBO())
            {
                //result = bObj.CalculateOrderDetail(model);
            }

            return Json(result);
        }
        [HttpPost]
        public JsonResult creatVariant(ItemVariantModel model)
        {
            try
            {
                BusinessResult result = null;
                using (DefinitionsBO bObj = new DefinitionsBO())
                {
                    model.ItemType = (int)ItemType.Product;
                    model.AttemptNo = "";
                    using (RequestBO rObj = new RequestBO())
                    {
                        model.ItemNo = model.ItemNo + rObj.GetNextVariantNo((int)model.ItemId);
                    }
                    result = bObj.SaveOrUpdateVariant(model);
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
        public JsonResult creatKnit(ItemModel model)
        {
            try
            {
                BusinessResult result = null;
                using (DefinitionsBO bObj = new DefinitionsBO())
                {
                    model.ItemType = (int)ItemType.Product;
                    model.AttemptNo = "";
                    using (RequestBO rObj = new RequestBO())
                    {
                        model.ItemNo = rObj.GetNextKnitNo();
                    }
                    result = bObj.SaveOrUpdateKnit(model);
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
        //[HttpPost]
        //public JsonResult CalculateRow(KnitYarnModel model)
        //{
        //    KnitYarnModel result = new KnitYarnModel();

        //    using (OrdersBO bObj = new OrdersBO())
        //    {
        //        //result = bObj.CalculateOrderDetail(model);
        //    }

        //    return Json(result);
        //}
        [HttpGet]
        public JsonResult GetNextAttemptNo()
        {
            string attemptNo = "";

            using (RequestBO bObj = new RequestBO())
            {
                attemptNo = bObj.GetNextAttemptNo();
            }
            var jsonResult = Json(new { Result = !string.IsNullOrEmpty(attemptNo), AttemptNo = attemptNo }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
    }
}