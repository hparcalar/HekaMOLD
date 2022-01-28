﻿using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Logistics;
using HekaMOLD.Business.Models.Operational;
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
    public class FirmController : Controller
    {
        // GET: Firm
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult ApprovedSuppliers()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetSelectables()
        {
            ForexTypeModel[] forexTypes = new ForexTypeModel[0];
            CityModel[] citys = new CityModel[0];
            CountryModel[] countrys = new CountryModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                forexTypes = bObj.GetForexTypeList();
                citys = bObj.GetCityList();
                countrys = bObj.GetCountryList();
            }

            var jsonResult = Json(new
            {
                ForexTypes = forexTypes,
                Citys = citys,
                Countrys = countrys
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        [HttpGet]
        public JsonResult GetFirmList()
        {
            FirmModel[] result = new FirmModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                result = bObj.GetFirmList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetApprovedSuppliers()
        {
            FirmModel[] result = new FirmModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                result = bObj.GetApprovedSuppliers();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult BindModel(int rid)
        {
            FirmModel model = null;
            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                model = bObj.GetFirm(rid);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteModel(int rid)
        {
            try
            {
                BusinessResult result = null;
                using (DefinitionsBO bObj = new DefinitionsBO())
                {
                    result = bObj.DeleteFirm(rid);
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
        public JsonResult SaveModel(FirmModel model)
        {
            try
            {
                BusinessResult result = null;
                using (DefinitionsBO bObj = new DefinitionsBO())
                {
                    result = bObj.SaveOrUpdateFirm(model);
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
                [HttpGet]
        public JsonResult GetFirmCode()
        {
           string firmCode = "";

            using (RequestBO bObj = new RequestBO())
            {
                firmCode = bObj.GetNextFirmCode();
            }
            var jsonResult = Json(new { Result = !string.IsNullOrEmpty(firmCode), FirmCode = firmCode }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
    }
}