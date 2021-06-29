using Heka.DataAccess.Context;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.UseCases
{
    public class MoldBO : CoreProductionBO
    {
        #region MOLD TEST BUSINESS
        public MoldTestModel[] GetMoldTestList()
        {
            List<MoldTestModel> data = new List<MoldTestModel>();

            var repo = _unitOfWork.GetRepository<MoldTest>();

            repo.GetAll().ToList().ForEach(d =>
            {
                MoldTestModel containerObj = new MoldTestModel();
                d.MapTo(containerObj);
                containerObj.MachineCode = d.Machine != null ? d.Machine.MachineCode : "";
                containerObj.MachineName = d.Machine != null ? d.Machine.MachineName : "";
                containerObj.TestDateStr = string.Format("{0:dd.MM.yyyy}", d.TestDate);
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateMoldTest(MoldTestModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if ((model.MachineId ?? 0) == 0)
                    throw new Exception("Makine seçilmelidir.");
                if (model.MoldId == null && string.IsNullOrEmpty(model.MoldCode))
                    throw new Exception("Kalıp bilgisi girilmelidir.");

                var repo = _unitOfWork.GetRepository<MoldTest>();

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new MoldTest();
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

        public BusinessResult DeleteMoldTest(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<MoldTest>();

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

        public MoldTestModel GetMoldTest(int id)
        {
            MoldTestModel model = new MoldTestModel { };

            var repo = _unitOfWork.GetRepository<MoldTest>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.TestDateStr = string.Format("{0:dd.MM.yyyy}", model.TestDate);
            }

            return model;
        }

        #endregion
    }
}
