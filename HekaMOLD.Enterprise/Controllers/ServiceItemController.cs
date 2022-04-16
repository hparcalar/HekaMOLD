using HekaMOLD.Business.Models.DataTransfer.Logistics;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using System;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    public class ServiceItemController : Controller
    {
        // GET: ServicesItem
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetServiceItemList()
        {
            ServiceItemModel[] result = new ServiceItemModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                result = bObj.GetServiceItemList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            ServiceItemModel model = null;
            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                model = bObj.GetServiceItem(rid);
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
                    result = bObj.DeleteServiceItem(rid);
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
        public JsonResult SaveModel(ServiceItemModel model)
        {
            try
            {
                BusinessResult result = null;
                using (DefinitionsBO bObj = new DefinitionsBO())
                {
                    result = bObj.SaveOrUpdateServiceItem(model);
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