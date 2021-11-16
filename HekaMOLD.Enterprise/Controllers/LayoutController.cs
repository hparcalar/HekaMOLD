using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HekaMOLD.Business.Models.DataTransfer.Layout;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using HekaMOLD.Enterprise.Controllers.Attributes;
using HekaMOLD.Enterprise.Controllers.Filters;
using System.IO;
using HekaMOLD.Business.Models.DataTransfer.Production;
using Newtonsoft.Json;

namespace HekaMOLD.Enterprise.Controllers
{
    [UserAuthFilter]
    public class LayoutController : Controller
    {
        // GET: Layout
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult BindModel()
        {
            LayoutItemModel[] model = new LayoutItemModel[0];
            MachineModel[] machines = new MachineModel[0];

            using (LayoutBO bObj = new LayoutBO())
            {
                model = bObj.GetLayoutItemList();
                machines = bObj.GetPlaceableMachines();
            }

            var jsonResponse = Json(new { 
                Items = model,
                Machines = machines,
            }, JsonRequestBehavior.AllowGet);
            jsonResponse.MaxJsonLength = int.MaxValue;
            return jsonResponse;
        }

        [HttpPost]
        public JsonResult SaveModel(LayoutItemModel[] model)
        {
            BusinessResult result = null;

            foreach (var item in model)
            {
                item.PlantId = Convert.ToInt32(Request.Cookies["PlantId"].Value);
                using (LayoutBO bObj = new LayoutBO())
                {
                    result = bObj.SaveOrUpdateLayoutItem(item);
                }
            }

            return Json(result);
        }
    }
}