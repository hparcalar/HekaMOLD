using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.PreventiveActions;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using HekaMOLD.Enterprise.Controllers.Attributes;
using HekaMOLD.Enterprise.Controllers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    [UserAuthFilter]
    public class PreventiveActionController : Controller
    {
        // GET: SIOrder
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetNextFormNo()
        {
            string receiptNo = "";

            using (InformationSheetsBO bObj = new InformationSheetsBO())
            {
                receiptNo = bObj.GetNextPreventiveActionFormNo();
            }

            var jsonResult = Json(new { Result = !string.IsNullOrEmpty(receiptNo), ReceiptNo = receiptNo }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetSelectables()
        {
            UserModel[] users = new UserModel[0];

            using (UsersBO bObj = new UsersBO())
            {
                users = bObj.GetUserList();
            }

            var jsonResult = Json(new
            {
                Users = users,
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetFormList()
        {
            PreventiveActionModel[] result = new PreventiveActionModel[0];

            using (InformationSheetsBO bObj = new InformationSheetsBO())
            {
                result = bObj.GetPreventiveActionList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            PreventiveActionModel model = null;
            using (InformationSheetsBO bObj = new InformationSheetsBO())
            {
                model = bObj.GetPreventiveAction(rid);
            }

            var jsonResponse = Json(model, JsonRequestBehavior.AllowGet);
            jsonResponse.MaxJsonLength = int.MaxValue;

            return jsonResponse;
        }

        [HttpPost]
        public JsonResult DeleteModel(int rid)
        {
            try
            {
                BusinessResult result = null;
                using (InformationSheetsBO bObj = new InformationSheetsBO())
                {
                    result = bObj.DeletePreventiveAction(rid);
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
        public JsonResult SaveModel(PreventiveActionModel model)
        {
            try
            {
                BusinessResult result = null;
                using (InformationSheetsBO bObj = new InformationSheetsBO())
                {
                    result = bObj.SaveOrUpdatePreventiveAction(model);
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

        [HttpPost]
        public JsonResult ApproveForm(int rid)
        {
            BusinessResult result = new BusinessResult();

            using (InformationSheetsBO bObj = new InformationSheetsBO())
            {
                result = bObj.ToggleApprovePreventiveForm(rid, Convert.ToInt32(Request.Cookies["UserId"].Value));
            }

            return Json(new { Status = result.Result ? 1 : 0, ErrorMessage = result.ErrorMessage });
        }

        [HttpPost]
        public JsonResult CloseForm(int rid)
        {
            BusinessResult result = new BusinessResult();

            using (InformationSheetsBO bObj = new InformationSheetsBO())
            {
                result = bObj.ToggleClosePreventiveForm(rid, Convert.ToInt32(Request.Cookies["UserId"].Value));
            }

            return Json(new { Status = result.Result ? 1 : 0, ErrorMessage = result.ErrorMessage });
        }
    }
}