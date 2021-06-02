using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HekaMOLD.Enterprise.Controllers.Attributes;
using HekaMOLD.Enterprise.Helpers;

namespace HekaMOLD.Enterprise.Controllers.Filters
{
    public class UserAuthFilter : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //filterContext.HttpContext.Response.Headers["Content-Type"] += ";charset=utf-8";
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionDescriptor
                .GetCustomAttributes(typeof(FreeAction), false).Length > 0)
                return;

            if (string.Equals(filterContext.ActionDescriptor.ActionName, "Login") &&
                string.Equals(filterContext.RouteData.Values["controller"].ToString(), "Home"))
            {
                if (filterContext.HttpContext.Request.Cookies.AllKeys.Contains("UserId"))
                {
                    string properCtrl = "Home";
                    var userId = filterContext.HttpContext.Request.Cookies["UserId"].Value;
                    if (HekaHtmlHelper.IsGranted(Convert.ToInt32(userId), "MobileProductionUser")
                        || HekaHtmlHelper.IsGranted(Convert.ToInt32(userId), "MobileMechanicUser")
                        || HekaHtmlHelper.IsGranted(Convert.ToInt32(userId), "MobileWarehouseUser"))
                        properCtrl = "Mobile";

                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(

                        new { controller = properCtrl, action = "Index" }
                    ));
                }
            }
            else
            {
                if (!filterContext.HttpContext.Request.Cookies.AllKeys.Contains("UserId"))
                {
                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(
                        new { controller = "Home", action = "Login" }));
                }
                else
                {
                    var userId = filterContext.HttpContext.Request.Cookies["UserId"].Value;
                    if (HekaHtmlHelper.IsGranted(Convert.ToInt32(userId), "MobileProductionUser")
                        || HekaHtmlHelper.IsGranted(Convert.ToInt32(userId), "MobileMechanicUser")
                        || HekaHtmlHelper.IsGranted(Convert.ToInt32(userId), "MobileWarehouseUser"))
                    {
                        filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(
                        new { controller = "Mobile", action = "Index" }));
                    }
                }
            }
        }
    }
}