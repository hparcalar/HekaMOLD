using HekaMOLD.Business.Models.Authentication;
using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using HekaMOLD.Enterprise.Controllers.Attributes;
using HekaMOLD.Enterprise.Controllers.Filters;
using HekaMOLD.Enterprise.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    [UserAuthFilter]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        #region LOGIN PROCESS
        public ActionResult Login()
        {
            ViewBag.Users = BindUsers();

            return View();
        }

        private UserModel[] BindUsers()
        {
            UserModel[] data = new UserModel[0];

            using (UsersBO bObj = new UsersBO())
            {
                data = bObj.GetUserList().OrderBy(d => d.UserCode).ToArray();
            }

            return data;
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            try
            {
                ViewBag.Users = BindUsers();

                AuthenticationResult authResult = null;
                using (UsersBO bObj = new UsersBO())
                {
                    authResult = bObj.Authenticate(model.Login, model.Password);
                }

                if (!authResult.Result)
                    throw new Exception(authResult.ErrorMessage);

                Response.Cookies.Set(new HttpCookie("UserId", authResult.UserData.Id.ToString()));
                Response.Cookies["UserId"].Expires = DateTime.Now.AddDays(1);

                Response.Cookies.Set(new HttpCookie("PlantId", authResult.UserData.PlantId.ToString()));
                Response.Cookies["PlantId"].Expires = DateTime.Now.AddDays(1);

                Response.Cookies.Set(new HttpCookie("UserCode", Server.UrlEncode(authResult.UserData.UserCode)));
                Response.Cookies["UserCode"].Expires = DateTime.Now.AddDays(1);

                Response.Cookies.Set(new HttpCookie("UserName", Server.UrlEncode(authResult.UserData.UserName)));
                Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(1);

                //Response.Cookies.Set(new HttpCookie("Gates", JsonConvert.SerializeObject(authResult.GateList)));
                //Response.Cookies["Gates"].Expires = DateTime.Now.AddDays(1);

                RedirectToRouteResult redirectionResult = null;
                if (authResult.UserData.IsMechanicTerminal
                    || authResult.UserData.IsProdTerminal || authResult.UserData.IsWarehouseTerminal)
                    redirectionResult = RedirectToAction("Index", "Mobile");
                else
                    redirectionResult = RedirectToAction("Index", "Home");

                // CHECK & RUN MIGRATION SCRTIPS
                HekaBO hekaBase = new HekaBO();
                hekaBase.RunMigrations();

                return redirectionResult;
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            return View(model);
        }

        [FreeAction]
        public ActionResult Logout()
        {
            Response.Cookies["UserId"].Expires = DateTime.Now.AddDays(-1);

            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region USER SETTINGS
        public ActionResult Settings(int? rid)
        {
            return View();
        }

        public ActionResult SettingsMobile(int? rid)
        {
            return View();
        }

        [FreeAction]
        [HttpGet]
        public JsonResult BindUser(int rid)
        {
            UserModel data = null;

            using (UsersBO bObj = new UsersBO())
            {
                data = bObj.GetUser(rid);
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [FreeAction]
        public JsonResult SaveUserSettings(UserModel model)
        {
            BusinessResult result = new BusinessResult();

            using (UsersBO bObj = new UsersBO())
            {
                result = bObj.SaveOrUpdateUser(model);
            }

            return Json(result);
        }
        #endregion
    }
}