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
    public class ProductRecipeController : Controller
    {
        // GET: ItemCategory
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetNextRecipeNo()
        {
            string receiptNo = "";

            using (RecipeBO bObj = new RecipeBO())
            {
                receiptNo = bObj.GetNextProductRecipeNo();
            }

            var jsonResult = Json(new { Result = !string.IsNullOrEmpty(receiptNo), 
                RecipeNo = receiptNo }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult FindRecipeByProductId(int productId)
        {
            ProductRecipeModel model = null;
            using (RecipeBO bObj = new RecipeBO())
            {
                model = bObj.FindActiveRecipeByProduct(productId);
            }

            var jsonResult = Json(model, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetSelectables()
        {
            ItemModel[] items = new ItemModel[0];
            ItemModel[] products = new ItemModel[0];
            UnitTypeModel[] units = new UnitTypeModel[0];
            WarehouseModel[] warehouses = new WarehouseModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                items = bObj.GetItemList();
                units = bObj.GetUnitTypeList();
                warehouses = bObj.GetWarehouseList();
                products = bObj.GetProductList();
            }

            var jsonResult = Json(new { Items = items,
                Units = units,
                Warehouses = warehouses,
                Products = products
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetRecipeList()
        {
            ProductRecipeModel[] result = new ProductRecipeModel[0];

            using (RecipeBO bObj = new RecipeBO())
            {
                result = bObj.GetActiveProductRecipeList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetRevisionsOfProduct(int productId)
        {
            ProductRecipeModel[] result = new ProductRecipeModel[0];

            using (RecipeBO bObj = new RecipeBO())
            {
                result = bObj.GetRevisionsOfProduct(productId);
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            ProductRecipeModel model = null;
            using (RecipeBO bObj = new RecipeBO())
            {
                model = bObj.GetProductRecipe(rid);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteModel(int rid)
        {
            try
            {
                BusinessResult result = null;
                using (RecipeBO bObj = new RecipeBO())
                {
                    result = bObj.DeleteProductRecipe(rid);
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
        public JsonResult SaveModel(ProductRecipeModel model)
        {
            try
            {
                BusinessResult result = null;
                using (RecipeBO bObj = new RecipeBO())
                {
                    result = bObj.SaveOrUpdateProductRecipe(model);
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