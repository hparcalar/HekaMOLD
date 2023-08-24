using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Crm;
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
    public class CrmComplaintController : Controller
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

            using (CrmBO bObj = new CrmBO())
            {
                receiptNo = bObj.GetNextComplaintFormNo();
            }

            var jsonResult = Json(new { Result = !string.IsNullOrEmpty(receiptNo), ReceiptNo = receiptNo }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetSelectables()
        {
            FirmModel[] firms = new FirmModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                firms = bObj.GetFirmList();
            }

            var jsonResult = Json(new
            {
                Firms = firms,
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetFormList()
        {
            CustomerComplaintModel[] result = new CustomerComplaintModel[0];

            using (CrmBO bObj = new CrmBO())
            {
                result = bObj.GetComplaintList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            CustomerComplaintModel model = null;
            using (CrmBO bObj = new CrmBO())
            {
                model = bObj.GetComplaint(rid);
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
                using (CrmBO bObj = new CrmBO())
                {
                    result = bObj.DeleteComplaint(rid);
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
        public JsonResult SaveModel(CustomerComplaintModel model)
        {
            try
            {
                BusinessResult result = null;
                using (CrmBO bObj = new CrmBO())
                {
                    result = bObj.SaveOrUpdateComplaint(model);
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
        public JsonResult CreateAction(int rid)
        {
            BusinessResult result = new BusinessResult();

            using (CrmBO bObj = new CrmBO())
            {
                result = bObj.ToggleCreateActionByComplaint(rid, Convert.ToInt32(Request.Cookies["UserId"].Value));
            }

            return Json(new { Status = result.Result ? 1 : 0, ErrorMessage = result.ErrorMessage });
        }

        [HttpPost]
        public JsonResult CloseForm(int rid)
        {
            BusinessResult result = new BusinessResult();

            using (CrmBO bObj = new CrmBO())
            {
                result = bObj.ToggleCloseComplaint(rid, Convert.ToInt32(Request.Cookies["UserId"].Value));
            }

            return Json(new { Status = result.Result ? 1 : 0, ErrorMessage = result.ErrorMessage });
        }
    }
}