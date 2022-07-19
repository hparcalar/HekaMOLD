﻿using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using HekaMOLD.Enterprise.Controllers.Attributes;
using HekaMOLD.Enterprise.Controllers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    [UserAuthFilter]
    public class MachineController : Controller
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

        public ActionResult Online()
        {

            return View();
        }

        [HttpGet]
        [FreeAction]
        public JsonResult GetMachineList()
        {
            MachineModel[] result = new MachineModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                result = bObj.GetMachineList();
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetSelectables()
        {
            MachineGroupModel[] groups = new MachineGroupModel[0];

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                groups = bObj.GetMachineGroupList();
            }

            var jsonResult = Json(new
            {
                Groups = groups,
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        [FreeAction]
        public JsonResult GetMachineActions(int machineId, string filterDate)
        {
            UserWorkOrderHistoryModel[] data = new UserWorkOrderHistoryModel[0];

            using (ProductionBO bObj = new ProductionBO())
            {
                data = bObj.GetMachineActions(machineId, DateTime.ParseExact(filterDate, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr")));
            }

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        [FreeAction]
        public JsonResult GetMachineStats(string t1, string t2)
        {
            MachineModel[] result = new MachineModel[0];

            ShiftModel shiftData = null;
            using (ProductionBO bObj = new ProductionBO())
            {
                shiftData = bObj.GetCurrentShift();
            }

            if (shiftData != null)
            {
                DateTime dt1 = DateTime.ParseExact(t1, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
                if (shiftData.ShiftBelongsToDate < dt1.Date)
                {
                    t1 = string.Format("{0:dd.MM.yyyy}", shiftData.ShiftBelongsToDate);
                    t2 = t1;//string.Format("{0:dd.MM.yyyy}", shiftData.ShiftBelongsToDate);
                }
            }

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                result = bObj.GetMachineStats(t1, t2);
            }

            var jsonResult = Json(new { Data=result, Shift = shiftData }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        [FreeAction]
        public JsonResult GetMachineStatsForDashboard()
        {
            MachineModel[] result = new MachineModel[0];

            ShiftModel shiftData = null;
            using (ProductionBO bObj = new ProductionBO())
            {
                shiftData = bObj.GetCurrentShift();
            }

            if (shiftData != null)
            {
                string t1 = string.Format("{0:dd.MM.yyyy}", shiftData.ShiftBelongsToDate);
                string t2 = string.Format("{0:dd.MM.yyyy}", shiftData.ShiftBelongsToDate);

                using (DefinitionsBO bObj = new DefinitionsBO())
                {
                    result = bObj.GetMachineStats(t1, t2);
                }

                // GÜNDÜZ VARDİYASINDA İSEK
                //if (shiftData.StartTime < shiftData.EndTime)
                //{
                //    t1 = string.Format("{0:dd.MM.yyyy}", shiftData.ShiftBelongsToDate.Value.AddDays(-1));
                //    t2 = string.Format("{0:dd.MM.yyyy}", shiftData.ShiftBelongsToDate.Value.AddDays(-1));

                //    using (DefinitionsBO bObj = new DefinitionsBO())
                //    {
                //        var resultYesterday = bObj.GetMachineStats(t1, t2);
                //        foreach (var item in resultYesterday)
                //        {
                //            var otherShiftData = item.MachineStats.ShiftStats.FirstOrDefault(d => d.ShiftId != shiftData.Id);
                //            var currentMachineData = result.FirstOrDefault(d => d.Id == item.Id);
                //            if (currentMachineData != null)
                //            {
                //                //currentMachineData.MachineStats
                //            }
                //        }
                //    }
                //}
            }

            var jsonResult = Json(new { Data=result, Shift= shiftData }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        [FreeAction]
        public JsonResult GetUserProfiles(string userIdPrm)
        {
            int[] userIdList = userIdPrm.Split(',').Select(d => Convert.ToInt32(d)).ToArray();

            List<UserModel> data = new List<UserModel>();
            using (UsersBO bObj = new UsersBO())
            {
                foreach (var userId in userIdList)
                {
                    var userModel = bObj.GetUser(userId);
                    if (userModel != null && userModel.Id > 0)
                    {
                        data.Add(userModel);
                    }
                }
            }

            var jsonResult = Json(data.ToArray(), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        [FreeAction]
        public JsonResult GetMachineStatsByMachineId(int machineId, string t1, string t2)
        {
            MachineModel[] result = new MachineModel[0];

            ShiftModel shiftData = null;
            using (ProductionBO bObj = new ProductionBO())
            {
                shiftData = bObj.GetCurrentShift();
            }

            if (shiftData != null)
            {
                DateTime dt1 = DateTime.ParseExact(t1, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
                if (shiftData.ShiftBelongsToDate < dt1.Date)
                {
                    t1 = string.Format("{0:dd.MM.yyyy}", shiftData.ShiftBelongsToDate);
                    t2 = string.Format("{0:dd.MM.yyyy}", shiftData.ShiftBelongsToDate);
                }
            }

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                result = bObj.GetMachineStats(machineId, t1, t2);
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        [FreeAction]
        public JsonResult BindModel(int rid)
        {
            MachineModel model = null;
            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                model = bObj.GetMachine(rid);
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
                    result = bObj.DeleteMachine(rid);
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
        public JsonResult SaveModel(MachineModel model)
        {
            try
            {
                BusinessResult result = null;
                using (DefinitionsBO bObj = new DefinitionsBO())
                {
                    result = bObj.SaveOrUpdateMachine(model);
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