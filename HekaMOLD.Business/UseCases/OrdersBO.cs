﻿using Heka.DataAccess.Context;
using HekaMOLD.Business.Base;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Order;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.UseCases
{
    public class OrdersBO : CoreReceiptsBO
    {
        public ItemOrderModel[] GetItemOrderList(ItemOrderType orderType)
        {
            List<ItemOrderModel> data = new List<ItemOrderModel>();

            var repo = _unitOfWork.GetRepository<ItemOrder>();

            return repo.Filter(d => d.OrderType == (int)orderType).ToList()
                .Select(d => new ItemOrderModel
                {
                    Id = d.Id,
                    OrderStatusStr = ((OrderStatusType)d.OrderStatus.Value).ToCaption(),
                    CreatedDateStr = string.Format("{0:dd.MM.yyyy}", d.CreatedDate),
                    DateOfNeedStr = string.Format("{0:dd.MM.yyyy}", d.DateOfNeed),
                    FirmCode = d.Firm != null ? d.Firm.FirmCode : "",
                    FirmName = d.Firm != null ? d.Firm.FirmName : "",
                    WarehouseCode = d.Warehouse != null ? d.Warehouse.WarehouseCode : "",
                    WarehouseName = d.Warehouse != null ? d.Warehouse.WarehouseName : "",
                    OrderNo = d.OrderNo,
                    OrderDate = d.OrderDate,
                    DocumentNo = d.DocumentNo,
                    Explanation = d.Explanation,
                    FirmId = d.FirmId,
                    OrderStatus = d.OrderStatus,
                    OrderType = d.OrderType,
                    PlantId = d.PlantId,
                }).ToArray();
        }

        public BusinessResult SaveOrUpdateItemOrder(ItemOrderModel model, bool detailCanBeNull=false)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemOrder>();
                var repoDetail = _unitOfWork.GetRepository<ItemOrderDetail>();
                var repoRequestDetail = _unitOfWork.GetRepository<ItemRequestDetail>();
                var repoNotify = _unitOfWork.GetRepository<Notification>();

                bool newRecord = false;
                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new ItemOrder();
                    dbObj.OrderNo = GetNextOrderNo(model.PlantId.Value, (ItemOrderType)model.OrderType.Value);
                    dbObj.CreatedDate = DateTime.Now;
                    dbObj.CreatedUserId = model.CreatedUserId;
                    dbObj.OrderStatus = (int)OrderStatusType.Created;
                    repo.Add(dbObj);
                    newRecord = true;
                }

                var crDate = dbObj.CreatedDate;
                var donDate = dbObj.DateOfNeed;
                var reqStats = dbObj.OrderStatus;
                var crUserId = dbObj.CreatedUserId;

                model.MapTo(dbObj);

                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = crDate;
                if (dbObj.DateOfNeed == null)
                    dbObj.DateOfNeed = donDate;
                if (dbObj.OrderStatus == null)
                    dbObj.OrderStatus = reqStats;
                if (dbObj.CreatedUserId == null)
                    dbObj.CreatedUserId = crUserId;

                dbObj.UpdatedDate = DateTime.Now;

                #region SAVE DETAILS
                if (model.Details == null && detailCanBeNull == false)
                    throw new Exception("Detay bilgisi olmadan sipariş kaydedilemez.");

                foreach (var item in model.Details)
                {
                    if (item.NewDetail)
                        item.Id = 0;
                }

                if (dbObj.OrderStatus != (int)OrderStatusType.Completed &&
                    dbObj.OrderStatus != (int)OrderStatusType.Cancelled)
                {
                    var newDetailIdList = model.Details.Select(d => d.Id).ToArray();
                    var deletedDetails = dbObj.ItemOrderDetail.Where(d => !newDetailIdList.Contains(d.Id)).ToArray();
                    foreach (var item in deletedDetails)
                    {
                        if (item.ItemReceiptDetail.Any())
                            throw new Exception("İrsaliyesi girilmiş olan bir sipariş detayı silinemez.");

                        #region SET REQUEST & DETAIL TO APPROVED
                        if (item.ItemRequestDetail != null)
                        {
                            item.ItemRequestDetail.RequestStatus = (int)RequestStatusType.Approved;
                            item.ItemRequestDetail.ItemRequest.RequestStatus = (int)RequestStatusType.Approved;
                        }
                        #endregion

                        repoDetail.Delete(item);
                    }

                    int lineNo = 1;
                    foreach (var item in model.Details)
                    {
                        var dbDetail = repoDetail.Get(d => d.Id == item.Id);
                        if (dbDetail == null)
                        {
                            dbDetail = new ItemOrderDetail
                            {
                                ItemOrder = dbObj,
                                OrderStatus = dbObj.OrderStatus
                            };

                            repoDetail.Add(dbDetail);
                        }

                        item.MapTo(dbDetail);
                        dbDetail.ItemOrder = dbObj;
                        dbDetail.OrderStatus = dbObj.OrderStatus;
                        if (dbObj.Id > 0)
                            dbDetail.ItemOrderId = dbObj.Id;

                        dbDetail.LineNumber = lineNo;

                        #region SET REQUEST & DETAIL STATUS TO COMPLETE
                        if (dbDetail.ItemRequestDetailId > 0)
                        {
                            var dbRequestDetail = repoRequestDetail.Get(d => d.Id == dbDetail.ItemRequestDetailId);
                            if (dbRequestDetail != null)
                            {
                                dbRequestDetail.RequestStatus = (int)RequestStatusType.Completed;

                                if (!dbRequestDetail.ItemRequest
                                    .ItemRequestDetail.Any(d => d.RequestStatus != (int)RequestStatusType.Completed))
                                {
                                    dbRequestDetail.ItemRequest.RequestStatus = (int)RequestStatusType.Completed;
                                }
                            }
                        }
                        #endregion

                        lineNo++;
                    }
                }
                #endregion

                _unitOfWork.SaveChanges();

                #region CREATE NOTIFICATION
                if (newRecord || !repoNotify.Any(d => d.RecordId == dbObj.Id && d.NotifyType == (int)NotifyType.ItemOrderWaitForApproval))
                {
                    var repoUser = _unitOfWork.GetRepository<User>();
                    var itemRequestApprovalOwners = repoUser.Filter(d => d.UserRole != null &&
                        d.UserRole.UserAuth.Any(m => m.UserAuthType.AuthTypeCode == "POApproval" && m.IsGranted == true)).ToArray();

                    foreach (var poOWNER in itemRequestApprovalOwners)
                    {
                        base.CreateNotification(new Models.DataTransfer.Core.NotificationModel
                        {
                            IsProcessed = false,
                            Message = string.Format("{0:dd.MM.yyyy}", dbObj.OrderDate)
                            + " yeni bir satınalma siparişi oluşturuldu. Onayınız bekleniyor.",
                            Title = NotifyType.ItemOrderWaitForApproval.ToCaption(),
                            NotifyType = (int)NotifyType.ItemOrderWaitForApproval,
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

        public BusinessResult AddOrderDetail(int orderId, ItemOrderDetailModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repoOrder = _unitOfWork.GetRepository<ItemOrder>();
                var repoOrderDetail = _unitOfWork.GetRepository<ItemOrderDetail>();

                var dbOrder = repoOrder.Get(d => d.Id == orderId);
                if (dbOrder == null)
                    throw new Exception("Sipariş bilgisi HEKA yazılımında bulunamadı.");

                var dbNewDetail = new ItemOrderDetail();
                model.MapTo(dbNewDetail);
                dbNewDetail.ItemOrder = dbOrder;
                repoOrderDetail.Add(dbNewDetail);

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

        public BusinessResult DeleteItemOrder(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemOrder>();
                var repoDetail = _unitOfWork.GetRepository<ItemOrderDetail>();
                var repoNotify = _unitOfWork.GetRepository<Notification>();

                var dbObj = repo.Get(d => d.Id == id);
                if (dbObj == null)
                    throw new Exception("Silinmesi istenen sipariş kaydına ulaşılamadı.");

                if (dbObj.ItemReceipt.Any())
                    throw new Exception("İrsaliyesi girilmiş olan bir sipariş silinemez.");

                // CLEAR DETAILS
                if (dbObj.ItemOrderDetail.Any())
                {
                    var detailObjArr = dbObj.ItemOrderDetail.ToArray();
                    foreach (var item in detailObjArr)
                    {
                        #region SET REQUEST & DETAIL TO APPROVED
                        if (item.ItemRequestDetail != null)
                        {
                            item.ItemRequestDetail.RequestStatus = (int)RequestStatusType.Approved;
                            item.ItemRequestDetail.ItemRequest.RequestStatus = (int)RequestStatusType.Approved;
                        }
                        #endregion

                        repoDetail.Delete(item);
                    }
                }

                // CLEAR NOTIFICATIONS
                if (repoNotify.Any(d => d.NotifyType == (int)NotifyType.ItemOrderWaitForApproval && d.RecordId == dbObj.Id))
                {
                    var notificationArr = repoNotify.Filter(d => d.NotifyType == (int)NotifyType.ItemOrderWaitForApproval && d.RecordId == dbObj.Id)
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

        public ItemOrderModel GetItemOrder(int id)
        {
            ItemOrderModel model = new ItemOrderModel { Details = new ItemOrderDetailModel[0] };

            var repo = _unitOfWork.GetRepository<ItemOrder>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.DateOfNeedStr = string.Format("{0:dd.MM.yyyy}", dbObj.DateOfNeed);
                model.OrderDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.OrderDate);
                model.OrderStatusStr = ((OrderStatusType)model.OrderStatus).ToCaption();
                model.FirmCode = dbObj.Firm != null ? dbObj.Firm.FirmCode : "";
                model.FirmName = dbObj.Firm != null ? dbObj.Firm.FirmName : "";
                model.WarehouseCode = dbObj.Warehouse != null ? dbObj.Warehouse.WarehouseCode : "";
                model.WarehouseName = dbObj.Warehouse != null ? dbObj.Warehouse.WarehouseName : "";

                List<ItemOrderDetailModel> detailContainers = new List<ItemOrderDetailModel>();
                dbObj.ItemOrderDetail.ToList().ForEach(d =>
                {
                    ItemOrderDetailModel detailContainerObj = new ItemOrderDetailModel();
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

        public ItemOrderModel GetItemOrder(string documentNo, ItemOrderType orderType)
        {
            ItemOrderModel model = new ItemOrderModel { Details = new ItemOrderDetailModel[0] };

            var repo = _unitOfWork.GetRepository<ItemOrder>();
            var dbObj = repo.Get(d => d.DocumentNo == documentNo && d.OrderType == (int)orderType);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.DateOfNeedStr = string.Format("{0:dd.MM.yyyy}", dbObj.DateOfNeed);
                model.OrderDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.OrderDate);
                model.OrderStatusStr = ((OrderStatusType)model.OrderStatus).ToCaption();
                model.FirmCode = dbObj.Firm != null ? dbObj.Firm.FirmCode : "";
                model.FirmName = dbObj.Firm != null ? dbObj.Firm.FirmName : "";
                model.WarehouseCode = dbObj.Warehouse != null ? dbObj.Warehouse.WarehouseCode : "";
                model.WarehouseName = dbObj.Warehouse != null ? dbObj.Warehouse.WarehouseName : "";

                List<ItemOrderDetailModel> detailContainers = new List<ItemOrderDetailModel>();
                dbObj.ItemOrderDetail.ToList().ForEach(d =>
                {
                    ItemOrderDetailModel detailContainerObj = new ItemOrderDetailModel();
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

        public ItemOrderDetailModel CalculateOrderDetail(ItemOrderDetailModel model)
        {
            if (model.ForexId > 0 && model.ForexRate > 0)
            {
                model.ForexUnitPrice = model.UnitPrice / model.ForexRate;
            }

            decimal? overallTotal = 0;
            decimal? taxExtractedUnitPrice = 0;

            if (model.TaxIncluded == true)
            {
                decimal? taxIncludedUnitPrice = (model.UnitPrice / (1 + (model.TaxRate / 100m)));
                overallTotal = taxIncludedUnitPrice * model.Quantity;
                taxExtractedUnitPrice = taxIncludedUnitPrice;
            }
            else
            {
                overallTotal = model.UnitPrice * model.Quantity;
                taxExtractedUnitPrice = model.UnitPrice;
            }

            model.OverallTotal = overallTotal;
            model.TaxAmount = overallTotal * model.TaxRate / 100.0m;

            return model;
        }

        public BusinessResult ApproveItemOrderPrice(int id, int userId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemOrder>();

                var dbObj = repo.Get(d => d.Id == id);
                if (dbObj == null)
                    throw new Exception("Onaylanması beklenen satınalma sipariş kaydına ulaşılamadı.");

                if (dbObj.OrderStatus != (int)OrderStatusType.Created)
                    throw new Exception("Onay bekleyen durumunda olmayan bir sipariş onaylanamaz.");

                dbObj.OrderStatus = (int)OrderStatusType.Approved;
                dbObj.UpdatedDate = DateTime.Now;
                dbObj.UpdatedUserId = userId;

                foreach (var item in dbObj.ItemOrderDetail)
                {
                    item.OrderStatus = (int)OrderStatusType.Approved;
                }

                _unitOfWork.SaveChanges();

                #region CREATE NOTIFICATIONS
                base.CreateNotification(new Models.DataTransfer.Core.NotificationModel
                {
                    IsProcessed = false,
                    Message = string.Format("{0:dd.MM.yyyy}", dbObj.DateOfNeed)
                            + " tarihinde oluşturduğunuz satınalma siparişi onaylandı.",
                    Title = NotifyType.ItemOrderIsApproved.ToCaption(),
                    NotifyType = (int)NotifyType.ItemOrderIsApproved,
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

        public bool HasAnySaleOrder(string documentNo)
        {
            var repo = _unitOfWork.GetRepository<ItemOrder>();
            return repo.Any(d => d.DocumentNo == documentNo && d.OrderType == (int)ItemOrderType.Sale);
        }

        public bool HasAnySaleOrderBySyncKey(string syncKey)
        {
            var repo = _unitOfWork.GetRepository<ItemOrder>();
            return repo.Any(d => d.SyncKey == syncKey);
        }

        #region ORDER PRESENTATION
        public ItemOrderModel[] GetRelatedOrders(int receiptId)
        {
            ItemOrderModel[] data = new ItemOrderModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceipt>();
                var repoUser = _unitOfWork.GetRepository<User>();

                var dbObj = repo.GetById(receiptId);
                if (dbObj == null)
                    throw new Exception("İrsaliye bilgisine ulaşılamadı.");

                List<ItemOrder> relatedOrders = new List<ItemOrder>();

                foreach (var item in dbObj.ItemReceiptDetail)
                {
                    if (item.ItemOrderDetail != null
                        && !relatedOrders.Any(d => d.Id == item.ItemOrderDetail.ItemOrderId))
                        relatedOrders.Add(item.ItemOrderDetail.ItemOrder);
                }

                List<ItemOrderModel> sumData = new List<ItemOrderModel>();
                relatedOrders.ForEach(d =>
                {
                    ItemOrderModel model = new ItemOrderModel();
                    d.MapTo(model);

                    if (d.CreatedUserId != null)
                    {
                        var dbUser = repoUser.GetById(d.CreatedUserId.Value);
                        if (dbUser != null)
                        {
                            model.CreatedUserName = dbUser.UserName;
                        }
                    }

                    model.FirmName = d.Firm != null ? d.Firm.FirmName : "";
                    model.FirmCode = d.Firm != null ? d.Firm.FirmCode : "";
                    model.CreatedDateStr = string.Format("{0:dd.MM.yyyy HH:mm}", d.CreatedDate);
                    model.OrderDateStr = string.Format("{0:dd.MM.yyyy HH:mm}", d.OrderDate);
                    sumData.Add(model);
                });

                data = sumData.ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }

        public ItemOrderDetailModel[] GetApprovedOrderDetails(int plantId)
        {
            ItemOrderDetailModel[] data = new ItemOrderDetailModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<ItemOrderDetail>();

                data = repo.Filter(d => d.ItemOrder.PlantId == plantId &&
                    d.OrderStatus == (int)OrderStatusType.Approved
                    && d.ItemOrder.OrderStatus == (int)OrderStatusType.Approved)
                    .ToList()
                    .Select(d => new ItemOrderDetailModel
                    {
                        Id = d.Id,
                        Quantity = d.Quantity,
                        FirmId = d.ItemOrder.FirmId,
                        FirmCode = d.ItemOrder.Firm != null ?
                            d.ItemOrder.Firm.FirmCode : "",
                        FirmName = d.ItemOrder.Firm != null ?
                            d.ItemOrder.Firm.FirmName : "",
                        OrderDate = d.ItemOrder.OrderDate,
                        OrderDateStr = string.Format("{0:dd.MM.yyyy}", d.ItemOrder.OrderDate),
                        CreatedDate = d.ItemOrder.CreatedDate,
                        Explanation = d.Explanation,
                        OrderExplanation = d.ItemOrder.Explanation,
                        ItemNo = d.Item.ItemNo,
                        ItemName = d.Item.ItemName,
                        ItemId = d.ItemId,
                        OrderNo = d.ItemOrder.OrderNo,
                        LineNumber = d.LineNumber,
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