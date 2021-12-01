using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Summary;
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
    public class WarehouseController : Controller
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
        public JsonResult GetWarehouseList()
        {
            WarehouseModel[] result = new WarehouseModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                result = bObj.GetWarehouseList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            WarehouseModel model = null;
            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                model = bObj.GetWarehouse(rid);
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
                    result = bObj.DeleteWarehouse(rid);
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
        public JsonResult SaveModel(WarehouseModel model)
        {
            try
            {
                BusinessResult result = null;
                using (DefinitionsBO bObj = new DefinitionsBO())
                {
                    result = bObj.SaveOrUpdateWarehouse(model);
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

        #region WAREHOUSE STATES
        public ActionResult States()
        {
            return View();
        }

        public ActionResult FinishedProducts()
        {
            return View();
        }

        public ActionResult FinishedProductState()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetStatesData(string warehouseList)
        {
            ItemStateModel[] data = new ItemStateModel[0];

            try
            {
                int[] warehouseIds = warehouseList.Split(',').Select(d => Convert.ToInt32(d)).ToArray();

                using (ReportingBO bObj = new ReportingBO())
                {
                    data = bObj.GetItemStates(warehouseIds);
                }
            }
            catch (Exception)
            {

            }

            var jsonResponse = Json(data, JsonRequestBehavior.AllowGet);
            jsonResponse.MaxJsonLength = int.MaxValue;
            return jsonResponse;
        }

        [HttpGet]
        public JsonResult GetFinishedProducts()
        {
            ItemStateModel[] data = new ItemStateModel[0];

            try
            {
                int pWrId = 0;

                using (DefinitionsBO bObj = new DefinitionsBO())
                {
                    var wrModel = bObj.GetProductWarehouse();
                    if (wrModel != null && wrModel.Id > 0)
                        pWrId = wrModel.Id;
                }

                using (ReportingBO bObj = new ReportingBO())
                {
                    data = bObj.GetItemStates(new int[] { pWrId });
                }
            }
            catch (Exception)
            {

            }

            var jsonResponse = Json(data, JsonRequestBehavior.AllowGet);
            jsonResponse.MaxJsonLength = int.MaxValue;
            return jsonResponse;
        }

        [HttpGet]
        public JsonResult GetFinishedProductState(string dt1, string dt2)
        {
            ItemStateModel[] data = new ItemStateModel[0];

            try
            {
                int pWrId = 0;

                using (DefinitionsBO bObj = new DefinitionsBO())
                {
                    var wrModel = bObj.GetProductWarehouse();
                    if (wrModel != null && wrModel.Id > 0)
                        pWrId = wrModel.Id;
                }

                using (ReportingBO bObj = new ReportingBO())
                {
                    data = bObj.GetItemStates(new int[] { pWrId }, new Business.Models.Filters.BasicRangeFilter { 
                        StartDate = dt1,
                        EndDate = dt2,
                    });
                }
            }
            catch (Exception)
            {

            }

            var jsonResponse = Json(data, JsonRequestBehavior.AllowGet);
            jsonResponse.MaxJsonLength = int.MaxValue;
            return jsonResponse;
        }
        #endregion
    }
}