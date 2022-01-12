using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    public class WeavingDraftController : Controller
    {
        // GET: WeavingDraft
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetWeavingDraftList()
        {
            WeavingDraftModel[] result = new WeavingDraftModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                result = bObj.GetWeavingDraftList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            WeavingDraftModel model = null;
            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                model = bObj.GetWeavingDraft(rid);
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
                    result = bObj.DeleteWeavingDraft(rid);
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
        public JsonResult SaveModel(WeavingDraftModel model)
        {
            try
            {
                BusinessResult result = null;
                using (DefinitionsBO bObj = new DefinitionsBO())
                {
                    result = bObj.SaveOrUpdateWeavingDraft(model);
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
            MachineBreedModel[] machineBreeds = new MachineBreedModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                machineBreeds = bObj.GetMachineBreedList();
            }

            var jsonResult = Json(new
            {
                MachineBreeds = machineBreeds,
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetNextWeavingDraftCode(string Code)
        {
            string receiptNo = "";

            using (RequestBO bObj = new RequestBO())
            {
                receiptNo = bObj.GetNextWeavingDraftCode(Code);
            }

            var jsonResult = Json(new { Result = !string.IsNullOrEmpty(receiptNo), ReceiptNo = receiptNo }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
    }
}