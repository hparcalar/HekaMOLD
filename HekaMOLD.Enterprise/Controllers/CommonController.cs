using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Files;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using HekaMOLD.Business.UseCases.Core;
using HekaMOLD.Enterprise.Controllers.Attributes;
using HekaMOLD.Enterprise.Controllers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    /// <summary>
    /// COMMON DATA - JSON API
    /// </summary>
    [UserAuthFilter]
    public class CommonController : Controller
    {
        #region NOTIFICATIONS
        [FreeAction]
        [HttpGet]
        public JsonResult GetNotifications()
        {
            NotificationModel[] data = new NotificationModel[0];
            using (UsersBO bObj = new UsersBO())
            {
                if (Request.Cookies.AllKeys.Contains("UserId"))
                    data = bObj.GetAwaitingNotifications(Convert.ToInt32(Request.Cookies["UserId"].Value), topRecords: 5);
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetAllNotifications()
        {
            NotificationModel[] data = new NotificationModel[0];
            using (UsersBO bObj = new UsersBO())
            {
                data = bObj.GetAwaitingNotifications(Convert.ToInt32(Request.Cookies["UserId"].Value));
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult SetNotifyAsSeen(int notificationId)
        {
            BusinessResult result = new BusinessResult();

            using (UsersBO bObj = new UsersBO())
            {
                result = bObj.SetNotifyAsSeen(Convert.ToInt32(Request.Cookies["UserId"].Value), notificationId);
            }

            return Json(result);
        }
        #endregion

        #region RECORD INFORMATION

        [HttpGet]
        public JsonResult GetRecordInformation(int id, string dataType)
        {
            RecordInformationModel data = new RecordInformationModel();
            using (CoreRecordTracingBO bObj = new CoreRecordTracingBO())
            {
                data = bObj.GetRecordInformation(id, dataType);
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region FOREX DATA

        [HttpGet]
        public JsonResult GetForexRate(string forexCode, string forexDate)
        {
            ForexHistoryModel data = new ForexHistoryModel();

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                data = bObj
                    .GetForexValue(forexCode, DateTime.ParseExact(forexDate, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr")));
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ATTACHMENTS
        [FreeAction]
        [HttpGet]
        public JsonResult GetAttachments(int recordId, int recordType)
        {
            AttachmentModel[] data = new AttachmentModel[0];
            using (FilesBO bObj = new FilesBO())
            {
                data = bObj.GetAttachmentList(recordId, (RecordType)recordType);
                foreach (var item in data)
                {
                    item.BinaryContent = null;
                }
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [FreeAction]
        [HttpGet]
        public FileContentResult ShowAttachment(int attachmentId)
        {
            AttachmentModel data = new AttachmentModel();

            using (FilesBO bObj = new FilesBO())
            {
                data = bObj.GetAttachment(attachmentId);
            }

            return File(data.BinaryContent, data.ContentType);
        }

        [HttpPost]
        public JsonResult SaveAttachments(int recordId, int recordType, AttachmentModel[] data)
        {
            BusinessResult result = new BusinessResult();

            using (FilesBO bObj = new FilesBO())
            {
                result = bObj.SaveAttachments(recordId, (RecordType)recordType, data);
            }

            return Json(result);
        }
        #endregion
    }
}