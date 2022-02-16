using HekaMOLD.Enterprise.Controllers.Filters;
using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HekaMOLD.Business.Models.DataTransfer.Authentication;
using System.IO;

namespace HekaMOLD.Enterprise.Controllers
{
    [UserAuthFilter]
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetUserList()
        {
            UserModel[] result = new UserModel[0];

            using (UsersBO bObj = new UsersBO())
            {
                result = bObj.GetUserList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetSelectables()
        {
            UserRoleModel[] roles = new UserRoleModel[0];

            using (UsersBO bObj = new UsersBO())
            {
                roles = bObj.GetUserRoleList();
            }

            var jsonResult = Json(new { Roles = roles }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            UserModel model = null;
            using (UsersBO bObj = new UsersBO())
            {
                model = bObj.GetUser(rid);
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
                using (UsersBO bObj = new UsersBO())
                {
                    result = bObj.DeleteUser(rid);
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
        public JsonResult SaveModel(UserModel model)
        {
            try
            {
                BusinessResult result = null;
                using (UsersBO bObj = new UsersBO())
                {
                    model.PlantId = Convert.ToInt32(Request.Cookies["PlantId"].Value);
                    result = bObj.SaveOrUpdateUser(model);
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
        public JsonResult UploadProfileImage(int userId,
            HttpPostedFileBase file)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (file == null || file.ContentLength == 0)
                    throw new Exception("Yüklemek için bir resim seçiniz.");

                var byteReader = new MemoryStream();
                file.InputStream.CopyTo(byteReader);

                using (UsersBO bObj = new UsersBO())
                {
                    var objType = bObj.GetUser(userId);
                    objType.ProfileImage = byteReader.ToArray();
                    //objType.DataTypeExtension = file.ContentType;

                    result = bObj.SaveOrUpdateUser(objType);
                }
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return Json(result);
        }

        #region NOTIFICATIONS
        public ActionResult Notifications()
        {

            return View();
        }
        #endregion
    }
}