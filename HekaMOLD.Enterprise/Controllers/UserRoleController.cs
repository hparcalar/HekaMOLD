using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Authentication;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.Models.Virtual;
using HekaMOLD.Business.UseCases;
using HekaMOLD.Enterprise.Controllers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    [UserAuthFilter]
    public class UserRoleController : Controller
    {
        // GET: UserRole
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAuthTypeList()
        {
            UserAuthTypeModel[] result = new UserAuthTypeModel[0];

            using (UsersBO bObj = new UsersBO())
            {
                result = bObj.GetAuthTypeList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetSelectables()
        {
            List<SubscriptionCategoryModel> result = new List<SubscriptionCategoryModel>();

            var values = Enum.GetValues(typeof(SubscriptionCategory));
            foreach (var valueItem in values)
            {
                result.Add(new SubscriptionCategoryModel
                {
                    Id = (int)valueItem,
                    Category = ((SubscriptionCategory)((int)valueItem)).ToCaption(),
                    IsChecked = false,
                });
            }

            var jsonResult = Json(result.ToArray(), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetUserRoleList()
        {
            UserRoleModel[] result = new UserRoleModel[0];

            using (UsersBO bObj = new UsersBO())
            {
                result = bObj.GetUserRoleList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            UserRoleModel model = null;
            using (UsersBO bObj = new UsersBO())
            {
                model = bObj.GetUserRole(rid);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteModel(int rid)
        {
            try
            {
                BusinessResult result = null;
                using (UsersBO bObj = new UsersBO())
                {
                    result = bObj.DeleteUserRole(rid);
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
        public JsonResult SaveModel(UserRoleModel model)
        {
            try
            {
                BusinessResult result = null;
                using (UsersBO bObj = new UsersBO())
                {
                    result = bObj.SaveOrUpdateUserRole(model);
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
    }
}