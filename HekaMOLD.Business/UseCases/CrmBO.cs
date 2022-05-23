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
            }

            return model;
        }
    }
}
