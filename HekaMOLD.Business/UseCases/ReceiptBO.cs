using Heka.DataAccess.Context;
using Heka.DataAccess.UnitOfWork;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.DataTransfer.Receipt;
using HekaMOLD.Business.Models.Dictionaries;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
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
            ItemReceiptModel[] data = new ItemReceiptModel[0];

            var repo = _unitOfWork.GetRepository<ItemReceipt>();

            data = repo.Filter(d =>
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
                .ToList()
                .Select(d => new ItemReceiptModel
                {
                    Id = d.Id,
                    ConsumptionReceiptId = d.ConsumptionReceiptId,
                    ConsumptionReceiptNo = d.ConsumptionReceipt != null ? d.ConsumptionReceipt.ReceiptNo : "",
                    CreatedDate = d.CreatedDate,
                    CreatedDateStr = string.Format("{0:dd.MM.yyyy}", d.CreatedDate),
                    CreatedUserId = d.CreatedUserId,
                    DocumentNo = d.DocumentNo,
                    Explanation = d.Explanation,
                    FirmCode = d.Firm != null ? d.Firm.FirmCode : "",
                    FirmId = d.FirmId,
                    FirmName = d.Firm != null ? d.Firm.FirmName : "",
                    InWarehouseId = d.InWarehouseId,
                    ItemOrderId = d.ItemOrderId,
                    OutWarehouseId = d.OutWarehouseId,
                    PlantId = d.PlantId,
                    ReceiptDate = d.ReceiptDate,
                    ReceiptNo = d.ReceiptNo,
                    ReceiptStatus = d.ReceiptStatus,
                    ReceiptType = d.ReceiptType,
                    ReceiptStatusStr = ((ReceiptStatusType)d.ReceiptStatus.Value).ToCaption(),
                    ReceiptDateStr = string.Format("{0:dd.MM.yyyy}", d.ReceiptDate),
                    WarehouseCode = d.Warehouse != null ? d.Warehouse.WarehouseCode : "",
                    WarehouseName = d.Warehouse != null ? d.Warehouse.WarehouseName : "",
                    ReceiptTypeStr = ((ItemReceiptType)d.ReceiptType.Value).ToCaption(),
                    ItemOrderDocumentNo = d.ItemOrder != null ? (d.ItemOrder.DocumentNo != null && d.ItemOrder.DocumentNo.Length > 0 ?
                        d.ItemOrder.DocumentNo : d.ItemOrder.OrderNo) : "",
                })
                .OrderByDescending(d => d.Id)
                .ToArray();

            return data;
        }

        #region OFF THE RECORD MANAGEMENT
        public ItemReceiptDetailModel[] GetOffTheRecordList()
        {
            ItemReceiptDetailModel[] data = new ItemReceiptDetailModel[0];

            var repo = _unitOfWork.GetRepository<ItemReceiptDetail>();

            data = repo.Filter(d =>
                d.ReceiptStatus == (int)ReceiptStatusType.OffTheRecord
            )
                .ToList()
                .Select(d => new ItemReceiptDetailModel
                {
                    Id = d.Id,
                    CreatedDate = d.CreatedDate,
                    CreatedUserId = d.CreatedUserId,
                    DiscountAmount = d.DiscountAmount,
                    Explanation = d.Explanation,
                    FirmCode = d.ItemReceipt.Firm != null ? d.ItemReceipt.Firm.FirmCode : "",
                    FirmName = d.ItemReceipt.Firm != null ? d.ItemReceipt.Firm.FirmName : "",
                    ForexId = d.ForexId,
                    GrossQuantity = d.GrossQuantity,
                    ForexRate = d.ForexRate,
                    ForexUnitPrice = d.ForexUnitPrice,
                    ItemId = d.ItemId,
                    ItemName = d.Item != null ? d.Item.ItemName : "",
                    ItemNo = d.Item != null ? d.Item.ItemNo : "",
                    ItemOrderDetailId = d.ItemOrderDetailId,
                    ItemReceiptId = d.ItemReceiptId,
                    LineNumber = d.LineNumber,
                    NetQuantity = d.NetQuantity,
                    OverallTotal = d.OverallTotal,
                    Quantity = d.Quantity,
                    ReceiptDateStr = d.ItemReceipt.ReceiptDate != null ?
                        string.Format("{0:dd.MM.yyyy}", d.ItemReceipt.ReceiptDate) : "",
                    ReceiptStatus = d.ReceiptStatus,
                    ReceiptNo = d.ItemReceipt.ReceiptNo,
                    SyncDate = d.SyncDate,
                    SyncStatus = d.SyncStatus ?? 0,
                    TaxAmount = d.TaxAmount,
                    TaxIncluded = d.TaxIncluded,
                    TaxRate = d.TaxRate,
                    UnitId = d.UnitId,
                    UnitCode = d.UnitType != null ? d.UnitType.UnitCode : "",
                    UnitName = d.UnitType != null ? d.UnitType.UnitName : "",
                    UnitPrice = d.UnitPrice,
                    SubTotal = d.SubTotal,
                })
                .OrderByDescending(d => d.Id)
                .ToArray();

            return data;
        }

        public ItemReceiptDetailModel[] GetWaitingForSyncSalesList()
        {
            ItemReceiptDetailModel[] data = new ItemReceiptDetailModel[0];

            var repo = _unitOfWork.GetRepository<ItemReceiptDetail>();
            var dtStart = DateTime.ParseExact("16.03.2022", "dd.MM.yyyy",
                System.Globalization.CultureInfo.GetCultureInfo("tr"));

            data = repo.Filter(d =>
                d.ReceiptStatus != (int)ReceiptStatusType.OffTheRecord &&
                d.ReceiptStatus != (int)ReceiptStatusType.ReadyToSync &&
                d.ItemReceipt.ReceiptType == (int)ItemReceiptType.ItemSelling
                && d.ItemReceipt.ReceiptDate > dtStart
            )
                .ToList()
                .Select(d => new ItemReceiptDetailModel
                {
                    Id = d.Id,
                    CreatedDate = d.CreatedDate,
                    CreatedUserId = d.CreatedUserId,
                    DiscountAmount = d.DiscountAmount,
                    Explanation = d.Explanation,
                    FirmCode = d.ItemReceipt.Firm != null ? d.ItemReceipt.Firm.FirmCode : "",
                    FirmName = d.ItemReceipt.Firm != null ? d.ItemReceipt.Firm.FirmName : "",
                    ForexId = d.ForexId,
                    GrossQuantity = d.GrossQuantity,
                    ForexRate = d.ForexRate,
                    ForexUnitPrice = d.ForexUnitPrice,
                    ItemId = d.ItemId,
                    ItemName = d.Item != null ? d.Item.ItemName : "",
                    ItemNo = d.Item != null ? d.Item.ItemNo : "",
                    ItemOrderDetailId = d.ItemOrderDetailId,
                    ItemReceiptId = d.ItemReceiptId,
                    LineNumber = d.LineNumber,
                    NetQuantity = d.NetQuantity,
                    OverallTotal = d.OverallTotal,
                    Quantity = d.Quantity,
                    ReceiptDateStr = d.ItemReceipt.ReceiptDate != null ?
                        string.Format("{0:dd.MM.yyyy}", d.ItemReceipt.ReceiptDate) : "",
                    ReceiptStatus = d.ReceiptStatus,
                    ReceiptNo = d.ItemReceipt.ReceiptNo,
                    SyncDate = d.SyncDate,
                    SyncStatus = d.SyncStatus ?? 0,
                    TaxAmount = d.TaxAmount,
                    TaxIncluded = d.TaxIncluded,
                    TaxRate = d.TaxRate,
                    UnitId = d.UnitId,
                    UnitCode = d.UnitType != null ? d.UnitType.UnitCode : "",
                    UnitName = d.UnitType != null ? d.UnitType.UnitName : "",
                    UnitPrice = d.UnitPrice,
                    SubTotal = d.SubTotal,
                })
                .OrderByDescending(d => d.Id)
                .ToArray();

            return data;
        }

        public ItemReceiptDetailModel[] GetReadyToSyncSalesList()
        {
            ItemReceiptDetailModel[] data = new ItemReceiptDetailModel[0];

            var repo = _unitOfWork.GetRepository<ItemReceiptDetail>();

            data = repo.Filter(d =>
                d.ReceiptStatus == (int)ReceiptStatusType.ReadyToSync
            )
                .ToList()
                .Select(d => new ItemReceiptDetailModel
                {
                    Id = d.Id,
                    CreatedDate = d.CreatedDate,
                    CreatedUserId = d.CreatedUserId,
                    DiscountAmount = d.DiscountAmount,
                    Explanation = d.Explanation,
                    FirmCode = d.ItemReceipt.Firm != null ? d.ItemReceipt.Firm.FirmCode : "",
                    FirmName = d.ItemReceipt.Firm != null ? d.ItemReceipt.Firm.FirmName : "",
                    ForexId = d.ForexId,
                    GrossQuantity = d.GrossQuantity,
                    ForexRate = d.ForexRate,
                    ForexUnitPrice = d.ForexUnitPrice,
                    ItemId = d.ItemId,
                    ItemName = d.Item != null ? d.Item.ItemName : "",
                    ItemNo = d.Item != null ? d.Item.ItemNo : "",
                    ItemOrderDetailId = d.ItemOrderDetailId,
                    ItemReceiptId = d.ItemReceiptId,
                    LineNumber = d.LineNumber,
                    NetQuantity = d.NetQuantity,
                    OverallTotal = d.OverallTotal,
                    Quantity = d.Quantity,
                    ReceiptDateStr = d.ItemReceipt.ReceiptDate != null ?
                        string.Format("{0:dd.MM.yyyy}", d.ItemReceipt.ReceiptDate) : "",
                    ReceiptStatus = d.ReceiptStatus,
                    ReceiptNo = d.ItemReceipt.ReceiptNo,
                    SyncDate = d.SyncDate,
                    SyncStatus = d.SyncStatus ?? 0,
                    TaxAmount = d.TaxAmount,
                    TaxIncluded = d.TaxIncluded,
                    TaxRate = d.TaxRate,
                    UnitId = d.UnitId,
                    UnitCode = d.UnitType != null ? d.UnitType.UnitCode : "",
                    UnitName = d.UnitType != null ? d.UnitType.UnitName : "",
                    UnitPrice = d.UnitPrice,
                    SubTotal = d.SubTotal,
                })
                .OrderByDescending(d => d.Id)
                .ToArray();

            return data;
        }

        public BusinessResult ChangeOTRStatus(int itemReceiptDetailId, int otrStatus)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceiptDetail>();
                var dbObj = repo.Get(d => d.Id == itemReceiptDetailId);
                if (dbObj == null)
                    throw new Exception("İrsaliye kaydına ulaşılamadı.");

                dbObj.ReceiptStatus = otrStatus;

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

        public ItemSerialModel[] GetSerialsOfDetail(int rid)
        {
            ItemSerialModel[] data = new ItemSerialModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<ItemSerial>();
                data = repo.Filter(d => d.ItemReceiptDetailId == rid)
                    .ToList()
                    .Select(d => new ItemSerialModel
                    {
                        Id = d.Id,
                        CreatedDate = d.CreatedDate,
                        CreatedUserId = d.CreatedUserId,
                        FirstQuantity = d.FirstQuantity,
                        CreatedDateStr = string.Format("{0:dd.MM.yyyy}", d.CreatedDate ?? DateTime.Now),
                        ItemId = d.ItemId,
                        InPackageQuantity = d.InPackageQuantity,
                        ItemReceiptDetailId = d.ItemReceiptDetailId,
                        SerialNo = d.SerialNo,
                    }).ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }

        public string GetNextSerialNo()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<ItemSerial>();
                string lastSerialNo = repo
                    .Filter(d => d.SerialNo != null && d.SerialNo.Length > 0)
                    .OrderByDescending(d => d.SerialNo)
                    .ToList()
                    .Where(d => Convert.ToInt64(d.SerialNo) > 60000000)
                    .Select(d => d.SerialNo)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(lastSerialNo))
                    lastSerialNo = "60000000";

                return string.Format("{0:00000000}", Convert.ToInt64(lastSerialNo) + 1);
            }
            catch (Exception ex)
            {

            }

            return default;
        }

        public ItemReceiptModel SearchPurchaseReceipt(string receiptNo)
        {
            ItemReceiptModel model = new ItemReceiptModel();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceipt>();
                var dbObj = repo.Get(d => d.ReceiptNo == receiptNo && d.ReceiptType == (int)ItemReceiptType.ItemBuying);
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
                    model.ConsumptionReceiptNo = dbObj.ConsumptionReceipt != null ? dbObj.ConsumptionReceipt.ReceiptNo : "";

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
            }
            catch (Exception)
            {

            }

            return model;
        }
        public BusinessResult AddItemEntry(int itemReceiptDetailId, int userId, WorkOrderSerialType serialType,
           int inPackageQuantity)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceiptDetail>();
                var repoItemSerial = _unitOfWork.GetRepository<ItemSerial>();
                var repoShift = _unitOfWork.GetRepository<Shift>();

                var dbObj = repo.Get(d => d.Id == itemReceiptDetailId);
                if (dbObj == null)
                    throw new Exception("İrsaliye kaydına ulaşılamadı.");

                ItemSerial product = null;

                ShiftModel currentShift = null;

                using (ProductionBO bObj = new ProductionBO())
                {
                    currentShift = bObj.GetCurrentShift();
                }

                // BATUSAN
                if (serialType == WorkOrderSerialType.ProductPackage)
                {
                    product = new ItemSerial
                    {
                        CreatedDate = DateTime.Now,
                        InPackageQuantity = inPackageQuantity,
                        ItemReceiptDetailId = itemReceiptDetailId,
                        FirstQuantity = inPackageQuantity,
                        LiveQuantity = inPackageQuantity,
                        SerialNo = GetNextSerialNo(),
                        ShiftBelongsToDate = currentShift != null ? currentShift.ShiftBelongsToDate : DateTime.Now,
                        SerialStatus = (int)SerialStatusType.Created,
                        SerialType = (int)serialType,
                        CreatedUserId = userId,
                    };

                    repoItemSerial.Add(product);
                }

                _unitOfWork.SaveChanges();

                result.RecordId = product.Id;
                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public ItemSerialModel GetItemSerial(int serialId)
        {
            ItemSerialModel data = new ItemSerialModel();

            var repo = _unitOfWork.GetRepository<ItemSerial>();
            var dbObj = repo.Get(d => d.Id == serialId);
            if (dbObj != null)
            {
                dbObj.MapTo(data);
                if (dbObj.ItemReceiptDetail.ItemReceipt.Firm != null)
                {
                    data.FirmCode = dbObj.ItemReceiptDetail.ItemReceipt.Firm.FirmCode;
                    data.FirmName = dbObj.ItemReceiptDetail.ItemReceipt.Firm.FirmName;
                }

                if (dbObj.ItemReceiptDetail.Item != null)
                {
                    data.ItemId = dbObj.ItemReceiptDetail.ItemId;
                    data.ItemNo = dbObj.ItemReceiptDetail.Item.ItemNo;
                    data.ItemName = dbObj.ItemReceiptDetail.Item.ItemName;
                }
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
                var repoOrderConsume = _unitOfWork.GetRepository<ItemOrderConsume>();

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

                if (!string.IsNullOrEmpty(model.ReceiptDateStr))
                {
                    model.ReceiptDate = DateTime.ParseExact(model.ReceiptDateStr, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
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
                List<int> orderDetailsChecked = new List<int>();
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

                            #region SET ORDER & DETAIL TO APPROVED
                            if (item.ItemOrderDetail != null)
                            {
                                item.ItemOrderDetail.OrderStatus = (int)OrderStatusType.Approved;
                                item.ItemOrderDetail.ItemOrder.OrderStatus = (int)OrderStatusType.Approved;
                            }
                            #endregion

                            #region CHECK ITEM ORDERS FOR PURCHASING
                            if (item.ItemOrderDetailId != null)
                            {
                                if (!orderDetailsChecked.Contains(item.ItemOrderDetailId.Value))
                                    orderDetailsChecked.Add(item.ItemOrderDetailId.Value);

                                var consumeList = repoOrderConsume.Filter(d => d.ItemOrderDetailId == item.ItemOrderDetailId.Value
                                    && d.ConsumerReceiptDetailId == item.Id).ToArray();
                                foreach (var consumer in consumeList)
                                {
                                    repoOrderConsume.Delete(consumer);
                                }
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

                            #region CHECK ITEM ORDER DETAILS (PURCHASE)
                            if (dbDetail.ItemOrderDetailId != null)
                            {
                                var consumedOrder = repoOrderConsume.Filter(d => d.ConsumerReceiptDetailId == dbDetail.Id &&
                                    d.ItemOrderDetailId == dbDetail.ItemOrderDetailId).FirstOrDefault();
                                if (consumedOrder == null)
                                {
                                    consumedOrder = new ItemOrderConsume
                                    {
                                        ItemReceiptDetailConsumer = dbDetail,
                                        ItemOrderDetailId = dbDetail.ItemOrderDetailId,
                                        UsedQuantity = dbDetail.Quantity,
                                    };
                                    repoOrderConsume.Add(consumedOrder);

                                    if (!orderDetailsChecked.Contains(dbDetail.ItemOrderDetailId.Value))
                                        orderDetailsChecked.Add(dbDetail.ItemOrderDetailId.Value);
                                }
                            }
                            #endregion

                            lineNo++;
                        }
                    }
                }
                #endregion

                _unitOfWork.SaveChanges();

                #region TRG-POINT-ITEM-STATUS
                if (itemsMustBeUpdated.Count() > 0)
                    base.UpdateItemStats(itemsMustBeUpdated.ToArray());
                #endregion

                #region TRG-POINT-CHECK-RECEIPT-DETAILS
                try
                {
                    using (ReceiptBO bObj = new ReceiptBO())
                    {
                        var dbReceipt = bObj.GetItemReceipt(dbObj.Id);
                        if (dbReceipt.Details != null)
                        {
                            foreach (var rDetail in dbReceipt.Details)
                            {
                                using (ReceiptBO dObj = new ReceiptBO())
                                {
                                    dObj.CheckReceiptDetailStatus(rDetail.Id);
                                }
                            }
                        }
                    }

                    using (ReceiptBO bObj = new ReceiptBO())
                    {
                        bObj.CheckReceiptStatus(dbObj.Id);
                    }
                }
                catch (Exception)
                {

                }
                #endregion

                #region TRG-POINT-CHECK-ORDER-DETAILS
                foreach (var item in orderDetailsChecked)
                {
                    using (OrdersBO bObj = new OrdersBO())
                    {
                        bObj.CheckOrderDetailStatus(item);
                    }
                }
                #endregion

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

        public BusinessResult UpdateDocumentNo(int receiptId, string documentNo)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceipt>();

                var dbObj = repo.Get(d => d.Id == receiptId);
                if (dbObj == null)
                    throw new Exception("İrsaliye kaydı bulunamadı.");

                dbObj.DocumentNo = documentNo;

                _unitOfWork.SaveChanges();

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

        public BusinessResult UpdateItemSerialQuantity(int serialId, decimal newQuantity)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemSerial>();
                var dbObj = repo.Get(d => d.Id == serialId);
                if (dbObj == null)
                    throw new Exception("Paket kaydı bulunamadı.");

                var exQuantity = dbObj.FirstQuantity;

                dbObj.FirstQuantity = newQuantity;
                dbObj.LiveQuantity = newQuantity;

                var receiptDetail = dbObj.ItemReceiptDetail;
                receiptDetail.Quantity = receiptDetail.ItemSerial.Sum(d => d.FirstQuantity) ?? 0;

                _unitOfWork.SaveChanges();

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

        public BusinessResult RemoveItemSerial(int serialId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemSerial>();
                var repoReceiptDetail = _unitOfWork.GetRepository<ItemReceiptDetail>();

                var dbObj = repo.Get(d => d.Id == serialId);
                if (dbObj == null)
                    throw new Exception("Paket kaydı bulunamadı.");

                var receiptDetail = dbObj.ItemReceiptDetail;
                receiptDetail.Quantity -= dbObj.FirstQuantity;

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
                var repoConsume = _unitOfWork.GetRepository<ItemReceiptConsume>();
                var repoOrderConsume = _unitOfWork.GetRepository<ItemOrderConsume>();

                var dbObj = repo.Get(d => d.Id == id);
                if (dbObj == null)
                    throw new Exception("Silinmesi istenen irsaliye kaydına ulaşılamadı.");

                //if (dbObj.ItemReceipt.Any())
                //    throw new Exception("İrsaliyesi girilmiş olan bir sipariş silinemez.");

                List<int> itemsMustBeUpdated = new List<int>();
                List<int> ordersMustBeUpdated = new List<int>();
                List<int> receiptDetailsMustBeUpdated = new List<int>();

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

                        // CLEAR CONSUMINGS
                        if (item.ItemReceiptConsumeByConsumed.Any())
                        {
                            var relatedData = item.ItemReceiptConsumeByConsumed.ToArray();
                            foreach (var rItem in relatedData)
                            {
                                if (!receiptDetailsMustBeUpdated.Contains(rItem.ConsumerReceiptDetailId ?? 0))
                                    receiptDetailsMustBeUpdated.Add(rItem.ConsumerReceiptDetailId ?? 0);
                                repoConsume.Delete(rItem);
                            }
                        }

                        if (item.ItemReceiptConsumeByConsumer.Any())
                        {
                            var relatedData = item.ItemReceiptConsumeByConsumer.ToArray();
                            foreach (var rItem in relatedData)
                            {
                                if (!receiptDetailsMustBeUpdated.Contains(rItem.ConsumedReceiptDetailId ?? 0))
                                    receiptDetailsMustBeUpdated.Add(rItem.ConsumedReceiptDetailId ?? 0);
                                repoConsume.Delete(rItem);
                            }
                        }

                        //if (item.ItemOrderConsumeByConsumed.Any())
                        //{
                        //    var relatedData = item.ItemOrderConsumeByConsumed.ToArray();
                        //    foreach (var rItem in relatedData)
                        //    {
                        //        if (!ordersMustBeUpdated.Contains(rItem.ItemOrderDetailId ?? 0))
                        //            ordersMustBeUpdated.Add(rItem.ItemOrderDetailId ?? 0);
                        //        repoOrderConsume.Delete(rItem);
                        //    }
                        //}

                        // UPDATE ORDER STATUS


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

                if (ordersMustBeUpdated.Any(d => d > 0))
                {
                    foreach (var item in ordersMustBeUpdated)
                    {
                        using (OrdersBO bObj = new OrdersBO())
                        {
                            var res = bObj.CheckOrderDetailStatus(item).Result;
                        }
                    }
                }

                if (receiptDetailsMustBeUpdated.Any(d => d > 0))
                {
                    foreach (var item in receiptDetailsMustBeUpdated)
                    {
                        using (ReceiptBO bObj = new ReceiptBO())
                        {
                            var res = bObj.CheckReceiptDetailStatus(item).Result;
                        }
                    }
                }

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
                model.ConsumptionReceiptNo = dbObj.ConsumptionReceipt != null ? dbObj.ConsumptionReceipt.ReceiptNo : "";

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
                var selUnit = dbItem.ItemUnit.FirstOrDefault(d => d.UnitId == model.UnitId);
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

            decimal? taxExtractedUnitPrice = 0;
            decimal? subTotal = 0;

            if (model.TaxIncluded == true)
            {
                decimal? taxIncludedUnitPrice = (model.UnitPrice / (1 + (model.TaxRate / 100m)));
                taxExtractedUnitPrice = taxIncludedUnitPrice;
                subTotal = taxExtractedUnitPrice * model.Quantity;
            }
            else
            {
                subTotal = model.UnitPrice * model.Quantity;
                taxExtractedUnitPrice = model.UnitPrice;
            }

            model.SubTotal = subTotal;
            model.TaxAmount = subTotal * model.TaxRate / 100.0m;
            model.OverallTotal = subTotal + model.TaxAmount;

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
                        && d.ReceiptType == (int)ItemReceiptType.WarehouseInput && d.Warehouse.WarehouseType == (int)WarehouseType.ProductWarehouse)
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

        public ItemReceiptModel[] GetNonSyncPurchasedItems()
        {
            ItemReceiptModel[] data = new ItemReceiptModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceipt>();
                data = repo.Filter(d => (d.SyncStatus == null || d.SyncStatus == 0)
                        && d.ReceiptType == (int)ItemReceiptType.ItemBuying)
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
                                ForexId = m.ForexId,
                                ForexRate = m.ForexRate,
                                ForexUnitPrice = m.ForexUnitPrice,
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
                var repoCountingDetail = _unitOfWork.GetRepository<CountingReceiptDetail>();
                var repoWr = _unitOfWork.GetRepository<Warehouse>();
                var repoItem = _unitOfWork.GetRepository<Item>();

                var dbObj = repo.Get(d => d.Id == countingReceiptId);
                if (dbObj == null)
                    throw new Exception("Sayım kaydı bulunamadı.");

                var dbWrItem = repoWr.Get(d => d.WarehouseType == (int)WarehouseType.ItemWarehouse);
                var dbWrProduct = repoWr.Get(d => d.WarehouseType == (int)WarehouseType.ProductWarehouse);

                var entryReceipt = new ItemReceiptModel { 
                    ReceiptType = (int)ItemReceiptType.WarehouseInput,
                    ReceiptNo = GetNextReceiptNo(dbObj.PlantId.Value, ItemReceiptType.WarehouseInput),
                    PlantId = dbObj.PlantId,
                    CreatedDate = DateTime.Now,
                    ReceiptDate = DateTime.Now,
                    FirmId = null,
                    ReceiptStatus = (int)ReceiptStatusType.Created,
                    SyncStatus = 1,
                    InWarehouseId = dbWrItem.Id,
                };
                var deliveryReceipt = new ItemReceiptModel {
                    ReceiptType = (int)ItemReceiptType.WarehouseOutput,
                    ReceiptNo = GetNextReceiptNo(dbObj.PlantId.Value, ItemReceiptType.WarehouseOutput),
                    PlantId = dbObj.PlantId,
                    CreatedDate = DateTime.Now,
                    ReceiptDate = DateTime.Now,
                    FirmId = null,
                    ReceiptStatus = (int)ReceiptStatusType.Created,
                    SyncStatus = 1,
                    InWarehouseId = dbWrItem.Id,
                };
                var entryReceiptProduct = new ItemReceiptModel
                {
                    ReceiptType = (int)ItemReceiptType.WarehouseInput,
                    ReceiptNo = string.Format("{0:000000}", Convert.ToInt32(GetNextReceiptNo(dbObj.PlantId.Value, ItemReceiptType.WarehouseInput)) + 1),
                    PlantId = dbObj.PlantId,
                    CreatedDate = DateTime.Now,
                    ReceiptDate = DateTime.Now,
                    FirmId = null,
                    ReceiptStatus = (int)ReceiptStatusType.Created,
                    SyncStatus = 1,
                    InWarehouseId = dbWrProduct.Id,
                };
                var deliveryReceiptProduct = new ItemReceiptModel
                {
                    ReceiptType = (int)ItemReceiptType.WarehouseOutput,
                    ReceiptNo = string.Format("{0:000000}", Convert.ToInt32(GetNextReceiptNo(dbObj.PlantId.Value, ItemReceiptType.WarehouseOutput)) + 1),
                    PlantId = dbObj.PlantId,
                    CreatedDate = DateTime.Now,
                    ReceiptDate = DateTime.Now,
                    FirmId = null,
                    ReceiptStatus = (int)ReceiptStatusType.Created,
                    SyncStatus = 1,
                    InWarehouseId = dbWrProduct.Id,
                };

                List<ItemReceiptDetailModel> entryDetails = new List<ItemReceiptDetailModel>();
                List<ItemReceiptDetailModel> deliveryDetails = new List<ItemReceiptDetailModel>();
                List<ItemReceiptDetailModel> entryDetailsProduct = new List<ItemReceiptDetailModel>();
                List<ItemReceiptDetailModel> deliveryDetailsProduct = new List<ItemReceiptDetailModel>();

                var existingItems = repoCountingDetail.Filter(d => d.CountingReceiptId == dbObj.Id).Select(d => d.ItemId).Distinct().ToArray();
                var notExistsItems = repoItem.Filter(d => !existingItems.Contains(d.Id)).ToArray();
                var countingDetails = repoCountingDetail.Filter(d => d.CountingReceiptId == dbObj.Id).ToArray();

                // ADD COUNTED ITEMS
                foreach (var item in countingDetails)
                {
                    if (item.ItemId > 0)
                    {
                        var currentQuantity = GetItemQuantity(item.ItemId.Value);
                        var countingQuantity = item.Quantity ?? 0;

                        if (countingQuantity > currentQuantity) // will be positive movement
                        {
                            var nDetail = new ItemReceiptDetailModel
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
                            };

                            if (item.Item.ItemNo.StartsWith("152"))
                                entryDetailsProduct.Add(nDetail);
                            else
                                entryDetails.Add(nDetail);
                        }
                        else if (countingQuantity < currentQuantity) // will be negative movement
                        {
                            var nDetail = new ItemReceiptDetailModel
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
                            };

                            if (item.Item.ItemNo.StartsWith("152"))
                                deliveryDetailsProduct.Add(nDetail);
                            else
                                deliveryDetails.Add(nDetail);
                        }
                    }
                }

                // ADD NON EXISTING ITEMS
                foreach (var item in notExistsItems)
                {
                    if (item.Id > 0)
                    {
                        var currentQuantity = GetItemQuantity(item.Id);
                        var countingQuantity = 0;

                        if (countingQuantity > currentQuantity) // will be positive movement
                        {
                            var nDetail = new ItemReceiptDetailModel
                            {
                                ItemId = item.Id,
                                LineNumber = entryDetails.Count() + 1,
                                Quantity = countingQuantity - currentQuantity,
                                CreatedDate = DateTime.Now,
                                NetQuantity = countingQuantity - currentQuantity,
                                UnitId = item.ItemUnit.Where(m => m.IsMainUnit == true).Select(m => m.UnitId).FirstOrDefault(),
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
                            };

                            if (item.ItemNo.StartsWith("152"))
                                entryDetailsProduct.Add(nDetail);
                            else
                                entryDetails.Add(nDetail);
                        }
                        else if (countingQuantity < currentQuantity) // will be negative movement
                        {
                            var nDetail = new ItemReceiptDetailModel
                            {
                                ItemId = item.Id,
                                LineNumber = entryDetails.Count() + 1,
                                Quantity = currentQuantity - countingQuantity,
                                CreatedDate = DateTime.Now,
                                NetQuantity = currentQuantity - countingQuantity,
                                UnitId = item.ItemUnit.Where(m => m.IsMainUnit == true).Select(m => m.UnitId).FirstOrDefault(),
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
                            };

                            if (item.ItemNo.StartsWith("152"))
                                deliveryDetailsProduct.Add(nDetail);
                            else
                                deliveryDetails.Add(nDetail);
                        }
                    }
                }

                entryReceipt.Details = entryDetails.ToArray();
                deliveryReceipt.Details = deliveryDetails.ToArray();
                entryReceiptProduct.Details = entryDetailsProduct.ToArray();
                deliveryReceiptProduct.Details = deliveryDetailsProduct.ToArray();

                #region SAVE ENTRY & DELIVERY RECEIPTS
                int entryReceiptId = 0;
                int deliveryReceiptId = 0;

                using (ReceiptBO bObj = new ReceiptBO())
                {
                    result = bObj.SaveOrUpdateItemReceipt(entryReceipt);
                    entryReceiptId = result.RecordId;
                }

                using (ReceiptBO bObj = new ReceiptBO())
                {
                    result = bObj.SaveOrUpdateItemReceipt(entryReceiptProduct);
                }

                if (result.Result)
                {
                    using (ReceiptBO bObj = new ReceiptBO())
                    {
                        result = bObj.SaveOrUpdateItemReceipt(deliveryReceipt);
                        deliveryReceiptId = result.RecordId;
                    }

                    using (ReceiptBO bObj = new ReceiptBO())
                    {
                        result = bObj.SaveOrUpdateItemReceipt(deliveryReceiptProduct);
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

                if (dbDetail.ReceiptStatus != (int)ReceiptStatusType.OffTheRecord &&
                    dbDetail.ReceiptStatus != (int)ReceiptStatusType.ReadyToSync)
                {
                    var availableQty = dbDetail.Quantity;
                    var usedQty = repoConsumption.Filter(d => d.ConsumedReceiptDetailId == dbDetail.Id)
                        .Sum(d => d.UsedQuantity ?? 0);
                    if (availableQty <= usedQty && usedQty > 0)
                        dbDetail.ReceiptStatus = (int)ReceiptStatusType.Closed;
                    else if (usedQty > 0 && availableQty > usedQty)
                        dbDetail.ReceiptStatus = (int)ReceiptStatusType.InUse;
                    else if (usedQty == 0)
                        dbDetail.ReceiptStatus = (int)ReceiptStatusType.Created;
                }

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
