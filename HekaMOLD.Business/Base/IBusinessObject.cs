using Heka.DataAccess.Context;
using Heka.DataAccess.UnitOfWork;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Base
{
    public abstract class IBusinessObject : IDisposable
    {
        protected IUnitOfWork _unitOfWork;
        protected bool _autoSave = true;

        public IBusinessObject()
        {
            _unitOfWork = new EFUnitOfWork();
        }

        public IBusinessObject(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool AutoSave
        {
            get { return _autoSave; }
            set { _autoSave = value; }
        }

        public void Dispose()
        {
            if (_unitOfWork != null)
                _unitOfWork.Dispose();

            GC.SuppressFinalize(this);
        }

        #region BASE LOGIC
        public PlantModel[] GetPlantList()
        {
            var repo = _unitOfWork.GetRepository<Plant>();
            List<PlantModel> list = new List<PlantModel>();
            var dataList = repo.GetAll();
            dataList.ToList().ForEach(d => { list.Add(d.MapTo(new PlantModel())); });

            return list.ToArray();
        }

        public BusinessResult CreateNotification(NotificationModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var uof = new EFUnitOfWork();
                var repo = uof.GetRepository<Notification>();

                var dbObj = new Notification();
                model.MapTo(dbObj);

                dbObj.CreatedDate = DateTime.Now;

                repo.Add(dbObj);

                if (model.NotifyType == (int)NotifyType.ItemRequestIsApproved)
                {
                    var dbWaitinfNotifications = repo.Filter(d => d.NotifyType == (int)NotifyType.ItemRequestWaitForApproval
                        && d.RecordId == model.RecordId && (d.IsProcessed ?? false) == false).ToArray();
                    foreach (var item in dbWaitinfNotifications)
                    {
                        if (item.SeenStatus == 0)
                        {
                            item.SeenStatus = 1;
                            item.SeenDate = DateTime.Now;
                        }

                        item.IsProcessed = true;
                        item.ProcessedDate = DateTime.Now;
                    }
                }

                uof.SaveChanges();
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
        #endregion
    }
}
