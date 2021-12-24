using Heka.DataAccess.Context;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Maintenance;
using HekaMOLD.Business.Models.Filters;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.UseCases
{
    public class MaintenanceBO : CoreSystemBO
    {
        #region PLAN BUSINESS
        public MaintenancePlanModel[] GetPlanList(BasicRangeFilter filter)
        {
            #region PREPARE FILTERS
            DateTime dtStart = DateTime.MinValue;
            DateTime dtEnd = DateTime.MinValue;

            if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
            {
                dtStart = DateTime.ParseExact(filter.StartDate, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
                dtEnd = DateTime.ParseExact(filter.EndDate, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
            }
            #endregion

            MaintenancePlanModel[] data = new MaintenancePlanModel[0];

            var repo = _unitOfWork.GetRepository<MaintenancePlan>();

            data = repo
                .Filter(d => 
                    (filter.MachineId == 0 || d.MachineId == filter.MachineId)
                    &&
                    (filter.UserId == 0 || d.ResponsibleId == filter.UserId)
                    &&
                    (dtStart == DateTime.MinValue || (d.StartDate >= dtStart && d.StartDate <= dtEnd))
                )
                .ToList()
                .Select(d => new MaintenancePlanModel
                {
                    Id = d.Id,
                    EndDate = d.EndDate,
                    Explanation = d.Explanation,
                    MachineCode = d.Machine != null ? d.Machine.MachineCode : "",
                    MachineName = d.Machine != null ? d.Machine.MachineName : "",
                    MachineId = d.MachineId,
                    PlanStatus = d.PlanStatus,
                    PlanStatusStr = ((MaintenancePlanStatusType)d.PlanStatus).ToCaption(),
                    ResponsibleId = d.ResponsibleId,
                    ResultExplanation = d.ResultExplanation,
                    StartDate = d.StartDate,
                    Subject = d.Subject,
                    UserCode = d.User != null ? d.User.UserCode : "",
                    UserName = d.User != null ? d.User.UserName : "",
                    StartDateStr = d.StartDate != null ? string.Format("{0:dd.MM.yyyy HH:mm}", d.StartDate) : "",
                    EndDateStr = d.EndDate != null ? string.Format("{0:dd.MM.yyyy HH:mm}", d.EndDate) : "",
                }).ToArray();

            return data;
        }

        public BusinessResult SaveOrUpdatePlan(MaintenancePlanModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (model.StartDate == null)
                    throw new Exception("Tarih bilgisi seçilmelidir.");                

                var repo = _unitOfWork.GetRepository<MaintenancePlan>();

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new MaintenancePlan();
                    repo.Add(dbObj);
                }

                model.MapTo(dbObj);

                _unitOfWork.SaveChanges();

                result.Result = true;
                result.RecordId = dbObj.Id;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult DeletePlan(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<MaintenancePlan>();

                var dbObj = repo.Get(d => d.Id == id);
                repo.Delete(dbObj);
                _unitOfWork.SaveChanges();

                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public MaintenancePlanModel GetPlan(int id)
        {
            MaintenancePlanModel model = new MaintenancePlanModel { };

            var repo = _unitOfWork.GetRepository<MaintenancePlan>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }

        #endregion
    }
}
