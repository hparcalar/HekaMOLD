using Heka.DataAccess.Context;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Receipt;
using HekaMOLD.Business.Models.Dictionaries;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.UseCases
{
    public class ReceiptBO : CoreReceiptsBO
    {
        public ItemReceiptModel[] GetItemReceiptList(ReceiptCategoryType receiptCategory,
            ItemReceiptType? receiptType)
        {
            List<ItemReceiptModel> data = new List<ItemReceiptModel>();

            var repo = _unitOfWork.GetRepository<ItemReceipt>();

            repo.Filter(d =>
                d.ReceiptType != null &&
                (receiptCategory == ReceiptCategoryType.All
                ||
                (receiptCategory == ReceiptCategoryType.Purchasing 
                    && DictItemReceiptType.PurchasingTypes.Contains(d.ReceiptType.Value))
                ||
                (receiptCategory == ReceiptCategoryType.ItemManagement
                    && DictItemReceiptType.ItemManagementTypes.Contains(d.ReceiptType.Value))
                ||
                (receiptCategory == ReceiptCategoryType.Sales
                    && DictItemReceiptType.SalesTypes.Contains(d.ReceiptType.Value))
                ||
                (receiptCategory == ReceiptCategoryType.AllInputs
                    && DictItemReceiptType.InputTypes.Contains(d.ReceiptType.Value))
                ||
                (receiptCategory == ReceiptCategoryType.AllOutputs
                    && DictItemReceiptType.OutputTypes.Contains(d.ReceiptType.Value)))
                &&
                (
                    receiptType == null || d.ReceiptType == (int?)receiptType
                )
            )
                .ToList().ForEach(d =>
            {
                ItemReceiptModel containerObj = new ItemReceiptModel();
                d.MapTo(containerObj);
                containerObj.TotalQuantity = d.ItemReceiptDetail.Sum(m => m.Quantity);
                containerObj.ReceiptStatusStr = ((ReceiptStatusType)d.ReceiptStatus.Value).ToCaption();
                containerObj.CreatedDateStr = string.Format("{0:dd.MM.yyyy}", d.CreatedDate);
                containerObj.ReceiptDateStr = string.Format("{0:dd.MM.yyyy}", d.ReceiptDate);
                containerObj.FirmCode = d.Firm != null ? d.Firm.FirmCode : "";
                containerObj.FirmName = d.Firm != null ? d.Firm.FirmName : "";
                containerObj.WarehouseCode = d.Warehouse != null ? d.Warehouse.WarehouseCode : "";
                containerObj.WarehouseName = d.Warehouse != null ? d.Warehouse.WarehouseName : "";
                containerObj.ReceiptTypeStr = ((ItemReceiptType)d.ReceiptType.Value).ToCaption();

                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateItemReceipt(ItemReceiptModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceipt>();
                var repoDetail = _unitOfWork.GetRepository<ItemReceiptDetail>();
                var repoOrderDetail = _unitOfWork.GetRepository<ItemOrderDetail>();
                var repoNotify = _unitOfWork.GetRepository<Notification>();
                var repoSerial = _unitOfWork.GetRepository<WorkOrderSerial>();

                if (model.FirmId == 0)
                    model.FirmId = null;

                bool newRecord = false;
                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new ItemReceipt();
                    dbObj.ReceiptNo = GetNextReceiptNo(model.PlantId.Value, (ItemReceiptType)model.ReceiptType.Value);
                    model.ReceiptNo = dbObj.ReceiptNo;
                    dbObj.CreatedDate = DateTime.Now;
                    dbObj.CreatedUserId = model.CreatedUserId;
                    dbObj.ReceiptStatus = (int)ReceiptStatusType.Created;
                    repo.Add(dbObj);
                    newRecord = true;
                }

                var crDate = dbObj.CreatedDate;
                var donDate = dbObj.ReceiptDate;
                var reqStats = dbObj.ReceiptStatus;
                var crUserId = dbObj.CreatedUserId;
                var plantId = dbObj.PlantId;

                model.MapTo(dbObj);

                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = crDate;
                if (dbObj.ReceiptDate == null)
                    dbObj.ReceiptDate = donDate;
                if (dbObj.ReceiptStatus == null)
                    dbObj.ReceiptStatus = reqStats;
                if (dbObj.CreatedUserId == null)
                    dbObj.CreatedUserId = crUserId;
                if (dbObj.PlantId == null)
                    dbObj.PlantId = plantId;

                dbObj.UpdatedDate = DateTime.Now;

                List<int> itemsMustBeUpdated = new List<int>();

                #region SAVE DETAILS
                if (model.Details == null)
                    throw new Exception("Detay bilgisi olmadan irsaliye kaydedilemez.");

                foreach (var item in model.Details)
                {
                    if (item.NewDetail)
                        item.Id = 0;
                }

                if (dbObj.ReceiptStatus != (int)ReceiptStatusType.Closed)
                {
                    var newDetailIdList = model.Details.Select(d => d.Id).ToArray();
                    var deletedDetails = dbObj.ItemReceiptDetail.Where(d => !newDetailIdList.Contains(d.Id)).ToArray();
                    foreach (var item in deletedDetails)
                    {
                        if (!itemsMustBeUpdated.Any(d => d == item.ItemId))
                            itemsMustBeUpdated.Add(item.ItemId.Value);

                        //if (item.ItemReceiptDetail.Any())
                        //    throw new Exception("İrsaliyesi girilmiş olan bir sipariş detayı silinemez.");

                        #region SET ORDER & DETAIL TO APPROVED
                        if (item.ItemOrderDetail != null)
                        {
                            item.ItemOrderDetail.OrderStatus = (int)OrderStatusType.Approved;
                            item.ItemOrderDetail.ItemOrder.OrderStatus = (int)OrderStatusType.Approved;
                        }
                        #endregion

                        repoDetail.Delete(item);
                    }

                    int lineNo = 1;
                    foreach (var item in model.Details)
                    {
                        if (!itemsMustBeUpdated.Any(d => d == item.ItemId))
                            itemsMustBeUpdated.Add(item.ItemId.Value);

                        var dbDetail = repoDetail.Get(d => d.Id == item.Id);
                        if (dbDetail == null)
                        {
                            dbDetail = new ItemReceiptDetail
                            {
                                ItemReceipt = dbObj
                            };

                            repoDetail.Add(dbDetail);
                        }

                        item.MapTo(dbDetail);
                        dbDetail.ItemReceipt = dbObj;
                        if (dbObj.Id > 0)
                            dbDetail.ItemReceiptId = dbObj.Id;

                        dbDetail.LineNumber = lineNo;

                        #region UPDATE SERIALS
                        if (item.UpdateSerials)
                        {
                            var newSerialIdList = item.Serials.Select(d => d.Id).ToArray();
                            var deletedSerials = dbDetail.WorkOrderSerial.Where(d => !newDetailIdList.Contains(d.Id)).ToArray();
                            foreach (var serialItem in deletedSerials)
                            {
                                if (serialItem.WorkOrderDetail != null)
                                {
                                    serialItem.SerialStatus = (int)SerialStatusType.Created;
                                    serialItem.ItemReceiptDetailId = null;
                                }
                                else
                                {
                                    repoSerial.Delete(serialItem);
                                }
                            }

                            foreach (var serialItem in item.Serials)
                            {
                                var dbSerial = repoSerial.Get(d => d.Id == serialItem.Id);
                                if (dbSerial != null)
                                {
                                    dbSerial.ItemReceiptDetail = dbDetail;
                                }
                            }
                        }
                        #endregion

                        #region CALCULATE IF THERE IS NO NET QUANTITY
                        if (dbDetail.NetQuantity == null)
                        {
                            ItemReceiptDetailModel detailModel = new ItemReceiptDetailModel();
                            dbDetail.MapTo(detailModel);
                            this.CalculateReceiptDetail(detailModel);
                            dbDetail.NetQuantity = detailModel.NetQuantity;
                        }
                        #endregion

                        #region SET ORDER & DETAIL STATUS TO COMPLETE
                        if (dbDetail.ItemOrderDetailId > 0)
                        {
                            var dbOrderDetail = repoOrderDetail.Get(d => d.Id == dbDetail.ItemOrderDetailId);
                            if (dbOrderDetail != null)
                            {
                                dbOrderDetail.OrderStatus = (int)OrderStatusType.Completed;

                                if (!dbOrderDetail.ItemOrder
                                    .ItemOrderDetail.Any(d => d.OrderStatus != (int)OrderStatusType.Completed))
                                {
                                    dbOrderDetail.ItemOrder.OrderStatus = (int)OrderStatusType.Completed;
                                }
                            }
                        }
                        #endregion

                        lineNo++;
                    }
                }
                #endregion

                _unitOfWork.SaveChanges();

                // TRG-POINT-ITEM-STATUS
                if (itemsMustBeUpdated.Count() > 0)
                    base.UpdateItemStats(itemsMustBeUpdated.ToArray());

                #region CREATE NOTIFICATION
                //if (newRecord || !repoNotify.Any(d => d.RecordId == dbObj.Id && d.NotifyType == (int)NotifyType.ItemOrderWaitForApproval))
                //{
                //    var repoUser = _unitOfWork.GetRepository<User>();
                //    var itemRequestApprovalOwners = repoUser.Filter(d => d.UserRole != null &&
                //        d.UserRole.UserAuth.Any(m => m.UserAuthType.AuthTypeCode == "POApproval" && m.IsGranted == true)).ToArray();

                //    foreach (var poOWNER in itemRequestApprovalOwners)
                //    {
                //        base.CreateNotification(new Models.DataTransfer.Core.NotificationModel
                //        {
                //            IsProcessed = false,
                //            Message = string.Format("{0:dd.MM.yyyy}", dbObj.OrderDate)
                //            + " yeni bir satınalma siparişi oluşturuldu. Onayınız bekleniyor.",
                //            Title = NotifyType.ItemOrderWaitForApproval.ToCaption(),
                //            NotifyType = (int)NotifyType.ItemOrderWaitForApproval,
                //            SeenStatus = 0,
                //            RecordId = dbObj.Id,
                //            UserId = poOWNER.Id
                //        });
                //    }
                //}
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

        public BusinessResult DeleteItemReceipt(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceipt>();
                var repoDetail = _unitOfWork.GetRepository<ItemReceiptDetail>();
                var repoNotify = _unitOfWork.GetRepository<Notification>();

                var dbObj = repo.Get(d => d.Id == id);
                if (dbObj == null)
                    throw new Exception("Silinmesi istenen irsaliye kaydına ulaşılamadı.");

                //if (dbObj.ItemReceipt.Any())
                //    throw new Exception("İrsaliyesi girilmiş olan bir sipariş silinemez.");

                List<int> itemsMustBeUpdated = new List<int>();

                // CLEAR DETAILS
                if (dbObj.ItemReceiptDetail.Any())
                {
                    var detailObjArr = dbObj.ItemReceiptDetail.ToArray();
                    foreach (var item in detailObjArr)
                    {
                        if (!itemsMustBeUpdated.Any(d => d == item.ItemId))
                            itemsMustBeUpdated.Add(item.ItemId.Value);

                        #region SET ORDER & DETAIL TO APPROVED
                        if (item.ItemOrderDetail != null)
                        {
                            item.ItemOrderDetail.OrderStatus = (int)OrderStatusType.Approved;
                            item.ItemOrderDetail.ItemOrder.OrderStatus = (int)OrderStatusType.Approved;
                        }
                        #endregion

                        repoDetail.Delete(item);
                    }
                }

                // CLEAR NOTIFICATIONS
                //if (repoNotify.Any(d => d.NotifyType == (int)NotifyType.ItemOrderWaitForApproval && d.RecordId == dbObj.Id))
                //{
                //    var notificationArr = repoNotify.Filter(d => d.NotifyType == (int)NotifyType.ItemOrderWaitForApproval && d.RecordId == dbObj.Id)
                //        .ToArray();

                //    foreach (var item in notificationArr)
                //    {
                //        repoNotify.Delete(item);
                //    }
                //}

                repo.Delete(dbObj);
                _unitOfWork.SaveChanges();

                // TRG-POINT-ITEM-STATUS
                if (itemsMustBeUpdated.Count() > 0)
                    base.UpdateItemStats(itemsMustBeUpdated.ToArray());

                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public ItemReceiptModel GetItemReceipt(int id)
        {
            ItemReceiptModel model = new ItemReceiptModel { Details = new ItemReceiptDetailModel[0] };

            var repo = _unitOfWork.GetRepository<ItemReceipt>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.CreatedDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.CreatedDate);
                model.ReceiptDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.ReceiptDate);
                model.ReceiptStatusStr = ((ReceiptStatusType)model.ReceiptStatus).ToCaption();
                model.FirmCode = dbObj.Firm != null ? dbObj.Firm.FirmCode : "";
                model.FirmName = dbObj.Firm != null ? dbObj.Firm.FirmName : "";
                model.WarehouseCode = dbObj.Warehouse != null ? dbObj.Warehouse.WarehouseCode : "";
                model.WarehouseName = dbObj.Warehouse != null ? dbObj.Warehouse.WarehouseName : "";
                model.ReceiptTypeStr = ((ItemReceiptType)dbObj.ReceiptType.Value).ToCaption();

                List<ItemReceiptDetailModel> detailContainers = new List<ItemReceiptDetailModel>();
                dbObj.ItemReceiptDetail.ToList().ForEach(d =>
                {
                    ItemReceiptDetailModel detailContainerObj = new ItemReceiptDetailModel();
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

        public ItemReceiptDetailModel CalculateReceiptDetail(ItemReceiptDetailModel model)
        {
            var repoItem = _unitOfWork.GetRepository<Item>();
            var repoUnit = _unitOfWork.GetRepository<UnitType>();

            decimal mFactor = 1, dFactor = 1;

            var dbItem = repoItem.Get(d => d.Id == model.ItemId);
            if (dbItem != null)
            {
                var selUnit = dbItem.ItemUnit.FirstOrDefault(d => d.Id == model.UnitId);
                if (selUnit != null)
                {
                    mFactor = selUnit.MultiplierFactor ?? 1;
                    dFactor = selUnit.DividerFactor ?? 1;
                }
            }

            model.NetQuantity = model.Quantity * mFactor / dFactor;

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

        public ItemReceiptDetailModel[] GetItemExtract(int itemId)
        {
            ItemReceiptDetailModel[] extract = new ItemReceiptDetailModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceiptDetail>();
                extract = repo.Filter(d => d.ItemId == itemId).ToList()
                    .Select(d => new ItemReceiptDetailModel
                    {
                        Id = d.Id,
                        ItemReceiptId = d.ItemReceiptId,
                        FirmCode = d.ItemReceipt.Firm != null ? d.ItemReceipt.Firm.FirmCode : "",
                        FirmName = d.ItemReceipt.Firm != null ? d.ItemReceipt.Firm.FirmName : "",
                        WarehouseCode = d.ItemReceipt.Warehouse != null ? d.ItemReceipt.Warehouse.WarehouseCode : "",
                        WarehouseName = d.ItemReceipt.Warehouse != null ? d.ItemReceipt.Warehouse.WarehouseName : "",
                        Quantity = d.Quantity,
                        InQuantity = d.ItemReceipt.ReceiptType < 100 ? d.Quantity : (decimal?)null,
                        OutQuantity = d.ItemReceipt.ReceiptType > 100 ? d.Quantity : (decimal?)null,
                        ReceiptDateStr = d.ItemReceipt.ReceiptDate != null ?
                            string.Format("{0:dd.MM.yyyy}", d.ItemReceipt.ReceiptDate) : "",
                        ReceiptTypeStr = ((ItemReceiptType)d.ItemReceipt.ReceiptType).ToCaption(),
                    }).ToArray();
            }
            catch (Exception)
            {

            }

            return extract;
        }
    }
}
