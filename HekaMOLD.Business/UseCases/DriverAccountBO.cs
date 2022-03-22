using Heka.DataAccess.Context;
using HekaMOLD.Business.Base;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Logistics;
using System.Collections.Generic;
using System.Linq;

namespace HekaMOLD.Business.UseCases
{
    public class DriverAccountBO : IBusinessObject
    {
        #region DRIVERACCOUNT BUSINESS

        public DriverAccountModel[] GetDriverAccountList()
        {
            List<DriverAccountModel> data = new List<DriverAccountModel>();

            var repo = _unitOfWork.GetRepository<DriverAccount>();

            repo.GetAll().ToList().ForEach(d =>
            {
                DriverAccountModel containerObj = new DriverAccountModel();
                d.MapTo(containerObj);
                containerObj.DriverNameAndSurName = d.Driver != null ? d.Driver.DriverName + " " + d.Driver.DriverSurName : "";
                containerObj.ForexTypeCode = d.ForexType != null ? d.ForexType.ForexTypeCode : "";
                data.Add(containerObj);
            });

            return data.ToArray();
        }
        public DriverAccountModel GetDriverAccount(int id)
        {
            DriverAccountModel model = new DriverAccountModel();

            var repo = _unitOfWork.GetRepository<DriverAccount>();
            var repoDetail = _unitOfWork.GetRepository<DriverAccountDetail>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.DriverNameAndSurName = dbObj.Driver != null ? dbObj.Driver.DriverName + " " + dbObj.Driver.DriverSurName:"";
                model.ForexTypeCode = dbObj.ForexType != null ? dbObj.ForexType.ForexTypeCode : "";
                model.DriverAccountDetails = repoDetail.Filter(d => d.DriverAccountId == dbObj.Id).Select(d => new DriverAccountDetailModel
                {
                    Id = d.Id,
                    ActionTypeStr = d.ActionType == (int)ActionType.Entry ? "Giriş" : d.ActionType == (int)ActionType.Exit ? "Çıkış":"",
                    CostCategoryId = d.CostCategoryId,
                    DocumentNo = d.DocumentNo,
                    DriverAccountId = d.DriverAccountId,
                    DriverNameAndSurName = d.Driver != null ? d.Driver.DriverName +" "+ d.Driver.DriverSurName :"",
                    Explanation =d.Explanation,
                    ForexTypeId  = d.ForexTypeId,
                    OverallTotal = d.OverallTotal,
                    VoyageCostDetailId = d.VoyageCostDetailId,
                    DriverId = d.DriverId,
                    ActionType = d.ActionType,
                    VoyageId = d.VoyageId,
                    VoyageCode = d.Voyage !=null ? d.Voyage.VoyageCode:"",
                    TowingVehicleId = d.TowingVehicleId,
                    CountryId = d.CountryId,
                    KmHour = d.KmHour,
                    Quantity = d.Quantity,
                    UnitTypeId = d.UnitTypeId,
                    OperationDate = d.OperationDate,
              
                }).ToArray();
            }

            return model;
        }

        #endregion
    }
}
