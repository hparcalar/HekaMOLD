using HekaMOLD.Business.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Helpers
{
    public static class HekaHtmlHelper
    {
        public static bool IsGranted(this HtmlHelper helper, string authType)
        {
            try
            {
                bool isGranted = false;
                using (UsersBO bObj = new UsersBO())
                {
                    var aRes = bObj.IsAuthenticated(authType, Convert.ToInt32(helper.ViewContext.RequestContext
                        .HttpContext.Request.Cookies["UserId"].Value));
                    isGranted = aRes.Result;
                }

                return isGranted;
            }
            catch (Exception)
            {

            }

            return false;
        }

        public static bool IsGranted(this Controller controller, string authType)
        {
            try
            {
                bool isGranted = false;
                using (UsersBO bObj = new UsersBO())
                {
                    var aRes = bObj.IsAuthenticated(authType, Convert.ToInt32(controller.HttpContext
                        .Request.Cookies["UserId"].Value));
                    isGranted = aRes.Result;
                }

                return isGranted;
            }
            catch (Exception)
            {

            }

            return false;
        }

        public static bool IsGranted(int userId, string authType)
        {
            try
            {
                bool isGranted = false;
                using (UsersBO bObj = new UsersBO())
                {
                    var aRes = bObj.IsAuthenticated(authType, userId);
                    isGranted = aRes.Result;
                }

                return isGranted;
            }
            catch (Exception)
            {

            }

            return false;
        }
    }
}