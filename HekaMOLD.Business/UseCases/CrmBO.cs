using HekaMOLD.Business.UseCases.Core.Base;
using HekaMOLD.Business.Models.DataTransfer.Crm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heka.DataAccess.Context;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.Helpers;

namespace HekaMOLD.Business.UseCases
{
    public class CrmBO : CoreSystemBO
    {
        public CustomerComplaintModel[] GetComplaintList()
        {
            var repo = _unitOfWork.GetRepository<CustomerComplaint>();

            return repo.GetAll()
                .ToList()
                .Select(d => new CustomerComplaintModel
                {
                    Id = d.Id,
                    ActionDate = d.ActionDate,
                    ActionDateText = d.ActionDate != null ? string.Format("{0:dd.MM.yyyy}", d.ActionDate) : "",
                    ClosedDate = d.ClosedDate,
                    ClosedDateText = d.ClosedDate != null ? string.Format("{0:dd.MM.yyyy}", d.ClosedDate) : "",
                    CustomerDocumentNo = d.CustomerDocumentNo,
                    Explanation = d.Explanation,
                    FirmId = d.FirmId,
                    FormDate = d.FormDate,
                    FormDateText = d.FormDate != null ? string.Format("{0:dd.MM.yyyy}", d.FormDate) : "",
                    FormNo = d.FormNo,
                    FormStatus = d.FormStatus,
                    FormStatusText = (d.FormStatus ?? 0) == 0 ? "Bekliyor" :
                    (d.FormStatus == 1 ? "Çalışılıyor" : (d.FormStatus == 2 ? "Tamamlandı" : "İptal Edildi")),
                    IncomingType = d.IncomingType,
                    IncomingTypeText = (d.IncomingType ?? 0) == 0 ? "Telefon" :
                        (d.IncomingType == 1 ? "Email" : ""),
                    Notes = d.Notes,
                    PreventiveActionFormNo = d.PreventiveAction != null ? d.PreventiveAction.FormNo : "",
                    PreventiveActionId = d.PreventiveActionId,
                    FirmCode = d.Firm != null ? d.Firm.FirmCode : "",
                    FirmName = d.Firm != null ? d.Firm.FirmName : "",
                }).ToArray();
        }
        public string GetNextComplaintFormNo()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<CustomerComplaint>();
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
        public BusinessResult SaveOrUpdateComplaint(CustomerComplaintModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (!string.IsNullOrEmpty(model.FormDateText))
                    model.FormDate = DateTime.ParseExact(model.FormDateText, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));

                if ((model.FirmId ?? 0) == 0)
                    throw new Exception("Bir firma seçmelisiniz.");

                var repo = _unitOfWork.GetRepository<CustomerComplaint>();

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new CustomerComplaint();
                    repo.Add(dbObj);
                }

                var crDate = dbObj.FormDate;
                var exFormNo = dbObj.FormNo;

                model.MapTo(dbObj);

                dbObj.FormNo = exFormNo;

                if (dbObj.FormDate == null)
                    dbObj.FormDate = crDate;

                if (dbObj.Id == 0)
                    dbObj.FormNo = GetNextComplaintFormNo();

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

        public BusinessResult DeleteComplaint(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<CustomerComplaint>();

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

        public CustomerComplaintModel GetComplaint(int id)
        {
            CustomerComplaintModel model = new CustomerComplaintModel { };

            var repo = _unitOfWork.GetRepository<CustomerComplaint>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.FormDateText = string.Format("{0:dd.MM.yyyy}", dbObj.FormDate);
                model.ActionDateText = dbObj.ActionDate != null ? string.Format("{0:dd.MM.yyyy}", dbObj.ActionDate) : "";
                model.ClosedDateText = dbObj.ClosedDate != null ? string.Format("{0:dd.MM.yyyy}", dbObj.ClosedDate) : "";
                model.FormStatusText = (dbObj.FormStatus ?? 0) == 0 ? "Bekliyor" : 
                    (dbObj.FormStatus == 1 ? "Çalışılıyor" : (dbObj.FormStatus == 2 ? "Tamamlandı" : "İptal Edildi"));
                model.PreventiveActionFormNo = dbObj.PreventiveAction != null ?
                        dbObj.PreventiveAction.FormNo : "";
            }

            return model;
        }

        public BusinessResult ToggleCreateActionByComplaint(int id, int? approverId = null)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<CustomerComplaint>();
                var repoAction = _unitOfWork.GetRepository<PreventiveAction>();
                var repoActionDetail = _unitOfWork.GetRepository<PreventiveActionDetail>();

                var dbObj = repo.Get(d => d.Id == id);
                if (dbObj != null)
                {
                    if (dbObj.PreventiveActionId == null)
                    {
                        var dbAction = new PreventiveAction
                        {
                            ActionType = "Düzeltici Faaliyet",
                            ApplicantFirmName = dbObj.Firm.FirmName,
                            FormDate = DateTime.Now,
                            Declaration = dbObj.Explanation,
                            SolutionProposal = dbObj.Notes,
                            FormResult = 0,
                        };

                        using (InformationSheetsBO bObj = new InformationSheetsBO())
                        {
                            dbAction.FormNo = bObj.GetNextPreventiveActionFormNo();
                        }

                        repoAction.Add(dbAction);

                        dbObj.PreventiveAction = dbAction;
                        dbObj.FormStatus = 1;
                    }
                    else
                    {
                        var dbAction = repoAction.Get(d => d.Id == dbObj.PreventiveActionId);
                        if (dbAction != null)
                        {
                            var details = dbAction.PreventiveActionDetail.ToArray();
                            foreach (var item in details)
                            {
                                repoActionDetail.Delete(item);
                            }

                            repoAction.Delete(dbAction);
                        }

                        dbObj.PreventiveActionId = null;
                        dbObj.FormStatus = 0;
                    }

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

        public BusinessResult ToggleCloseComplaint(int id, int? closingUserId = null)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<CustomerComplaint>();
                var dbObj = repo.Get(d => d.Id == id);
                if (dbObj != null)
                {
                    if (dbObj.FormStatus == 2)
                    {
                        if (dbObj.PreventiveActionId != null)
                            dbObj.FormStatus = 1;
                        else
                            dbObj.FormStatus = 0;
                    }
                    else
                    {
                        dbObj.FormStatus = 2;
                    }

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
