using Heka.DataAccess.Context;
using Heka.DataAccess.UnitOfWork;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
                var repoSbsc = uof.GetRepository<UserRoleSubscription>();
                var repoUsers = uof.GetRepository<User>();

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
                else if (model.NotifyType == (int)NotifyType.ItemOrderIsApproved)
                {
                    var dbWaitinfNotifications = repo.Filter(d => d.NotifyType == (int)NotifyType.ItemOrderWaitForApproval
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
                else if (model.UserId == null) // BROADCAST MESSAGES BY SUBSCRIPTION LISTS
                {
                    SubscriptionCategory category = GetSubscriptionCatetory((NotifyType)model.NotifyType);

                    var memberRoles = repoSbsc.Filter(d => d.SubscriptionCategory == (int)category).ToArray();
                    var memberRoleIds = memberRoles
                        .Select(m => m.UserRoleId ?? 0).ToArray();
                    var members = repoUsers.Filter(d => memberRoleIds.Contains(d.UserRoleId ?? 0)).ToArray();

                    // FIRST EDIT DBOBJ.RECEIVER THEN GO ON WITH OTHER MEMBERS
                    int memberIndex = 0;
                    foreach (var member in members)
                    {
                        var newNotificaiton = new Notification();
                        model.MapTo(newNotificaiton);
                        newNotificaiton.UserId = member.Id;
                        repo.Add(newNotificaiton);

                        // send auto email
                        if (!string.IsNullOrEmpty(member.EmailAddr))
                        {
                            try
                            {
                                var smtpClient = new SmtpClient("smtp-mail.outlook.com", 587);
                                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                                smtpClient.UseDefaultCredentials = false;
                                smtpClient.Credentials = new NetworkCredential("btsmesheka@outlook.com", "Bts2021.*");
                                smtpClient.EnableSsl = true;

                                smtpClient.Send("btsmesheka@outlook.com", member.EmailAddr, model.Title, model.Message);
                            }
                            catch (Exception ex)
                            {

                            }
                        }

                        memberIndex++;
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

        protected SubscriptionCategory GetSubscriptionCatetory(NotifyType notifyType)
        {
            switch (notifyType)
            {
                case NotifyType.ItemRequestWaitForApproval:
                    return SubscriptionCategory.ItemRequest;
                case NotifyType.ItemRequestIsApproved:
                    return SubscriptionCategory.ItemRequest;
                case NotifyType.ItemOrderWaitForApproval:
                    return SubscriptionCategory.ItemOrder;
                case NotifyType.ItemOrderIsApproved:
                    return SubscriptionCategory.ItemOrder;
                case NotifyType.IncidentCreated:
                    return SubscriptionCategory.Incident;
                case NotifyType.PostureCreated:
                    return SubscriptionCategory.Posture;
                case NotifyType.IncidentTouched:
                    return SubscriptionCategory.Incident;
                case NotifyType.IncidentResolved:
                    return SubscriptionCategory.Incident;
                case NotifyType.PostureTouched:
                    return SubscriptionCategory.Posture;
                case NotifyType.PostureResolved:
                    return SubscriptionCategory.Posture;
                case NotifyType.ItemAtMinimumInWarehouse:
                    return SubscriptionCategory.ItemCriticals;
                case NotifyType.ItemAtMaximumInWarehouse:
                    return SubscriptionCategory.ItemCriticals;
                case NotifyType.ProductPickupComplete:
                    return SubscriptionCategory.ProductPickup;
                case NotifyType.ManuelProductLabelPrinted:
                    return SubscriptionCategory.ProductPickup;
                default:
                    return default;
            }
        }

        public SystemParameterModel GetParameter(string prmCode, int plantId)
        {
            var repo = _unitOfWork.GetRepository<SystemParameter>();
            var dbObj = repo.Get(d => d.PrmCode == prmCode && d.PlantId == plantId);
            if (dbObj != null)
            {
                return new SystemParameterModel
                {
                    Id = dbObj.Id,
                    PlantId = dbObj.PlantId,
                    PrmCode = dbObj.PrmCode,
                    PrmValue = dbObj.PrmValue,
                };
            }

            return null;
        }

        protected BusinessResult CreateDefaultParams(int plantId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var uof = new EFUnitOfWork();
                var repo = uof.GetRepository<SystemParameter>();

                string[] defParams = new string[] { "DefaultProductPrinter" };

                foreach (var prm in defParams)
                {
                    var dbObj = repo.Get(d => d.PrmCode == prm && d.PlantId == plantId);
                    if (dbObj == null)
                    {
                        dbObj = new SystemParameter
                        {
                            PlantId = plantId,
                            PrmCode = prm,
                            PrmValue = "",
                        };
                        repo.Add(dbObj);
                    }
                }

                uof.SaveChanges();

                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public SystemParameterModel[] GetAllParameters(int plantId)
        {
            SystemParameterModel[] data = new SystemParameterModel[0];

            try
            {
                // CREATE DEFAULT SYSTEM PARAMETERS IF NOT EXISTS
                CreateDefaultParams(plantId);

                var repo = _unitOfWork.GetRepository<SystemParameter>();
                data = repo.Filter(d => d.PlantId == plantId)
                    .Select(d => new SystemParameterModel
                    {
                        Id = d.Id,
                        PrmCode = d.PrmCode,
                        PrmValue = d.PrmValue,
                        PlantId = d.PlantId,
                    }).ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }

        public BusinessResult SetParameters(SystemParameterModel[] model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<SystemParameter>();

                foreach (var item in model)
                {
                    var dbPrm = repo.Get(d => d.PrmCode == item.PrmCode && d.PlantId == item.PlantId);
                    if (dbPrm == null)
                    {
                        dbPrm = new SystemParameter
                        {
                            PrmCode = item.PrmCode,
                            PlantId = item.PlantId,
                        };
                        repo.Add(dbPrm);
                    }

                    dbPrm.PrmValue = item.PrmValue;
                }

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
        #endregion
    }
}
