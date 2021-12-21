using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    public class ContractWorksController : Controller
    {
        // GET: ContractWorks
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetSelectables()
        {
            WarehouseModel[] warehouses = new WarehouseModel[0];
            WorkOrderCategoryModel[] workCategories = new WorkOrderCategoryModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                warehouses = bObj.GetWarehouseList();
                workCategories = bObj.GetWorkOrderCategoryList();
            }

            var jsonResp = Json(new { Warehouses = warehouses,
                WorkCategories = workCategories }, JsonRequestBehavior.AllowGet);
            jsonResp.MaxJsonLength = int.MaxValue;
            return jsonResp;
        }

        [HttpGet]
        public JsonResult BindModel(int? workOrderCategory)
        {
            WorkOrderDetailModel[] workOrders = new WorkOrderDetailModel[0];

            using (ContractWorksBO bObj = new ContractWorksBO())
            {
                workOrders = bObj.GetOuterWorkList(workOrderCategory);
            }

            var jsonResp = Json(new { WorkOrders = workOrders }, JsonRequestBehavior.AllowGet);
            jsonResp.MaxJsonLength = int.MaxValue;
            return jsonResp;
        }

        [HttpGet]
        public JsonResult GetMovements(int workOrderDetailId)
        {
            ContractWorkFlowModel[] data = new ContractWorkFlowModel[0];

            using (ContractWorksBO bObj = new ContractWorksBO())
            {
                data = bObj.GetFlowList(workOrderDetailId);
            }

            var jsonResp = Json(data, JsonRequestBehavior.AllowGet);
            jsonResp.MaxJsonLength = int.MaxValue;
            return jsonResp;
        }
    }
}