using HekaMOLD.Business.Models.DataTransfer.Logistics;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    public class DriverController : Controller
    {
        // GET: driver
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetDriverList()
        {
            DriverModel[] result = new DriverModel[0];

            using (UsersBO bObj = new UsersBO())
            {
                result = bObj.GetDriverList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetSelectables()
        {
            CountryModel[] countrys = new CountryModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                countrys = bObj.GetCountryList();
            }

            var jsonResult = Json(new { Countrys = countrys }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            DriverModel model = null;
            using (UsersBO bObj = new UsersBO())
            {
                model = bObj.GetDriver(rid);
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
                    result = bObj.DeleteDriver(rid);
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
        public JsonResult SaveModel(DriverModel model)
        {
            try
            {
                BusinessResult result = null;
                using (UsersBO bObj = new UsersBO())
                {
                    result = bObj.SaveOrUpdateDriver(model);
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
                    var objType = bObj.GetDriver(userId);
                    objType.ProfileImage = byteReader.ToArray();
                    //objType.DataTypeExtension = file.ContentType;

                    result = bObj.SaveOrUpdateDriver(objType);
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