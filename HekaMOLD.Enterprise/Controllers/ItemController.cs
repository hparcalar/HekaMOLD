using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Receipt;
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
    public class ItemController : Controller
    {
        // GET: Item
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Extract()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetItemList()
        {
            ItemModel[] result = new ItemModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                result = bObj.GetItemList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetItemExtract(int itemId)
        {
            ItemReceiptDetailModel[] result = new ItemReceiptDetailModel[0];

            using (ReceiptBO bObj = new ReceiptBO())
            {
                result = bObj.GetItemExtract(itemId);
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetSelectables()
        {
            ItemCategoryModel[] categories = new ItemCategoryModel[0];
            ItemGroupModel[] groups = new ItemGroupModel[0];
            FirmModel[] firms = new FirmModel[0];
            UnitTypeModel[] units = new UnitTypeModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                categories = bObj.GetItemCategoryList();
                groups = bObj.GetItemGroupList();
                firms = bObj.GetFirmList();
                units = bObj.GetUnitTypeList();
            }

            var jsonResult = Json(new { Categories=categories, 
                Groups=groups, Firms=firms, Units=units }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetProperWarehouses(int itemType, int itemId)
        {
            ItemWarehouseModel[] warehouses = new ItemWarehouseModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                warehouses = bObj.GetProperWarehouses((ItemType)itemType, itemId);
            }

            var jsonResult = Json(warehouses, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            ItemModel model = null;
            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                model = bObj.GetItem(rid);
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
                    result = bObj.DeleteItem(rid);
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
                    result = bObj.SaveOrUpdateItem(model);
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