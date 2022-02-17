using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using System;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    public class YarnRecipeController : Controller
    {
        // GET: YarnRecipe
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetYarnRecipeList()
        {
            YarnRecipeModel[] result = new YarnRecipeModel[0];
            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                result = bObj.GetYarnRecipeList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public JsonResult GetSelectables()
        {
            YarnColourModel[] colours = new YarnColourModel[0];
            FirmModel[] firms = new FirmModel[0];
            YarnBreedModel[] yarnBreed = new YarnBreedModel[0];
            ForexTypeModel[] forexTypes = new ForexTypeModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                colours = bObj.GetYarnColourList();
                firms = bObj.GetFirmList();
                yarnBreed = bObj.GetYarnBreedList();
                forexTypes = bObj.GetForexTypeList();
            }

            var jsonResult = Json(new
            {
                Firms = firms,
                Colours = colours,
                YarnBreed = yarnBreed,
                ForexTypes = forexTypes,
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            YarnRecipeModel model = null;
            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                model = bObj.GetYarnRecipe(rid);
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
                    result = bObj.DeleteYarnRecipe(rid);
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
        public JsonResult SaveModel(YarnRecipeModel model)
        {
            try
            {
                BusinessResult result = null;
                using (DefinitionsBO bObj = new DefinitionsBO())
                {
                    result = bObj.SaveOrUpdateYarnRecipe(model);
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