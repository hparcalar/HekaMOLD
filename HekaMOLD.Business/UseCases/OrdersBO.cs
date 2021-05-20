using Heka.DataAccess.Context;
using HekaMOLD.Business.Base;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Order;
using HekaMOLD.Business.Models.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.UseCases
{
    public class OrdersBO : IBusinessObject
    {
        public ItemOrderModel[] GetItemOrderList()
        {
            List<ItemOrderModel> data = new List<ItemOrderModel>();

            var repo = _unitOfWork.GetRepository<ItemOrder>();

            repo.GetAll().ToList().ForEach(d =>
            {
                ItemOrderModel containerObj = new ItemOrderModel();
                d.MapTo(containerObj);
                containerObj.OrderStatusStr = ((OrderStatusType)d.OrderStatus.Value).ToCaption();
                containerObj.CreatedDateStr = string.Format("{0:dd.MM.yyyy}", d.CreatedDate);
                containerObj.DateOfNeedStr = string.Format("{0:dd.MM.yyyy}", d.DateOfNeed);
                containerObj.FirmCode = d.Firm != null ? d.Firm.FirmCode : "";
                containerObj.FirmName = d.Firm != null ? d.Firm.FirmName : "";
                containerObj.WarehouseCode = d.Warehouse != null ? d.Warehouse.WarehouseCode : "";
                containerObj.WarehouseName = d.Warehouse != null ? d.Warehouse.WarehouseName : "";

                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateItemOrder(ItemOrderModel model)
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
                    dbObj.OrderNo = GetNextOrderNo(model.PlantId.Value);
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
                                ItemOrder = dbObj
                            };

                            repoDetail.Add(dbDetail);
                        }

                        item.MapTo(dbDetail);
                        dbDetail.ItemOrder = dbObj;
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

        public BusinessResult DeleteItemOrder(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemOrder>();
                var repoDetail = _unitOfWork.GetRepository<ItemOrderDetail>();
                var repoNotify = _unitOfWork.GetRepository<Notification>();

                var dbObj = repo.Get(d => d.Id == id);
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

        public string GetNextOrderNo(int plantId)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<ItemOrder>();
                string lastReceiptNo = repo.Filter(d => d.PlantId == plantId)
                    .OrderByDescending(d => d.OrderNo)
                    .Select(d => d.OrderNo)
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

                dbObj.OrderStatus = (int)RequestStatusType.Approved;
                dbObj.UpdatedDate = DateTime.Now;
                dbObj.UpdatedUserId = userId;

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
    }
}
