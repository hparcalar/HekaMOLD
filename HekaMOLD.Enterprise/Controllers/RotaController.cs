using HekaMOLD.Business.Models.DataTransfer.Logistics;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    public class RotaController : Controller
    {
        // GET: Rota
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetRotaList()
        {
            RotaModel[] result = new RotaModel[0];

            using (UsersBO bObj = new UsersBO())
            {
                result = bObj.GetRotaList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetSelectables()
        {
            CityModel[] citys = new CityModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                citys = bObj.GetCityList();
            }

            var jsonResult = Json(new { Citys = citys }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            RotaModel model = null;
            using (UsersBO bObj = new UsersBO())
            {
                model = bObj.GetRota(rid);
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
                    result = bObj.DeleteRota(rid);
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
        public JsonResult SaveModel(RotaModel model)
        {
            try
            {
                BusinessResult result = null;
                using (UsersBO bObj = new UsersBO())
                {
                    result = bObj.SaveOrUpdateRota(model);
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
        public JsonResult UploadProfileImage(int userId, HttpPostedFileBase file)
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
                    var objType = bObj.GetRota(userId);
                    objType.ProfileImage = byteReader.ToArray();
                    //objType.DataTypeExtension = file.ContentType;

                    result = bObj.SaveOrUpdateRota(objType);
                }
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return Json(result);
        }
    }
}