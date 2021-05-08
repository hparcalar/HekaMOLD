using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.UseCases;
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
    }
}