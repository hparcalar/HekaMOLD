using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using HekaMOLD.Business.UseCases.Core;
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
        [HttpGet]
        public JsonResult GetNotifications()
        {
            NotificationModel[] data = new NotificationModel[0];
            using (UsersBO bObj = new UsersBO())
            {
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
    }
}