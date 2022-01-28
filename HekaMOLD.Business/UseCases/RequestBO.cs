using Heka.DataAccess.Context;
using Heka.DataAccess.Context.Models;
using HekaMOLD.Business.Base;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Request;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.UseCases
{
    public class RequestBO : CoreReceiptsBO
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
                containerObj.RequestCategoryName = d.ItemRequestCategory != null ? d.ItemRequestCategory.CategoryName : "";
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
                    dbObj.RequestNo = GetNextRequestNo(model.PlantId.Value);
                    dbObj.CreatedDate = DateTime.Now;
                    dbObj.CreatedUserId = model.CreatedUserId;
                    dbObj.RequestStatus = (int)RequestStatusType.Created;
                    repo.Add(dbObj);
                    newRecord = true;
                }

                var crDate = dbObj.CreatedDate;
                var donDate = dbObj.DateOfNeed;
                var reqStats = dbObj.RequestStatus;
                var crUserId = dbObj.CreatedUserId;

                model.MapTo(dbObj);

                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = crDate;
                if (dbObj.DateOfNeed == null)
                    dbObj.DateOfNeed = donDate;
                if (dbObj.RequestStatus == null)
                    dbObj.RequestStatus = reqStats;
                if (dbObj.CreatedUserId == null)
                    dbObj.CreatedUserId = crUserId;

                dbObj.UpdatedDate = DateTime.Now;

                #region SAVE DETAILS
                if (model.Details == null)
                    throw new Exception("Detay bilgisi olmadan talep kaydedilemez.");

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
                var repoApprLog = _unitOfWork.GetRepository<ItemRequestApproveLog>();

                var dbObj = repo.Get(d => d.Id == id);
                if (dbObj == null)
                    throw new Exception("Silinmesi istenen talep kaydına ulaşılamadı.");

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

                // CLEAR APPROVE LOGS
                if (dbObj.ItemRequestApproveLog.Any())
                {
                    var approveLogArr = dbObj.ItemRequestApproveLog.ToArray();
                    foreach (var item in approveLogArr)
                    {
                        repoApprLog.Delete(item);
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
                model.RequestCategoryName = dbObj.ItemRequestCategory != null ? dbObj.ItemRequestCategory.CategoryName : "";

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

                foreach (var item in dbObj.ItemRequestDetail)
                {
                    item.RequestStatus = dbObj.RequestStatus;
                }

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

        #region REQUEST CATEGORY DEFINITIONS
        public ItemRequestCategoryModel[] GetRequestCategoryList()
        {
            List<ItemRequestCategoryModel> data = new List<ItemRequestCategoryModel>();

            var repo = _unitOfWork.GetRepository<ItemRequestCategory>();

            repo.GetAll().ToList().ForEach(d =>
            {
                ItemRequestCategoryModel containerObj = new ItemRequestCategoryModel();
                d.MapTo(containerObj);
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateRequestCategory(ItemRequestCategoryModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.CategoryName))
                    throw new Exception("Kategori adı girilmelidir.");

                var repo = _unitOfWork.GetRepository<ItemRequestCategory>();

                if (repo.Any(d => (d.CategoryName == model.CategoryName)
                    && d.Id != model.Id))
                    throw new Exception("Aynı isme sahip başka bir kategori mevcuttur. Lütfen farklı bir isim giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new ItemRequestCategory();
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

        public BusinessResult DeleteRequestCategory(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemRequestCategory>();

                var dbObj = repo.Get(d => d.Id == id);
                if (dbObj.ItemRequest.Any())
                    throw new Exception("Bu kategori geçmişte kullanılmış olduğu için silinemez.");

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

        public ItemRequestCategoryModel GetRequestCategory(int id)
        {
            ItemRequestCategoryModel model = new ItemRequestCategoryModel { };

            var repo = _unitOfWork.GetRepository<ItemRequestCategory>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }
        #endregion

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

        public ItemRequestModel[] GetRelatedRequests(int orderId)
        {
            ItemRequestModel[] data = new ItemRequestModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<ItemOrder>();
                var dbObj = repo.GetById(orderId);
                if (dbObj == null)
                    throw new Exception("Sipariş bilgisine ulaşılamadı.");

                List<ItemRequest> relatedRequests = new List<ItemRequest>();

                foreach (var item in dbObj.ItemOrderDetail)
                {
                    if (item.ItemRequestDetail != null 
                        && !relatedRequests.Any(d => d.Id == item.ItemRequestDetail.ItemRequestId))
                        relatedRequests.Add(item.ItemRequestDetail.ItemRequest);
                }

                List<ItemRequestModel> sumData = new List<ItemRequestModel>();
                relatedRequests.ForEach(d =>
                {
                    ItemRequestModel model = new ItemRequestModel();
                    d.MapTo(model);
                    model.CreatedUserName = d.User != null ? d.User.UserName : "";
                    model.CreatedDateStr = string.Format("{0:dd.MM.yyyy HH:mm}", d.CreatedDate);
                    sumData.Add(model);
                });

                data = sumData.ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }
        #endregion

        #region ORDER CONVERSION BUSINESS
        public BusinessResult CreatePurchaseOrder(int requestId, int userId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemRequest>();
                var repoOrder = _unitOfWork.GetRepository<ItemOrder>();
                var repoOrderDetail = _unitOfWork.GetRepository<ItemOrderDetail>();
                var repoNotify = _unitOfWork.GetRepository<Notification>();

                var dbRequest = repo.Get(d => d.Id == requestId);
                if (dbRequest == null)
                    throw new Exception("Talep bilgisine ulaşılamadı.");

                // CREATE ORDER
                var dbOrder = new ItemOrder
                {
                    CreatedDate = DateTime.Now,
                    CreatedUserId = userId,
                    DateOfNeed = dbRequest.DateOfNeed,
                    OrderDate = DateTime.Now,
                    DocumentNo = "",
                    Explanation = dbRequest.Explanation,
                    ItemRequest = dbRequest,
                    OrderNo = GetNextOrderNo(dbRequest.PlantId.Value, ItemOrderType.Purchase),
                    OrderStatus = (int)OrderStatusType.Created,
                    PlantId = dbRequest.PlantId.Value,
                    SubTotal = 0,
                    OverallTotal = 0,
                    TaxPrice = 0
                };
                repoOrder.Add(dbOrder);

                // CHANGE REQUEST STATUS
                dbRequest.RequestStatus = (int)RequestStatusType.Completed;
                dbRequest.UpdatedDate = DateTime.Now;
                dbRequest.UpdatedUserId = userId;

                // CREATE ORDER DETAILS
                foreach (var dbRequestDetail in dbRequest.ItemRequestDetail)
                {
                    var dbOrderDetail = new ItemOrderDetail
                    {
                        CreatedDate = DateTime.Now,
                        CreatedUserId = userId,
                        Explanation = dbRequestDetail.Explanation,
                        Item = dbRequestDetail.Item,
                        ItemOrder = dbOrder,
                        ItemRequestDetail = dbRequestDetail,
                        LineNumber = dbRequestDetail.LineNumber,
                        NetQuantity = dbRequestDetail.NetQuantity,
                        OverallTotal = 0,
                        Quantity = dbRequestDetail.Quantity,
                        SubTotal = 0,
                        TaxIncluded = false, TaxRate = 0, TaxAmount = 0,
                        UnitPrice = 0,
                        OrderStatus = (int)OrderStatusType.Created
                    };
                    repoOrderDetail.Add(dbOrderDetail);

                    // CHANGE REQUEST DETAIL STATUS
                    dbRequestDetail.RequestStatus = (int)RequestStatusType.Completed;
                    dbRequestDetail.UpdatedDate = DateTime.Now;
                    dbRequestDetail.UpdatedUserId = userId;
                }

                _unitOfWork.SaveChanges();

                result.Result = true;
                result.RecordId = dbOrder.Id;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }
        #endregion

        #region LOAD CONVERSION BUSINESS
        public BusinessResult CreateLoad(int orderId, int userId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemOrder>();
                var repoLoad = _unitOfWork.GetRepository<ItemLoad>();
                var repoLoadDetail = _unitOfWork.GetRepository<ItemLoadDetail>();
                var repoNotify = _unitOfWork.GetRepository<Notification>();

                var dbOrder = repo.Get(d => d.Id == orderId);
                if (dbOrder == null)
                    throw new Exception("Sipariş bilgisine ulaşılamadı.");

                // CREATE LOAD
                var dbLoad = new ItemLoad
                {
                    CreatDate = DateTime.Now,
                    OrderNo = dbOrder.OrderNo,
                    CreatedUserId = userId,
                    DateOfNeed = dbOrder.DateOfNeed,
                    LoadOutDate = dbOrder.LoadOutDate,
                    OrderUploadType = dbOrder.OrderUploadType,
                    OrderUploadPointType = dbOrder.OrderUploadPointType,
                    OrderTransactionDirectionType = dbOrder.OrderTransactionDirectionType,
                    OrderCalculationType = dbOrder.OrderCalculationType,
                    CustomerFirmId = dbOrder.CustomerFirmId,
                    EntryCustomsId =dbOrder.EntryCustomsId,
                    ExitCustomsId = dbOrder.ExitCustomsId,
                    OveralLadametre = dbOrder.OveralLadametre,
                    OveralVolume = dbOrder.OveralVolume,
                    OveralWeight = dbOrder.OveralWeight,
                    CalculationTypePrice = dbOrder.CalculationTypePrice,
                    Explanation = dbOrder.Explanation,
                    ItemOrder = dbOrder,
                    LoadCode = "Test",//GetNextOrderNo(dbRequest.PlantId.Value, ItemOrderType.Purchase),
                    LoadStatusType = (int)LoadStatusType.Created,
                    PlantId = dbOrder.PlantId.Value,
                };
                repoLoad.Add(dbLoad);

                // CHANGE REQUEST STATUS
                dbOrder.OrderStatus = (int)OrderStatusType.Loaded;
                dbOrder.UpdatedDate = DateTime.Now;
                dbOrder.UpdatedUserId = userId;

                // CREATE LOAD DETAILS
                foreach (var dbOrderDetail in dbOrder.ItemOrderDetail)
                {
                    var dbLoadDetail = new ItemLoadDetail
                    {
                        CreatedDate = DateTime.Now,
                        CreatedUserId = userId,
                        Item = dbOrderDetail.Item,
                        ItemLoad = dbLoad,
                        Quantity = dbOrderDetail.Quantity,
                        ItemOrderDetail = dbOrderDetail,
                        LineNumber = dbOrderDetail.LineNumber,
                        Height = dbOrderDetail.Height,
                        Ladametre = dbOrderDetail.Ladametre,
                        LongWidth = dbOrderDetail.LongWidth,
                        PackageInNumber = dbOrderDetail.PackageInNumber,
                        ShortWidth = dbOrderDetail.ShortWidth,
                        UnitType = dbOrderDetail.UnitType,
                        Stackable = dbOrderDetail.Stackable,
                        Volume = dbOrderDetail.Volume,
                        Weight = dbOrderDetail.Weight,
                        LoadStatus = (int)LoadStatusType.Created
                    };
                    repoLoadDetail.Add(dbLoadDetail);

                    // CHANGE REQUEST DETAIL STATUS
                    dbOrderDetail.OrderStatus = (int)OrderStatusType.Completed;
                    dbOrderDetail.UpdatedDate = DateTime.Now;
                    dbOrderDetail.UpdatedUserId = userId;
                }

                _unitOfWork.SaveChanges();

                result.Result = true;
                result.RecordId = dbLoad.Id;
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
