using Heka.DataAccess.Context;
using Heka.DataAccess.UnitOfWork;
using HekaMOLD.Business.Base;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.Models.DataTransfer.PreventiveActions;
using HekaMOLD.Business.UseCases.Core;
using HekaMOLD.Business.UseCases.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.UseCases
{
    public class InformationSheetsBO : CoreSystemBO
    {
        public PreventiveActionModel[] GetPreventiveActionList()
        {
            var repo = _unitOfWork.GetRepository<PreventiveAction>();

            return repo.GetAll()
                .ToList()
                .Select(d => new PreventiveActionModel
                {
                    Id = d.Id,
                    ActionType = d.ActionType,
                    ApplicantFirmName = d.ApplicantFirmName,
                    ApplicantIdentity = d.ApplicantIdentity,
                    ApplicantName = d.ApplicantName,
                    ApplicantTitle = d.ApplicantTitle,
                    ApproverUserId = d.ApproverUserId,
                    ApproverUserName = d.ApproverUser != null ? d.ApproverUser.UserName : "",
                    ClosingUserId = d.ClosingUserId,
                    ClosingUserName = d.ClosingUser != null ? d.ClosingUser.UserName : "",
                    Declaration = d.Declaration,
                    FormDate = d.FormDate,
                    FormNo = d.FormNo,
                    FormResult = d.FormResult,
                    FormResultText = (d.FormResult ?? 0) == 0 ? "Bekliyor" : (d.FormResult == 1 ? "Risk Ortadan Kaldırıldı" : "Süreç Başarısız"),
                    RootCause = d.RootCause,
                    SolutionProposal = d.SolutionProposal,
                    FormDateText = string.Format("{0:dd.MM.yyyy}", d.FormDate),
                    ApproveState = d.ApproverUserId != null ? "Onaylandı" : "Onay Bekliyor",
                    CloseState = d.ClosingUserId != null ? "Kapalı" : "Açık",
                }).ToArray();
        }
        public string GetNextPreventiveActionFormNo()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<PreventiveAction>();
                string lastReceiptNo = repo.GetAll()
                    .OrderByDescending(d => d.FormNo)
                    .Select(d => d.FormNo)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(lastReceiptNo))
                    lastReceiptNo = "0";

                return string.Format("{0:000000}", Convert.ToInt32(lastReceiptNo) + 1);
            }
            catch (Exception)
            {

            }

            return default;
        }
        public BusinessResult SaveOrUpdatePreventiveAction(PreventiveActionModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (!string.IsNullOrEmpty(model.FormDateText))
                    model.FormDate = DateTime.ParseExact(model.FormDateText, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));

                var repo = _unitOfWork.GetRepository<PreventiveAction>();
                var repoDetails = _unitOfWork.GetRepository<PreventiveActionDetail>();

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new PreventiveAction();
                    repo.Add(dbObj);
                }

                var crDate = dbObj.FormDate;
                var exFormNo = dbObj.FormNo;

                model.MapTo(dbObj);

                dbObj.FormNo = exFormNo;

                if (dbObj.FormDate == null)
                    dbObj.FormDate = crDate;

                if (dbObj.Id == 0)
                    dbObj.FormNo = GetNextPreventiveActionFormNo();

                #region SAVE DETAILS
                if (model.Details == null)
                    model.Details = new PreventiveActionDetailModel[0];

                var toBeRemovedDetails = dbObj.PreventiveActionDetail
                    .Where(d => !model.Details.Where(m => m.NewDetail == false)
                        .Select(m => m.Id).ToArray().Contains(d.Id)
                    ).ToArray();
                foreach (var item in toBeRemovedDetails)
                {
                    repoDetails.Delete(item);
                }

                foreach (var item in model.Details)
                {
                    if (item.NewDetail == true)
                    {
                        var dbItemAu = new PreventiveActionDetail();
                        item.MapTo(dbItemAu);
                        if (dbItemAu.ResponsibleUserId == 0)
                            dbItemAu.ResponsibleUserId = null;
                        dbItemAu.PreventiveAction = dbObj;
                        repoDetails.Add(dbItemAu);
                    }
                    else if (!toBeRemovedDetails.Any(d => d.Id == item.Id))
                    {
                        var dbItemAu = repoDetails.GetById(item.Id);
                        item.MapTo(dbItemAu);
                        if (dbItemAu.ResponsibleUserId == 0)
                            dbItemAu.ResponsibleUserId = null;
                        dbItemAu.PreventiveAction = dbObj;
                    }
                }
                #endregion

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

        public BusinessResult DeletePreventiveAction(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<PreventiveAction>();
                var repoDetails = _unitOfWork.GetRepository<PreventiveActionDetail>();

                var dbObj = repo.Get(d => d.Id == id);
                if (dbObj != null)
                {
                    var details = dbObj.PreventiveActionDetail.ToArray();
                    foreach (var item in details)
                    {
                        repoDetails.Delete(item);
                    }
                }

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

        public PreventiveActionModel GetPreventiveAction(int id)
        {
            PreventiveActionModel model = new PreventiveActionModel { };

            var repo = _unitOfWork.GetRepository<PreventiveAction>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.FormDateText = string.Format("{0:dd.MM.yyyy}", dbObj.FormDate);

                #region GET DETAILS
                List<PreventiveActionDetailModel> detailList = new List<PreventiveActionDetailModel>();
                dbObj.PreventiveActionDetail.ToList().ForEach(d =>
                {
                    PreventiveActionDetailModel detailModel = new PreventiveActionDetailModel();
                    d.MapTo(detailModel);
                    detailModel.ActionStatusText = (d.ActionStatus ?? 0) == 0 ? "Bekliyor" : (d.ActionStatus == 1 ? "Çalışılıyor" : 
                        (d.ActionStatus == 2 ? "Tamamlandı" : "İptal Edildi")
                    );
                    detailModel.DeadlineDateText = d.DeadlineDate != null ?
                        string.Format("{0:dd.MM.yyyy}", d.DeadlineDate) : "";
                    detailList.Add(detailModel);
                });
                model.Details = detailList.ToArray();
                #endregion
            }

            return model;
        }

        public BusinessResult ToggleApprovePreventiveForm(int id, int? approverId = null)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<PreventiveAction>();
                var dbObj = repo.Get(d => d.Id == id);
                if (dbObj != null)
                {
                    if (dbObj.ApproverUserId == null)
                        dbObj.ApproverUserId = approverId;
                    else
                        dbObj.ApproverUserId = null;

                    if (dbObj.ApproverUserId == null)
                        dbObj.FormResult = 0;

                    _unitOfWork.SaveChanges();
                }

                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult ToggleClosePreventiveForm(int id, int? closingUserId = null)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<PreventiveAction>();
                var dbObj = repo.Get(d => d.Id == id);
                if (dbObj != null)
                {
                    if (dbObj.ClosingUserId == null)
                        dbObj.ClosingUserId = closingUserId;
                    else
                        dbObj.ClosingUserId = null;

                    _unitOfWork.SaveChanges();
                }

                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }
    }
}
