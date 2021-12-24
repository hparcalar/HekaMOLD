using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.MoldTrace;
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
    public class MoldController : Controller
    {
        // GET: Warehouse
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetMoldList()
        {
            MoldModel[] result = new MoldModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                result = bObj.GetMoldList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetMoldMovementHistory(int moldId)
        {
            MoldMoveHistory[] result = new MoldMoveHistory[0];

            using (MoldBO bObj = new MoldBO())
            {
                result = bObj.GetMoldMovementHistory(moldId);
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            MoldModel model = null;
            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                model = bObj.GetMold(rid);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSelectables()
        {
            ItemModel[] items = new ItemModel[0];
            FirmModel[] firms = new FirmModel[0];
            WarehouseModel[] warehouses = new WarehouseModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                items = bObj.GetItemList();
                firms = bObj.GetFirmList().OrderBy(d => d.FirmName).ToArray();
                warehouses = bObj.GetWarehouseList();
            }

            var jsonResult = Json(new
            {
                Items = items,
                Firms = firms,
                Warehouses = warehouses,
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetProductsOfMold(int moldId)
        {
            ItemModel[] data = new ItemModel[0];

            using (MoldBO bObj = new MoldBO())
            {
                data = bObj.GetItemsOfMold(moldId);
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult DeleteModel(int rid)
        {
            try
            {
                BusinessResult result = null;
                using (DefinitionsBO bObj = new DefinitionsBO())
                {
                    result = bObj.DeleteMold(rid);
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
        public JsonResult ChangeMoldStatus(int rid, int status)
        {
            try
            {
                BusinessResult result = null;

                int userId = Convert.ToInt32(Request.Cookies["UserId"].Value);

                using (MoldBO bObj = new MoldBO())
                {
                    result = bObj.SetMoldStatus(rid, (MoldStatus)status, userId);
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
        public JsonResult SendToRevision(int rid, int firmId, string moveDate)
        {
            try
            {
                BusinessResult result = null;

                int userId = Convert.ToInt32(Request.Cookies["UserId"].Value);

                using (MoldBO bObj = new MoldBO())
                {
                    DateTime dtMove = DateTime.Now;
                    if (!string.IsNullOrEmpty(moveDate))
                        dtMove = DateTime.ParseExact(moveDate, "yyyy-MM-dd",
                            System.Globalization.CultureInfo.GetCultureInfo("tr"));

                    result = bObj.SendToRevision(rid, firmId, userId, dtMove);
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
        public JsonResult BackFromRevision(int rid, string moveDate)
        {
            try
            {
                BusinessResult result = null;

                int userId = Convert.ToInt32(Request.Cookies["UserId"].Value);

                using (MoldBO bObj = new MoldBO())
                {
                    DateTime dtMove = DateTime.Now;
                    if (!string.IsNullOrEmpty(moveDate))
                        dtMove = DateTime.ParseExact(moveDate, "yyyy-MM-dd",
                            System.Globalization.CultureInfo.GetCultureInfo("tr"));

                    result = bObj.BackFromRevision(rid, userId, dtMove);
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
        public JsonResult SaveModel(MoldModel model)
        {
            try
            {
                BusinessResult result = null;
                using (DefinitionsBO bObj = new DefinitionsBO())
                {
                    model.PlantId = Convert.ToInt32(Request.Cookies["PlantId"].Value);
                    result = bObj.SaveOrUpdateMold(model);
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