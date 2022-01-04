using Heka.DataAccess.Context;
using Heka.DataAccess.UnitOfWork;
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

        public ItemReceiptDetailModel[] GetOpenWarehouseEntries()
        {
            ItemReceiptDetailModel[] data = new ItemReceiptDetailModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceiptDetail>();
                data = repo.Filter(d => d.ReceiptStatus != (int)ReceiptStatusType.Closed
                    && d.ItemReceipt.ReceiptType < 100)
                    .ToList()
                    .Select(d => new ItemReceiptDetailModel
                    {
                        Id = d.Id,
                        ReceiptDateStr = string.Format("{0:dd.MM.yyyy}", d.ItemReceipt.ReceiptDate),
                        WarehouseName = d.ItemReceipt.Warehouse.WarehouseName,
                        ItemName = d.Item != null ? d.Item.ItemName : "",
                        FirmName = d.ItemReceipt.Firm != null ? d.ItemReceipt.Firm.FirmName : "",
                        ReceiptNo = d.ItemReceipt.ReceiptNo,
                        Quantity = d.Quantity,
                    }).ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }

        public bool HasAnySaleReceipt(string documentNo)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceipt>();
                return repo.Any(d => d.DocumentNo == documentNo && d.ReceiptType > 100);
            }
            catch (Exception)
            {

            }

            return false;
        }

        public BusinessResult SaveOrUpdateItemReceipt(ItemReceiptModel model, bool detailCanBeNull = false, bool dontChangeDetails=false)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceipt>();
                var repoDetail = _unitOfWork.GetRepository<ItemReceiptDetail>();
                var repoOrderDetail = _unitOfWork.GetRepository<ItemOrderDetail>();
                var repoNotify = _unitOfWork.GetRepository<Notification>();
                var repoSerial = _unitOfWork.GetRepository<WorkOrderSerial>();
                var repoItemSerial = _unitOfWork.GetRepository<ItemSerial>();

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
                ItemReceiptDetail firstNewDetail = null;

                #region SAVE DETAILS
                if (model.Details == null && detailCanBeNull == false)
                    throw new Exception("Detay bilgisi olmadan irsaliye kaydedilemez.");

                foreach (var item in model.Details)
                {
                    if (item.NewDetail)
                        item.Id = 0;
                }

                ItemReceiptDetail newDbDetail = null;
                if (dbObj.ReceiptStatus != (int)ReceiptStatusType.Closed)
                {
                    if (!dontChangeDetails)
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
                                newDbDetail = dbDetail;

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
                                var newSerialIdList = item.ItemSerials.Select(d => d.Id).ToArray();
                                var deletedSerials = dbDetail.ItemSerial.Where(d => !newDetailIdList.Contains(d.Id)).ToArray();
                                foreach (var serialItem in deletedSerials)
                                {
                                    // IF THERE IS A WORK ORDER, THEN CHANGE STATUS TO APPROVED
                                    if (serialItem.WorkOrderDetail != null && dbObj.ReceiptType == (int)ItemReceiptType.WarehouseInput)
                                    {
                                        var dbWorkOrderSerial = repoSerial.Get(m => m.WorkOrderDetailId == serialItem.WorkOrderDetailId
                                            && m.SerialNo == serialItem.SerialNo);
                                        if (dbWorkOrderSerial != null)
                                        {
                                            dbWorkOrderSerial.SerialStatus = (int)SerialStatusType.Approved;
                                            dbWorkOrderSerial.QualityStatus = (int)QualityStatusType.Waiting;
                                            dbWorkOrderSerial.ItemReceiptDetailId = null;
                                        }
                                    }
                                    else
                                    {
                                        repoItemSerial.Delete(serialItem);
                                    }
                                }

                                foreach (var serialItem in item.ItemSerials)
                                {
                                    var dbSerial = repoItemSerial.Get(d => d.Id == serialItem.Id);
                                    if (dbSerial != null)
                                    {
                                        serialItem.MapTo(dbSerial);
                                    }
                                    else
                                    {
                                        dbSerial = new ItemSerial();
                                        dbSerial.ItemReceiptDetail = dbDetail;
                                        serialItem.MapTo(dbSerial);
                                        repoItemSerial.Add(dbSerial);
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

                            // WILL BE TRIGGER POINT TO CHECK ITEM ORDER STATUS

                            lineNo++;
                        }
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

                if (newDbDetail != null)
                    result.DetailRecordId = newDbDetail.Id;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult UpdateReceiptDetail(ItemReceiptDetailModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repoReceiptDetail = _unitOfWork.GetRepository<ItemReceiptDetail>();

                var dbDetail = repoReceiptDetail.Get(d => d.Id == model.Id);
                if (dbDetail == null)
                    throw new Exception("İrsaliye kalem bilgisi HEKA yazılımında bulunamadı.");

                dbDetail.Quantity = model.Quantity;
                dbDetail.UnitPrice = model.UnitPrice;
                dbDetail.SubTotal = model.SubTotal;
                dbDetail.TaxAmount = model.TaxAmount;
                dbDetail.ForexRate = model.ForexRate;
                dbDetail.ForexId = model.ForexId;

                _unitOfWork.SaveChanges();

                base.UpdateItemStats(new int[] { dbDetail.ItemId.Value });

                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult AddReceiptDetail(int receiptId, ItemReceiptDetailModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repoReceipt = _unitOfWork.GetRepository<ItemReceipt>();
                var repoReceiptDetail = _unitOfWork.GetRepository<ItemReceiptDetail>();

                var dbReceipt = repoReceipt.Get(d => d.Id == receiptId);
                if (dbReceipt == null)
                    throw new Exception("İrsaliye bilgisi HEKA yazılımında bulunamadı.");

                var dbNewDetail = new ItemReceiptDetail();
                model.MapTo(dbNewDetail);
                dbNewDetail.ItemReceipt = dbReceipt;
                repoReceiptDetail.Add(dbNewDetail);

                _unitOfWork.SaveChanges();

                base.UpdateItemStats(new int[] { model.ItemId.Value });

                result.Result = true;
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

        public ItemReceiptModel GetItemReceipt(string documentNo, ItemReceiptType receiptType)
        {
            ItemReceiptModel model = new ItemReceiptModel { Details = new ItemReceiptDetailModel[0] };

            var repo = _unitOfWork.GetRepository<ItemReceipt>();
            var dbObj = repo.Get(d => d.DocumentNo == documentNo && d.ReceiptType == (int)receiptType);
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

        public ItemReceiptModel FindItemEntryReceipt(string documentNo)
        {
            ItemReceiptModel data = new ItemReceiptModel();

            var repo = _unitOfWork.GetRepository<ItemReceipt>();
            var dbReceipt = repo.Get(d => d.DocumentNo == documentNo && d.ReceiptType == (int)ItemReceiptType.ItemBuying);
            if (dbReceipt != null)
                data = GetItemReceipt(dbReceipt.Id);
            else
                data = null;

            return data;
        }

        public ItemReceiptModel GetConsumptionReceipt(int workOrderDetailId)
        {
            ItemReceiptModel data = new ItemReceiptModel();

            var repo = _unitOfWork.GetRepository<ItemReceipt>();
            var dbReceipt = repo.Filter(d => d.WorkOrderDetailId == workOrderDetailId && d.ReceiptType == (int)ItemReceiptType.Consumption)
                .FirstOrDefault();
            if (dbReceipt != null)
                data = GetItemReceipt(dbReceipt.Id);
            else
                data = null;

            return data;
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

        public ItemReceiptDetailModel[] GetOpenItemEntries()
        {
            ItemReceiptDetailModel[] extract = new ItemReceiptDetailModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceiptDetail>();
                extract = repo.Filter(d => d.Item.ItemType != (int)ItemType.Product 
                    && d.ReceiptStatus != (int)ReceiptStatusType.Closed
                    && d.ReceiptStatus != (int)ReceiptStatusType.Blocked
                    && d.Item.ItemType != (int)ItemType.SemiProduct
                    && d.Quantity > 0
                    ).ToList()
                    .Select(d => new ItemReceiptDetailModel
                    {
                        Id = d.Id,
                        ItemReceiptId = d.ItemReceiptId,
                        ItemNo = d.Item.ItemNo,
                        ItemName = d.Item.ItemName,
                        FirmCode = d.ItemReceipt.Firm != null ? d.ItemReceipt.Firm.FirmCode : "",
                        FirmName = d.ItemReceipt.Firm != null ? d.ItemReceipt.Firm.FirmName : "",
                        WarehouseCode = d.ItemReceipt.Warehouse != null ? d.ItemReceipt.Warehouse.WarehouseCode : "",
                        WarehouseName = d.ItemReceipt.Warehouse != null ? d.ItemReceipt.Warehouse.WarehouseName : "",
                        Quantity = d.Quantity - (d.ItemReceiptConsumeByConsumed.Sum(m => m.UsedQuantity) ?? 0),
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

        public ItemReceiptModel[] GetNonSyncProductions()
        {
            ItemReceiptModel[] data = new ItemReceiptModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceipt>();
                data = repo.Filter(d => (d.SyncStatus == null || d.SyncStatus == 0)
                        && d.ReceiptType == (int)ItemReceiptType.WarehouseInput)
                    .ToList()
                    .Select(d => new ItemReceiptModel
                    {
                        Id = d.Id,
                        WarehouseCode = d.Warehouse.WarehouseCode,
                        WarehouseName = d.Warehouse.WarehouseName,
                        DocumentNo = d.DocumentNo,
                        Explanation = d.Explanation,
                        FirmId = d.FirmId,
                        FirmCode = d.Firm != null ? d.Firm.FirmCode : "",
                        FirmName = d.Firm != null ? d.Firm.FirmName : "",
                        ReceiptDate = d.ReceiptDate,
                        ReceiptNo = d.ReceiptNo,
                        ReceiptType = d.ReceiptType,
                        Details = d.ItemReceiptDetail.Where(m => m.SyncStatus == null || m.SyncStatus == 0)
                            .Select(m => new ItemReceiptDetailModel
                            {
                                Id = m.Id,
                                ItemId = m.ItemId,
                                ItemNo = m.Item != null ? m.Item.ItemNo : "",
                                ItemName = m.Item != null ? m.Item.ItemName : "",
                                UnitId = m.UnitId,
                                UnitCode = m.UnitType != null ? m.UnitType.UnitCode : "",
                                UnitName = m.UnitType != null ? m.UnitType.UnitName : "",
                                Quantity = m.Quantity,
                                LineNumber = m.LineNumber,
                                UnitPrice = m.UnitPrice,
                                OverallTotal = m.OverallTotal,
                                SubTotal = m.SubTotal,
                                TaxAmount = m.TaxAmount,
                                TaxIncluded = m.TaxIncluded,
                                TaxRate = m.TaxRate,
                            }).ToArray()
                    }).ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }

        public BusinessResult SignDetailAsSynced(int receiptDetailId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                IUnitOfWork newUof = new EFUnitOfWork();
                var repo = newUof.GetRepository<ItemReceiptDetail>();

                var dbObj = repo.Get(d => d.Id == receiptDetailId);
                if (dbObj == null)
                    throw new Exception("İrsaliye kalem kaydı bulunamadı.");

                dbObj.SyncDate = DateTime.Now;
                dbObj.SyncStatus = 1;

                if (!dbObj.ItemReceipt.ItemReceiptDetail.Any(d => d.SyncStatus != 1))
                {
                    dbObj.ItemReceipt.SyncDate = DateTime.Now;
                    dbObj.ItemReceipt.SyncStatus = 1;
                }

                newUof.SaveChanges();

                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        #region WAREHOUSE COUNTING BUSINESS
        public CountingReceiptModel[] GetCountingReceiptList()
        {
            CountingReceiptModel[] data = new CountingReceiptModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<CountingReceipt>();

                data = repo.GetAll().ToList()
                    .Select(d => new CountingReceiptModel
                    {
                        Id = d.Id,
                        CountingDate = d.CountingDate,
                        CountingDateStr = string.Format("{0:dd.MM.yyyy}", d.CountingDate),
                        ReceiptNo = d.ReceiptNo,
                    }).ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }

        public CountingReceiptModel GetCountingReceipt(int id)
        {
            CountingReceiptModel data = new CountingReceiptModel();

            try
            {
                var repo = _unitOfWork.GetRepository<CountingReceipt>();
                var dbModel = repo.Get(d => d.Id == id);
                if (dbModel != null)
                {
                    dbModel.MapTo(data);
                }
            }
            catch (Exception)
            {

            }

            return data;
        }
        public BusinessResult AddBarcodeToCounting(string barcode, int warehouseId, int decreaseCount=0)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<CountingReceipt>();
                var repoDetail = _unitOfWork.GetRepository<CountingReceiptDetail>();
                var repoSerial = _unitOfWork.GetRepository<CountingReceiptSerial>();
                var repoWorkSerial = _unitOfWork.GetRepository<WorkOrderSerial>();

                var dbOpenCounting = repo.Get(d => (d.CountingStatus ?? 0) == 0);
                if (dbOpenCounting == null)
                    throw new Exception("Başlatılan bir sayım süreci bulunamadı.");

                int itemId = 0;
                decimal quantity = 0;

                // PARSE BARCODE
                if (!barcode.Contains("XX"))
                {
                    var dbWorkSerial = repoWorkSerial.Get(d => d.SerialNo == barcode);
                    if (dbWorkSerial != null)
                    {
                        quantity = dbWorkSerial.FirstQuantity ?? 0;
                        itemId = dbWorkSerial.WorkOrderDetail.ItemId ?? 0;
                    }
                }
                else
                {
                    string[] barcodeParts = System.Text.RegularExpressions.Regex.Split(barcode, "XX");
                    itemId = Convert.ToInt32(barcodeParts[0]);

                    //if (barcodeParts[1].Contains(".") && barcodeParts[1].Contains(","))
                    //    barcodeParts[1] = barcodeParts[1].Replace(".", "").Replace(",", ".");

                    quantity = Decimal.Parse(barcodeParts[1], System.Globalization.NumberStyles.Currency,
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                    quantity -= decreaseCount;
                    if (quantity < 0)
                        quantity = 0;

                    //Convert.ToDecimal(Convert.ToSingle( barcodeParts[1].Replace(",",".") ));
                }

                if (itemId == 0)
                    throw new Exception("Barkoddan ürün bilgisine ulaşılamadı ya da bu bir deneme üretimi barkodudur.");

                // SAVE BARCODE
                var dbSerial = new CountingReceiptSerial
                {
                    ItemId = itemId,
                    Barcode = barcode,
                    Quantity = quantity,
                };
                repoSerial.Add(dbSerial);


                // UPDATE DETAIL
                var relatedDetail = repoDetail.Get(d => d.CountingReceiptId == dbOpenCounting.Id
                    && d.ItemId == itemId && d.WarehouseId == warehouseId);
                if (relatedDetail == null)
                {
                    relatedDetail = new CountingReceiptDetail
                    {
                        CountingReceipt = dbOpenCounting,
                        ItemId = itemId,
                        Quantity = quantity,
                        WarehouseId = warehouseId,
                        PackageQuantity = 1,
                    };
                    repoDetail.Add(relatedDetail);
                }
                else
                {
                    relatedDetail.PackageQuantity++;
                    relatedDetail.Quantity += quantity;
                }

                dbSerial.CountingReceiptDetail = relatedDetail;
                _unitOfWork.SaveChanges();

                result.RecordId = dbSerial.Id;
                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult RemoveBarcodeFromCounting(int serialId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<CountingReceipt>();
                var repoDetail = _unitOfWork.GetRepository<CountingReceiptDetail>();
                var repoSerial = _unitOfWork.GetRepository<CountingReceiptSerial>();

                var dbOpenCounting = repo.Get(d => (d.CountingStatus ?? 0) == 0);
                if (dbOpenCounting == null)
                    throw new Exception("Başlatılan bir sayım süreci bulunamadı.");

                // SAVE BARCODE
                var dbSerial = repoSerial.Get(d => d.Id == serialId);
                if (dbSerial == null)
                    throw new Exception("Barkod kaydı bulunamadı.");

                // UPDATE DETAIL
                var relatedDetail = dbSerial.CountingReceiptDetail;
                if (relatedDetail != null)
                {
                    relatedDetail.PackageQuantity--;
                    relatedDetail.Quantity -= dbSerial.Quantity;

                    if (relatedDetail.Quantity < 0)
                        relatedDetail.Quantity = 0;
                }

                repoSerial.Delete(dbSerial);                
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

        public CountingReceiptDetailModel[] GetActiveCountingDetails(int warehouseId)
        {
            CountingReceiptDetailModel[] data = new CountingReceiptDetailModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<CountingReceipt>();
                var dbOpenCounting = repo.Get(d => (d.CountingStatus ?? 0) == 0);
                if (dbOpenCounting == null)
                    throw new Exception("Başlatılan bir sayım süreci bulunamadı.");

                var repoDetail = _unitOfWork.GetRepository<CountingReceiptDetail>();
                data = repoDetail.Filter(d => d.WarehouseId == warehouseId
                    && d.CountingReceiptId == dbOpenCounting.Id)
                    .Select(d => new CountingReceiptDetailModel { 
                        Id = d.Id,
                        CountingReceiptId = d.CountingReceiptId,
                        ItemId = d.ItemId,
                        ItemNo = d.Item.ItemNo,
                        ItemName = d.Item.ItemName,
                        PackageQuantity = d.CountingReceiptSerial.Count(),
                        Quantity = d.Quantity,
                        WarehouseCode = d.Warehouse.WarehouseCode,
                        WarehouseName = d.Warehouse.WarehouseName,
                        WarehouseId = d.WarehouseId,
                    })
                    .OrderByDescending(d => d.Id)
                    .ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }

        public CountingReceiptDetailModel[] GetActiveCountingDetailsByReceiptId(int receiptId)
        {
            CountingReceiptDetailModel[] data = new CountingReceiptDetailModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<CountingReceipt>();
                var dbOpenCounting = repo.Get(d => d.Id == receiptId);
                if (dbOpenCounting == null)
                    throw new Exception("Başlatılan bir sayım süreci bulunamadı.");

                var repoDetail = _unitOfWork.GetRepository<CountingReceiptDetail>();
                data = repoDetail.Filter(d => d.CountingReceiptId == dbOpenCounting.Id)
                    .ToList()
                    .Select(d => new CountingReceiptDetailModel
                    {
                        Id = d.Id,
                        CountingReceiptId = d.CountingReceiptId,
                        ItemId = d.ItemId,
                        ItemNo = d.Item.ItemNo,
                        ItemName = d.Item.ItemName,
                        PackageQuantity = d.CountingReceiptSerial.Count(),
                        ItemTypeStr = ((ItemType)d.Item.ItemType).ToCaption(),
                        CategoryName = d.Item.ItemCategory != null ? d.Item.ItemCategory.ItemCategoryName : "",
                        GroupName = d.Item.ItemGroup != null ? d.Item.ItemGroup.ItemGroupName : "",
                        Quantity = d.Quantity,
                        WarehouseCode = d.Warehouse.WarehouseCode,
                        WarehouseName = d.Warehouse.WarehouseName,
                        WarehouseId = d.WarehouseId,
                    })
                    .OrderByDescending(d => d.Id)
                    .ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }

        public CountingReceiptSerialModel[] GetActiveCountingSerials(int warehouseId)
        {
            CountingReceiptSerialModel[] data = new CountingReceiptSerialModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<CountingReceipt>();
                var dbOpenCounting = repo.Get(d => (d.CountingStatus ?? 0) == 0);
                if (dbOpenCounting == null)
                    throw new Exception("Başlatılan bir sayım süreci bulunamadı.");

                var repoSerial = _unitOfWork.GetRepository<CountingReceiptSerial>();
                data = repoSerial.Filter(d => d.CountingReceiptDetail.CountingReceiptId == dbOpenCounting.Id
                    && d.CountingReceiptDetail.WarehouseId == warehouseId)
                    .Select(d => new CountingReceiptSerialModel
                    {
                        Id = d.Id,
                        Barcode = d.Barcode,
                        ItemId = d.ItemId,
                        Quantity = d.Quantity,
                        ItemNo = d.Item.ItemNo,
                        ItemName = d.Item.ItemName,
                        CountingReceiptDetailId = d.CountingReceiptDetailId,
                    })
                    .OrderByDescending(d => d.Id)
                    .ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }

        public decimal GetItemQuantity(int itemId)
        {
            decimal quantity = 0;

            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceiptDetail>();

                var entries = repo.Filter(d => d.ItemId == itemId
                    && d.ItemReceipt != null && d.ItemReceipt.ReceiptType < 100)
                    .Select(d => d.Quantity).Sum() ?? 0;

                var deliveries = repo.Filter(d => d.ItemId == itemId
                    && d.ItemReceipt != null && d.ItemReceipt.ReceiptType > 100)
                    .Select(d => d.Quantity).Sum() ?? 0;

                return entries - deliveries;
            }
            catch (Exception)
            {

            }

            return quantity;
        }

        public BusinessResult ApplyCountingToWarehouse(int countingReceiptId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<CountingReceipt>();

                var dbObj = repo.Get(d => d.Id == countingReceiptId);
                if (dbObj == null)
                    throw new Exception("Sayım kaydı bulunamadı.");

                var entryReceipt = new ItemReceiptModel { };
                var deliveryReceipt = new ItemReceiptModel { };
                List<ItemReceiptDetailModel> entryDetails = new List<ItemReceiptDetailModel>();
                List<ItemReceiptDetailModel> deliveryDetails = new List<ItemReceiptDetailModel>();

                var countingDetails = dbObj.CountingReceiptDetail.ToArray();

                foreach (var item in countingDetails)
                {
                    if (item.ItemId > 0)
                    {
                        var currentQuantity = GetItemQuantity(item.ItemId.Value);
                        var countingQuantity = item.Quantity ?? 0;

                        if (countingQuantity > currentQuantity) // will be positive movement
                        {
                            entryDetails.Add(new ItemReceiptDetailModel
                            {
                                ItemId = item.ItemId,
                                LineNumber = entryDetails.Count() + 1,
                                Quantity = countingQuantity - currentQuantity,
                                CreatedDate = DateTime.Now,
                                NetQuantity = countingQuantity - currentQuantity,
                                UnitId = item.Item.ItemUnit.Where(m => m.IsMainUnit == true).Select(m => m.UnitId).FirstOrDefault(),
                                UnitPrice = 0,
                                TaxIncluded = false,
                                TaxRate = 0,
                                TaxAmount = 0,
                                SubTotal = 0,
                                DiscountRate = 0,
                                DiscountAmount = 0,
                                NewDetail = true,
                                OverallTotal = 0,
                                SyncStatus = 1,
                                ReceiptStatus = (int)ReceiptStatusType.Created,
                            });
                        }
                        else if (countingQuantity < currentQuantity) // will be negative movement
                        {
                            deliveryDetails.Add(new ItemReceiptDetailModel
                            {
                                ItemId = item.ItemId,
                                LineNumber = entryDetails.Count() + 1,
                                Quantity = currentQuantity - countingQuantity,
                                CreatedDate = DateTime.Now,
                                NetQuantity = currentQuantity - countingQuantity,
                                UnitId = item.Item.ItemUnit.Where(m => m.IsMainUnit == true).Select(m => m.UnitId).FirstOrDefault(),
                                UnitPrice = 0,
                                TaxIncluded = false,
                                TaxRate = 0,
                                TaxAmount = 0,
                                SubTotal = 0,
                                DiscountRate = 0,
                                DiscountAmount = 0,
                                NewDetail = true,
                                OverallTotal = 0,
                                SyncStatus = 1,
                                ReceiptStatus = (int)ReceiptStatusType.Created,
                            });
                        }
                    }
                }

                #region SAVE ENTRY & DELIVERY RECEIPTS
                int entryReceiptId = 0;
                int deliveryReceiptId = 0;

                using (ReceiptBO bObj = new ReceiptBO())
                {
                    result = bObj.SaveOrUpdateItemReceipt(entryReceipt);
                    entryReceiptId = result.RecordId;
                }

                if (result.Result)
                {
                    using (ReceiptBO bObj = new ReceiptBO())
                    {
                        result = bObj.SaveOrUpdateItemReceipt(deliveryReceipt);
                        deliveryReceiptId = result.RecordId;
                    }
                }
                else
                    throw new Exception("Giriş fişinde hata: " + result.ErrorMessage);

                if (result.Result)
                {
                    dbObj.CountingStatus = 1;
                    _unitOfWork.SaveChanges();
                }
                else
                {
                    // ROLLBACK ENTRY RECEIPT
                    using (ReceiptBO bObj = new ReceiptBO())
                    {
                        bObj.DeleteItemReceipt(entryReceiptId);
                    }

                    throw new Exception("Çıkış fişinde hata: " + result.ErrorMessage);
                }
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
        #endregion

        #region STATUS VALIDATIONS
        public BusinessResult CheckReceiptDetailStatus(int itemReceiptDetailId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceiptDetail>();
                var repoConsumption = _unitOfWork.GetRepository<ItemReceiptConsume>();

                var dbDetail = repo.Get(d => d.Id == itemReceiptDetailId);
                if (dbDetail == null)
                    throw new Exception("İrsaliye kalemi bulunamadı.");

                var availableQty = dbDetail.Quantity;
                var usedQty = repoConsumption.Filter(d => d.ConsumedReceiptDetailId == dbDetail.Id)
                    .Sum(d => d.UsedQuantity ?? 0);
                if (availableQty <= usedQty && usedQty > 0)
                    dbDetail.ReceiptStatus = (int)ReceiptStatusType.Closed;
                else if (usedQty > 0 && availableQty > usedQty)
                    dbDetail.ReceiptStatus = (int)ReceiptStatusType.InUse;
                else if (usedQty == 0)
                    dbDetail.ReceiptStatus = (int)ReceiptStatusType.Created;

                _unitOfWork.SaveChanges();

                try
                {
                    using (ReceiptBO bObj = new ReceiptBO())
                    {
                        bObj.CheckReceiptStatus(dbDetail.ItemReceiptId.Value);
                    }
                }
                catch (Exception)
                {

                }

                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult CheckReceiptStatus(int itemReceiptId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceipt>();
                var dbReceipt = repo.Get(d => d.Id == itemReceiptId);
                if (dbReceipt == null)
                    throw new Exception("İrsaliye bulunamadı.");

                var totalDetailCount = dbReceipt.ItemReceiptDetail.Count();
                var closedDetailCount = dbReceipt.ItemReceiptDetail
                    .Where(d => d.ReceiptStatus == (int)ReceiptStatusType.Closed)
                    .Count();

                if (totalDetailCount <= closedDetailCount && closedDetailCount > 0)
                    dbReceipt.ReceiptStatus = (int)ReceiptStatusType.Closed;
                else if (closedDetailCount > 0)
                    dbReceipt.ReceiptStatus = (int)ReceiptStatusType.InUse;
                else if (closedDetailCount == 0)
                    dbReceipt.ReceiptStatus = (int)ReceiptStatusType.Created;

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
