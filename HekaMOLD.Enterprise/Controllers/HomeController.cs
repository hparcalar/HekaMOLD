using HekaMOLD.Business.Models.Authentication;
using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Production;
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
            return RedirectToAction("List", "SIOffer");
            //return View();
        }

        #region LOGIN PROCESS
        public ActionResult Login()
        {
            // CHECK & RUN MIGRATION SCRTIPS
            //HekaBO hekaBase = new HekaBO();
            //hekaBase.RunMigrations();

            return View();
        }

        [HttpGet]
        [FreeAction]
        public JsonResult GetSelectables()
        {
            UserModel[] users = new UserModel[0];
            MachineModel[] machines = new MachineModel[0];

            using (UsersBO bObj = new UsersBO())
            {
                users = bObj.GetUserList();
            }

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                machines = bObj.GetMachineList();
            }

            var jsonResult = Json(new
            {
                Users = users,
                Machines = machines,
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [FreeAction]
        public ActionResult Login(LoginModel model)
        {
            try
            {
                AuthenticationResult authResult = null;
                using (UsersBO bObj = new UsersBO())
                {
                    authResult = bObj.Authenticate(model.Login, model.Password);
                }

                if (!authResult.Result)
                    throw new Exception(authResult.ErrorMessage);
                else
                {
                    if (authResult.UserData.IsProdTerminal
                        && !authResult.UserData.IsProdChief)
                    {
                        if (model.MachineId <= 0)
                            throw new Exception("Makine seçmelisiniz.");

                        BusinessResult machineAvailableResult = null;
                        using (UsersBO bObj = new UsersBO())
                        {
                            machineAvailableResult =
                                bObj.IsMachineAvailableForLoggedIn(authResult.UserData.Id, model.MachineId);
                        }

                        if (!machineAvailableResult.Result)
                            throw new Exception(machineAvailableResult.ErrorMessage);
                        else
                        {
                            BusinessResult logIntoMachineResult = null;
                            using (UsersBO bObj = new UsersBO())
                            {
                                logIntoMachineResult = bObj.LogUserIntoMachine(authResult.UserData.Id, model.MachineId);
                            }

                            if (!logIntoMachineResult.Result)
                                throw new Exception(logIntoMachineResult.ErrorMessage);

                            Response.Cookies.Set(new HttpCookie("MachineName", Server.UrlEncode(logIntoMachineResult.Code)));
                            Response.Cookies["MachineName"].Expires = DateTime.Now.AddDays(1);
                        }

                        Response.Cookies.Set(new HttpCookie("MachineId", model.MachineId.ToString()));
                        Response.Cookies["MachineId"].Expires = DateTime.Now.AddDays(1);
                    }
                }

                Response.Cookies.Set(new HttpCookie("UserId", authResult.UserData.Id.ToString()));
                Response.Cookies["UserId"].Expires = DateTime.Now.AddDays(1);

                Response.Cookies.Set(new HttpCookie("PlantId", authResult.UserData.PlantId.ToString()));
                Response.Cookies["PlantId"].Expires = DateTime.Now.AddDays(1);

                Response.Cookies.Set(new HttpCookie("UserCode", Server.UrlEncode(authResult.UserData.UserCode)));
                Response.Cookies["UserCode"].Expires = DateTime.Now.AddDays(1);

                Response.Cookies.Set(new HttpCookie("UserName", Server.UrlEncode(authResult.UserData.UserName)));
                Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(1);

                RedirectToRouteResult redirectionResult = null;
                if (authResult.UserData.IsMechanicTerminal
                    || authResult.UserData.IsProdTerminal || authResult.UserData.IsWarehouseTerminal)
                    redirectionResult = RedirectToAction("Index", "Mobile");
                else
                    redirectionResult = RedirectToAction("Index", "Home");

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
            if (Request.Cookies.AllKeys.Contains("UserId")
                && Request.Cookies["UserId"].Value != null)
            {
                using (UsersBO bObj = new UsersBO())
                {
                    bObj.LogOffUserFromMachine(Convert.ToInt32(Request.Cookies["UserId"].Value));
                }
            }

            Response.Cookies["UserId"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["MachineId"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["MachineName"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["ProfileImage"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["ShowAs"].Expires = DateTime.Now.AddDays(-1);

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

        #region DASHBOARDS
        [FreeAction]
        public ActionResult Dashboard()
        {
            return View();
        }

        [FreeAction]
        public ActionResult PlainDashboard()
        {
            return View();
        }
        #endregion
    }
}