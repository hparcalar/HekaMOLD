using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using HekaMOLD.Business.UseCases.Core;
using System;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    public class YarnColourController : Controller
    {
        // GET: YarnColour
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetYarnColourList()
        {
            YarnColourModel[] result = new YarnColourModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                result = bObj.GetYarnColourList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            YarnColourModel model = null;
            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                model = bObj.GetYarnColour(rid);
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
                    result = bObj.DeleteYarnColour(rid);
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
        public JsonResult SaveModel(YarnColourModel model)
        {
            try
            {
                BusinessResult result = null;
                using (DefinitionsBO bObj = new DefinitionsBO())
                {
                    result = bObj.SaveOrUpdateYarnColour(model);
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
        [HttpGet]
        public JsonResult GetSelectables()
        {
            YarnColourGroupModel[] colourGroups = new YarnColourGroupModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                colourGroups = bObj.GetYarnColourGroupList();
            }

            var jsonResult = Json(new
            {
                ColourGroups = colourGroups,
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetNextYarnColourNo()
        {
            string receiptNo = "";

            using (RequestBO bObj = new RequestBO())
            {
                receiptNo = bObj.GetNextYarnColourNo();
            }

            var jsonResult = Json(new { Result = !string.IsNullOrEmpty(receiptNo), ReceiptNo = receiptNo }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        [HttpGet]
        public JsonResult GetYarnColorCode(string strParam)
        {
           string colorCode = "";

            using (RequestBO bObj = new RequestBO())
            {
                colorCode = bObj.GetYarnColorCode(strParam);
            }
            var jsonResult = Json(new { Result = !string.IsNullOrEmpty(colorCode), ColorCode = colorCode }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

    }
}