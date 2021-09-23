using HekaMOLD.Business.Models.DataTransfer.Core;
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
    public class DocumentController : Controller
    {
        // GET: ItemCategory
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Preview()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetDocumentList()
        {
            UsageDocumentModel[] result = new UsageDocumentModel[0];

            using (DocumentBO bObj = new DocumentBO())
            {
                result = bObj.GetDocumentList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            UsageDocumentModel model = null;
            using (DocumentBO bObj = new DocumentBO())
            {
                model = bObj.GetDocument(rid);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteModel(int rid)
        {
            try
            {
                BusinessResult result = null;
                using (DocumentBO bObj = new DocumentBO())
                {
                    result = bObj.DeleteDocument(rid);
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
        public JsonResult SaveModel(UsageDocumentModel model)
        {
            try
            {
                BusinessResult result = null;
                using (DocumentBO bObj = new DocumentBO())
                {
                    result = bObj.SaveOrUpdateDocument(model);
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