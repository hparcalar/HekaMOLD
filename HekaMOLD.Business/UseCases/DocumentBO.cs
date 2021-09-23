using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HekaMOLD.Business.Models.DataTransfer.Core;
using Heka.DataAccess.Context;
using Heka.DataAccess.UnitOfWork;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.Helpers;

namespace HekaMOLD.Business.UseCases
{
    public class DocumentBO : IBusinessObject
    {
        #region DOCUMENT MANAGEMENT
        public UsageDocumentModel[] GetDocumentList()
        {
            List<UsageDocumentModel> data = new List<UsageDocumentModel>();

            var repo = _unitOfWork.GetRepository<UsageDocument>();

            repo.GetAll().ToList().ForEach(d =>
            {
                UsageDocumentModel containerObj = new UsageDocumentModel();
                d.MapTo(containerObj);
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateDocument(UsageDocumentModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.DocumentTitle))
                    throw new Exception("Talimat başlığı girilmelidir.");

                var repo = _unitOfWork.GetRepository<UsageDocument>();

                if (repo.Any(d => (d.DocumentTitle == model.DocumentTitle)
                    && d.Id != model.Id))
                    throw new Exception("Aynı koda sahip başka bir belge mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new UsageDocument();
                    dbObj.CreatedDate = DateTime.Now;
                    dbObj.CreatedUserId = model.CreatedUserId;
                    repo.Add(dbObj);
                }

                var crDate = dbObj.CreatedDate;

                model.MapTo(dbObj);

                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = crDate;

                dbObj.UpdatedDate = DateTime.Now;

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

        public BusinessResult DeleteDocument(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<UsageDocument>();

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

        public UsageDocumentModel GetDocument(int id)
        {
            UsageDocumentModel model = new UsageDocumentModel{ };

            var repo = _unitOfWork.GetRepository<UsageDocument>();
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
