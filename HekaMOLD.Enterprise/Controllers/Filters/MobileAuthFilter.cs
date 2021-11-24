using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers.Filters
{
    public class MobileAuthFilter : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //filterContext.HttpContext.Response.Headers["Content-Type"] += ";charset=utf-8";
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.QueryString.AllKeys.Contains("ShowAs"))
            {
                var showAs = filterContext.HttpContext.Request.QueryString["ShowAs"];
                filterContext.HttpContext.Response.Cookies.Set(new HttpCookie("ShowAs", HttpUtility.UrlEncode(showAs)));
                filterContext.HttpContext.Response.Cookies["ShowAs"].Expires = DateTime.Now.AddDays(1);
            }

            if (string.Equals(filterContext.ActionDescriptor.ActionName, "Login") &&
                string.Equals(filterContext.RouteData.Values["controller"].ToString(), "Mobile"))
            {
                if (filterContext.HttpContext.Request.Cookies.AllKeys.Contains("UserId"))
                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(

                        new { controller = "Mobile", action = "Index" }
                    ));
            }
            else
            {
                if (!filterContext.HttpContext.Request.Cookies.AllKeys.Contains("UserId"))
                {
                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(
                        new { controller = "Home", action = "Login" }));
                }
            }
        }
    }
}