using Heka.DataAccess.Context;
using HekaMOLD.Business.Base;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Request;
using HekaMOLD.Business.Models.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.UseCases
{
    public class RequestBO : IBusinessObject
    {
        public ItemRequestModel[] GetItemRequestList()
        {
            List<ItemRequestModel> data = new List<ItemRequestModel>();

            var repo = _unitOfWork.GetRepository<ItemRequest>();

            repo.GetAll().ToList().ForEach(d =>
            {
                ItemRequestModel containerObj = new ItemRequestModel();
                d.MapTo(containerObj);
                containerObj.RequestStatusStr = ((RequestStatusType)d.RequestStatus.Value).ToCaption();
                containerObj.CreatedDateStr = string.Format("{0:dd.MM.yyyy}", d.CreatedDate);
                containerObj.DateOfNeedStr = string.Format("{0:dd.MM.yyyy}", d.DateOfNeed);
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateItemRequest(ItemRequestModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemRequest>();
                var repoDetail = _unitOfWork.GetRepository<ItemRequestDetail>();

                bool newRecord = false;
                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new ItemRequest();
                    dbObj.RequestNo = GetNextReceiptNo(model.PlantId.Value);
                    dbObj.CreatedDate = DateTime.Now;
                    dbObj.CreatedUserId = model.CreatedUserId;
                    dbObj.RequestStatus = (int)RequestStatusType.Created;
                    repo.Add(dbObj);
                    newRecord = true;
                }

                var crDate = dbObj.CreatedDate;
                var donDate = dbObj.DateOfNeed;
                var reqStats = dbObj.RequestStatus;

                model.MapTo(dbObj);

                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = crDate;
                if (dbObj.DateOfNeed == null)
                    dbObj.DateOfNeed = donDate;
                if (dbObj.RequestStatus == null)
                    dbObj.RequestStatus = reqStats;

                dbObj.UpdatedDate = DateTime.Now;

                #region SAVE DETAILS
                foreach (var item in model.Details)
                {
                    if (item.NewDetail)
                        item.Id = 0;
                }

                if (dbObj.RequestStatus != (int)RequestStatusType.Completed &&
                    dbObj.RequestStatus != (int)RequestStatusType.Cancelled)
                {
                    var newDetailIdList = model.Details.Select(d => d.Id).ToArray();
                    var deletedDetails = dbObj.ItemRequestDetail.Where(d => !newDetailIdList.Contains(d.Id)).ToArray();
                    foreach (var item in deletedDetails)
                    {
                        if (item.ItemOrderDetail.Any())
                            throw new Exception("Siparişe dönüştürülmüş olan bir talep detayı silinemez.");

                        repoDetail.Delete(item);
                    }

                    int lineNo = 1;
                    foreach (var item in model.Details)
                    {
                        var dbDetail = repoDetail.Get(d => d.Id == item.Id);
                        if (dbDetail == null)
                        {
                            dbDetail = new ItemRequestDetail
                            {
                                ItemRequest = dbObj
                            };

                            repoDetail.Add(dbDetail);
                        }

                        item.MapTo(dbDetail);
                        dbDetail.RequestStatus = dbObj.RequestStatus;
                        dbDetail.ItemRequest = dbObj;
                        if (dbObj.Id > 0)
                            dbDetail.ItemRequestId = dbObj.Id;

                        dbDetail.LineNumber = lineNo;

                        lineNo++;
                    }
                }
                #endregion

                _unitOfWork.SaveChanges();

                #region CREATE NOTIFICATION
                if (newRecord)
                {
                    var repoUser = _unitOfWork.GetRepository<User>();
                    var itemRequestApprovalOwners = repoUser.Filter(d => d.UserRole != null &&
                        d.UserRole.UserAuth.Any(m => m.UserAuthType.AuthTypeCode == "POApproval" && m.IsGranted == true)).ToArray();

                    foreach (var poOWNER in itemRequestApprovalOwners)
                    {
                        base.CreateNotification(new Models.DataTransfer.Core.NotificationModel
                        {
                            IsProcessed = false,
                            Message = string.Format("{0:dd.MM.yyyy}", dbObj.DateOfNeed)
                            + " tarihindeki bir ihtiyaç için yeni satınalma talebi oluşturuldu. Onayınız bekleniyor.",
                            Title = NotifyType.ItemRequestWaitForApproval.ToCaption(),
                            NotifyType = (int)NotifyType.ItemRequestWaitForApproval,
                            SeenStatus = 0,
                            RecordId = dbObj.Id,
                            UserId = poOWNER.Id
                        });
                    }
                }
                #endregion

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

        public BusinessResult DeleteItemRequest(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemRequest>();
                var repoDetail = _unitOfWork.GetRepository<ItemRequestDetail>();
                var repoNotify = _unitOfWork.GetRepository<Notification>();

                var dbObj = repo.Get(d => d.Id == id);
                if (dbObj.ItemOrder.Any())
                    throw new Exception("Siparişe dönüştürülen bir talep silinemez.");

                // CLEAR DETAILS
                if (dbObj.ItemRequestDetail.Any())
                {
                    var detailObjArr = dbObj.ItemRequestDetail.ToArray();
                    foreach (var item in detailObjArr)
                    {
                        repoDetail.Delete(item);
                    }
                }

                // CLEAR NOTIFICATIONS
                if (repoNotify.Any(d => d.NotifyType == (int)NotifyType.ItemRequestWaitForApproval && d.RecordId == dbObj.Id))
                {
                    var notificationArr = repoNotify.Filter(d => d.NotifyType == (int)NotifyType.ItemRequestWaitForApproval && d.RecordId == dbObj.Id)
                        .ToArray();

                    foreach (var item in notificationArr)
                    {
                        repoNotify.Delete(item);
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

        public ItemRequestModel GetItemRequest(int id)
        {
            ItemRequestModel model = new ItemRequestModel { Details = new ItemRequestDetailModel[0] };

            var repo = _unitOfWork.GetRepository<ItemRequest>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.DateOfNeedStr = string.Format("{0:dd.MM.yyyy}", dbObj.DateOfNeed);
                model.RequestStatusStr = ((RequestStatusType)model.RequestStatus).ToCaption();

                List<ItemRequestDetailModel> detailContainers = new List<ItemRequestDetailModel>();
                dbObj.ItemRequestDetail.ToList().ForEach(d =>
                {
                    ItemRequestDetailModel detailContainerObj = new ItemRequestDetailModel();
                    d.MapTo(detailContainerObj);
                    detailContainerObj.ItemNo = d.Item != null ? d.Item.ItemNo : "";
                    detailContainerObj.ItemName = d.Item != null ? d.Item.ItemName : "";
                    detailContainerObj.UnitCode = d.UnitType != null ? d.UnitType.UnitCode : "";
                    detailContainerObj.UnitName = d.UnitType != null ? d.UnitType.UnitName : "";
                    detailContainerObj.NewDetail = false;
                    detailContainers.Add(detailContainerObj);
                });

                model.Details = detailContainers.ToArray();
            }

            return model;
        }

        public BusinessResult ApprovePoRequest(int id, int userId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemRequest>();
                var repoLog = _unitOfWork.GetRepository<ItemRequestApproveLog>();

                var dbObj = repo.Get(d => d.Id == id);
                if (dbObj == null)
                    throw new Exception("Onaylanması beklenen satınalma talebi kaydına ulaşılamadı.");

                if (dbObj.RequestStatus != (int)RequestStatusType.Created)
                    throw new Exception("Onay bekleyen durumunda olmayan bir talep onaylanamaz.");

                dbObj.RequestStatus = (int)RequestStatusType.Approved;
                dbObj.UpdatedDate = DateTime.Now;
                dbObj.UpdatedUserId = userId;

                var dbLog = new ItemRequestApproveLog
                {
                    ActorUserId = userId,
                    CreatedDate = DateTime.Now,
                    NewRequestStatus = (int)RequestStatusType.Approved,
                    OldRequestStatus = (int)RequestStatusType.Created,
                    ItemRequestId = dbObj.Id
                };
                repoLog.Add(dbLog);

                _unitOfWork.SaveChanges();

                #region CREATE NOTIFICATIONS
                base.CreateNotification(new Models.DataTransfer.Core.NotificationModel
                {
                    IsProcessed = false,
                    Message = string.Format("{0:dd.MM.yyyy}", dbObj.DateOfNeed)
                            + " tarihinde oluşturduğunuz satınalma talebi onaylandı.",
                    Title = NotifyType.ItemRequestIsApproved.ToCaption(),
                    NotifyType = (int)NotifyType.ItemRequestIsApproved,
                    SeenStatus = 0,
                    RecordId = dbObj.Id,
                    UserId = dbObj.CreatedUserId
                });
                #endregion

                result.RecordId = dbObj.Id;
                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public string GetNextReceiptNo(int plantId)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<ItemRequest>();
                string lastReceiptNo = repo.Filter(d => d.PlantId == plantId)
                    .OrderByDescending(d => d.RequestNo)
                    .Select(d => d.RequestNo)
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

        #region ITEM REQUEST PRESENTATION
        public ItemRequestDetailModel[] GetApprovedDetails(int plantId)
        {
            ItemRequestDetailModel[] data = new ItemRequestDetailModel[0];
            try
            {
                var repo = _unitOfWork.GetRepository<ItemRequestDetail>();

                data = repo.Filter(d => d.ItemRequest.RequestStatus == (int)RequestStatusType.Approved &&
                    d.RequestStatus == (int)RequestStatusType.Approved)
                    .Select(d => new ItemRequestDetailModel
                    {
                        Id = d.Id,
                        ApprovedQuantity = d.ApprovedQuantity,
                        Quantity = d.Quantity,
                        RequestDate = d.ItemRequest.DateOfNeed,
                        CreatedDate = d.ItemRequest.CreatedDate,
                        CreatedUserStr = d.ItemRequest.User != null ? d.ItemRequest.User.UserName : "",
                        Explanation = d.Explanation,
                        RequestExplanation = d.ItemRequest.Explanation,
                        ItemNo = d.Item.ItemNo,
                        ItemName = d.Item.ItemName,
                        ItemId = d.ItemId,
                        RequestNo = d.ItemRequest.RequestNo,
                        LineNumber = d.LineNumber,
                        ItemRequestId = d.ItemRequestId,
                        UnitId = d.UnitId,
                        UnitCode = d.UnitType != null ? d.UnitType.UnitCode : "",
                        UnitName = d.UnitType != null ? d.UnitType.UnitName : ""
                    }).ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }
        #endregion
    }
}
