using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers.Filters
{
    public class BossAuthFilter : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //filterContext.HttpContext.Response.Headers["Content-Type"] += ";charset=utf-8";
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (string.Equals(filterContext.ActionDescriptor.ActionName, "Login") &&
                string.Equals(filterContext.RouteData.Values["controller"].ToString(), "Boss"))
            {
                if (filterContext.HttpContext.Request.Cookies.AllKeys.Contains("UserId"))
                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(

                        new { controller = "Boss", action = "Index" }
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