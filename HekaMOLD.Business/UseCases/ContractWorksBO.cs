using Heka.DataAccess.Context;
using HekaMOLD.Business.Base;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.Models.Virtual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.UseCases
{
    public class ContractWorksBO : IBusinessObject
    {
        public WorkOrderDetailModel[] GetOuterWorkList(int? workOrderCategoryId)
        {
            WorkOrderDetailModel[] data = new WorkOrderDetailModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<WorkOrderDetail>();
                data = repo.Filter(d =>
                    d.WorkOrder.WorkOrderType == (int)WorkOrderType.Contract &&
                    (
                        workOrderCategoryId == null ||
                        (
                            d.WorkOrder.WorkOrderCategoryId == workOrderCategoryId
                        )
                    )
                   )
                    .ToList()
                    .Select(d => new WorkOrderDetailModel
                    {
                        Id = d.Id,
                        WorkOrderNo = d.WorkOrder.WorkOrderNo,
                        FirmCode = d.WorkOrder.Firm != null ? d.WorkOrder.Firm.FirmCode : "",
                        FirmName = d.WorkOrder.Firm != null ? d.WorkOrder.Firm.FirmName : "",
                        ProductCode = d.Item != null ? d.Item.ItemNo : "",
                        ProductName = d.Item != null ? d.Item.ItemName : "",
                        Quantity = d.Quantity,
                        WorkOrderDateStr = string.Format("{0:dd.MM.yyyy}", d.WorkOrder.WorkOrderDate),
                        Explanation = d.WorkOrder.Explanation,
                        WorkOrderStatus = d.WorkOrderStatus,
                        WorkOrderStatusStr = ((WorkOrderStatusType)d.WorkOrderStatus).ToCaption(),
                    }).ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }

        public BusinessResult CreateDelivery(ContractDeliveryModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                DateTime deliveryDate = DateTime.Now;
                if (!string.IsNullOrEmpty(model.DeliveryDate))
                {
                    deliveryDate = DateTime.ParseExact(model.DeliveryDate, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                }

                var repoReceiptDetail = _unitOfWork.GetRepository<ItemReceiptDetail>();
                var selectedEntryDetail = repoReceiptDetail.Get(d => d.Id == model.EntryReceiptDetailId);
                if (selectedEntryDetail == null)
                    throw new Exception("Seçilen irsaliye kaydı bulunamadı.");

                // CREATE DELIVERY ITEM RECEIPT
                BusinessResult receiptResult = null;
                using (ReceiptBO bObj = new ReceiptBO())
                {
                    receiptResult = bObj.SaveOrUpdateItemReceipt(new Models.DataTransfer.Receipt.ItemReceiptModel
                    {
                        CreatedDate = DateTime.Now,
                        ReceiptDate = deliveryDate,
                        ReceiptType = (int)ItemReceiptType.ToContractor,
                        FirmId = model.FirmId,
                        DocumentNo = model.DocumentNo,
                        InWarehouseId = selectedEntryDetail.ItemReceipt.InWarehouseId,
                        PlantId = selectedEntryDetail.ItemReceipt.PlantId,
                        ReceiptStatus = (int)ReceiptStatusType.Created,
                        ReceiptNo = bObj.GetNextReceiptNo(selectedEntryDetail.ItemReceipt.PlantId ?? 1, ItemReceiptType.ToContractor),
                        Details = new Models.DataTransfer.Receipt.ItemReceiptDetailModel[]
                        {
                            new Models.DataTransfer.Receipt.ItemReceiptDetailModel
                            {
                                ItemId = selectedEntryDetail.ItemId,
                                LineNumber = 1,
                                CreatedDate = DateTime.Now,
                                NewDetail = true,
                                Quantity = model.Quantity,
                                ReceiptStatus = (int)ReceiptStatusType.Created,
                            }
                        }
                    });
                }

                if (receiptResult.Result)
                {
                    // CREATE CONSUMINGS
                    BusinessResult consumptionResult = null;
                    using (ReceiptBO bObj = new ReceiptBO())
                    {
                        consumptionResult = bObj.UpdateConsume(model.EntryReceiptDetailId, receiptResult.DetailRecordId, model.Quantity ?? 0);
                    }

                    if (consumptionResult.Result)
                    {
                        var repo = _unitOfWork.GetRepository<ContractWorkFlow>();
                        var dbFlow = new ContractWorkFlow
                        {
                            WorkOrderDetailId = model.WorkOrderDetailId,
                            DeliveredDetailId = receiptResult.DetailRecordId,
                            ReceivedDetailId = null,
                            FlowDate = deliveryDate,
                        };
                        repo.Add(dbFlow);
                        _unitOfWork.SaveChanges();

                        result.RecordId = dbFlow.Id;
                    }
                    else
                    {
                        // ROLLBACK RECEIPT
                        using (ReceiptBO bObj = new ReceiptBO())
                        {
                            bObj.DeleteItemReceipt(receiptResult.RecordId);
                        }

                        throw new Exception(consumptionResult.ErrorMessage);
                    }
                }
                else
                    throw new Exception(receiptResult.ErrorMessage);

                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult CreateReceiving(ContractReceivingModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                DateTime entryDate = DateTime.Now;
                if (!string.IsNullOrEmpty(model.EntryDate))
                {
                    entryDate = DateTime.ParseExact(model.EntryDate, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                }

                var repoReceiptDetail = _unitOfWork.GetRepository<ItemReceiptDetail>();
                var selectedDeliveryDetail = repoReceiptDetail.Get(d => d.Id == model.DeliveredDetailId);
                if (selectedDeliveryDetail == null)
                    throw new Exception("Seçilen sevkiyat kaydı bulunamadı.");

                // CREATE ENTRY ITEM RECEIPT
                BusinessResult receiptResult = null;
                using (ReceiptBO bObj = new ReceiptBO())
                {
                    receiptResult = bObj.SaveOrUpdateItemReceipt(new Models.DataTransfer.Receipt.ItemReceiptModel
                    {
                        CreatedDate = DateTime.Now,
                        ReceiptDate = entryDate,
                        ReceiptType = (int)ItemReceiptType.FromContractor,
                        FirmId = selectedDeliveryDetail.ItemReceipt.FirmId,
                        DocumentNo = model.DocumentNo,
                        InWarehouseId = model.WarehouseId,
                        PlantId = selectedDeliveryDetail.ItemReceipt.PlantId,
                        ReceiptStatus = (int)ReceiptStatusType.Created,
                        ReceiptNo = bObj.GetNextReceiptNo(selectedDeliveryDetail.ItemReceipt.PlantId ?? 1, ItemReceiptType.FromContractor),
                        Details = new Models.DataTransfer.Receipt.ItemReceiptDetailModel[]
                        {
                            new Models.DataTransfer.Receipt.ItemReceiptDetailModel
                            {
                                ItemId = selectedDeliveryDetail.ItemId,
                                LineNumber = 1,
                                CreatedDate = DateTime.Now,
                                NewDetail = true,
                                Quantity = model.Quantity,
                                ReceiptStatus = (int)ReceiptStatusType.Created,
                            }
                        }
                    });
                }

                if (receiptResult.Result)
                {
                    var repo = _unitOfWork.GetRepository<ContractWorkFlow>();
                    var dbFlow = new ContractWorkFlow
                    {
                        WorkOrderDetailId = model.WorkOrderDetailId,
                        DeliveredDetailId = null,
                        ReceivedDetailId = receiptResult.DetailRecordId,
                        FlowDate = entryDate,
                    };
                    repo.Add(dbFlow);
                    _unitOfWork.SaveChanges();

                    // TRIGGER POINT: CHECK WORK ORDER STATUS
                    using (ContractWorksBO bObj = new ContractWorksBO())
                    {
                        bObj.CheckContractWorkOrderStatus(model.WorkOrderDetailId);
                    }
                }
                else
                    throw new Exception(receiptResult.ErrorMessage);

                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public ContractWorkFlowModel[] GetFlowList(int workOrderDetailId)
        {
            ContractWorkFlowModel[] data = new ContractWorkFlowModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<ContractWorkFlow>();
                data = repo.Filter(d => d.WorkOrderDetailId == workOrderDetailId)
                    .ToList()
                    .Select(d => new ContractWorkFlowModel
                    {
                        Id = d.Id,
                        DeliveredDetailId = d.DeliveredDetailId,
                        ReceivedDetailId = d.ReceivedDetailId,
                        FlowDate = d.FlowDate,
                        FlowDateStr = string.Format("{0:dd.MM.yyyy}", d.FlowDate),
                        WorkOrderDetailId = d.WorkOrderDetailId,
                        ItemNo = d.DeliveredReceiptDetail.Item != null ?
                                    d.DeliveredReceiptDetail.Item.ItemNo : "",
                        ItemName = d.DeliveredReceiptDetail.Item != null ?
                                    d.DeliveredReceiptDetail.Item.ItemName : "",
                        FirmCode = d.DeliveredReceiptDetail.ItemReceipt.Firm != null ?
                                    d.DeliveredReceiptDetail.ItemReceipt.Firm.FirmCode : "",
                        FirmName = d.DeliveredReceiptDetail.ItemReceipt.Firm != null ?
                                    d.DeliveredReceiptDetail.ItemReceipt.Firm.FirmName : "",
                        Quantity = d.DeliveredReceiptDetail.Quantity,
                    }).ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }

        #region LOGICAL STATUS CHECK FUNCTIONS
        public BusinessResult CheckContractWorkOrderStatus(int? workOrderDetailId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<WorkOrderDetail>();
                var repoFlows = _unitOfWork.GetRepository<ContractWorkFlow>();

                var dbObj = repo.Get(d => d.Id == workOrderDetailId);
                if (dbObj == null)
                    throw new Exception("İş emri kaydı bulunamadı.");

                var totalReceived = repoFlows.Filter(d => d.WorkOrderDetailId == dbObj.Id && d.ReceivedDetailId != null)
                    .Sum(d => d.ReceivedReceiptDetail.Quantity) ?? 0;
                if (dbObj.Quantity <= totalReceived)
                {
                    dbObj.WorkOrderStatus = (int)WorkOrderStatusType.Completed;
                    dbObj.UpdatedDate = DateTime.Now;
                }
                else if (dbObj.WorkOrderStatus == (int)WorkOrderStatusType.Completed)
                {
                    dbObj.WorkOrderStatus = (int)WorkOrderStatusType.Created;
                    dbObj.UpdatedDate = DateTime.Now;
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
