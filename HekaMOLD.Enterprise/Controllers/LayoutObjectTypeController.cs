using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Layout;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using HekaMOLD.Enterprise.Controllers.Attributes;
using HekaMOLD.Enterprise.Controllers.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    [UserAuthFilter]
    public class LayoutObjectTypeController : Controller
    {
        // GET: ItemCategory
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        [HttpGet]
        [FreeAction]
        public JsonResult GetLayoutObjectTypeList()
        {
            LayoutObjectTypeModel[] result = new LayoutObjectTypeModel[0];

            using (LayoutBO bObj = new LayoutBO())
            {
                result = bObj.GetLayoutObjectTypeList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        [FreeAction]
        public JsonResult BindModel(int rid)
        {
            LayoutObjectTypeModel model = null;
            using (LayoutBO bObj = new LayoutBO())
            {
                model = bObj.GetLayoutObjectType(rid);
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
                using (LayoutBO bObj = new LayoutBO())
                {
                    result = bObj.DeleteLayoutObjectType(rid);
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
        public JsonResult SaveModel(LayoutObjectTypeModel model)
        {
            try
            {
                BusinessResult result = null;
                using (LayoutBO bObj = new LayoutBO())
                {
                    result = bObj.SaveOrUpdateLayoutObjectType(model);
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
        public JsonResult UploadData(int recordId,
            HttpPostedFileBase file)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (file == null || file.ContentLength == 0)
                    throw new Exception("Yüklemek için bir dosya seçiniz.");

                var byteReader = new MemoryStream();
                file.InputStream.CopyTo(byteReader);

                using (LayoutBO bObj = new LayoutBO())
                {
                    var objType = bObj.GetLayoutObjectType(recordId);
                    objType.ObjectData = byteReader.ToArray();
                    //objType.DataTypeExtension = file.ContentType;

                    result = bObj.SaveOrUpdateLayoutObjectType(objType);

                    // SAVE LAYOUT OBJECT TO OUTPUTS DIRECTORY
                    if (result.Result)
                    {
                        var fileNameParts = file.FileName.Split('.');
                        file.SaveAs(Server.MapPath("~/Outputs/LObject_" + recordId + "." + fileNameParts[fileNameParts.Length - 1]));
                    }
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