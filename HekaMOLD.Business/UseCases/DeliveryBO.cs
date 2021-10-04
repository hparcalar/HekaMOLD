using Heka.DataAccess.Context;
using HekaMOLD.Business.Base;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.UseCases
{
    public class DeliveryBO : IBusinessObject
    {
        #region PLANNING BUSINESS
        public BusinessResult CreateDeliveryPlan(WorkOrderDetailModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<WorkOrder>();
                var repoDetails = _unitOfWork.GetRepository<WorkOrderDetail>();
                var repoItem = _unitOfWork.GetRepository<Item>();
                var repoDeliveryPlan = _unitOfWork.GetRepository<DeliveryPlan>();

                var dbWorkOrderDetail = repoDetails.Get(d => d.Id == model.Id);
                if (dbWorkOrderDetail == null)
                    throw new Exception("Planlanması istenen iş emrinin kaydına ulaşılamadı.");

                dbWorkOrderDetail.MapTo(model);

                #region CHECK/ADD DELIVERY PLAN QUEUE
                var dbDeliveryPlan = repoDeliveryPlan.Get(d => d.WorkOrderDetailId == dbWorkOrderDetail.Id);
                if (dbDeliveryPlan == null)
                {
                    int? lastOrderNo = repoDeliveryPlan.Filter(d => d.PlanDate == model.DeliveryPlanDate)
                        .Max(d => d.OrderNo);
                    if ((lastOrderNo ?? 0) == 0)
                        lastOrderNo = 0;

                    lastOrderNo++;

                    dbDeliveryPlan = new DeliveryPlan
                    {
                        WorkOrderDetail = dbWorkOrderDetail,
                        PlanDate = model.DeliveryPlanDate,
                        OrderNo = lastOrderNo
                    };
                    repoDeliveryPlan.Add(dbDeliveryPlan);
                }
                #endregion

                _unitOfWork.SaveChanges();

                result.Result = true;
                result.RecordId = dbDeliveryPlan.Id;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult ReOrderPlan(DeliveryPlanModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<DeliveryPlan>();
                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                    throw new Exception("İşlem yapmak istediğiniz sevkiyat planı bulunamadı.");

                // REORDER EX DELIVERY PLAN LIST
                if (dbObj.PlanDate != model.PlanDate)
                {
                    var exDeliveryPlans = repo.Filter(d => d.PlanDate == dbObj.PlanDate
                            && d.Id != dbObj.Id)
                        .OrderBy(d => d.OrderNo)
                        .ToArray();

                    var orderNo = 1;
                    foreach (var item in exDeliveryPlans)
                    {
                        item.OrderNo = orderNo++;
                    }
                }

                dbObj.PlanDate = model.PlanDate;

                var activePlans = repo.Filter(d => d.PlanDate == dbObj.PlanDate).ToArray();

                if (model.OrderNo == -1)
                {
                    var lastOrderNo = 1;
                    var orderedList = activePlans.Where(d => d.Id != dbObj.Id).OrderBy(d => d.OrderNo).ToArray();
                    foreach (var item in orderedList)
                    {
                        item.OrderNo = lastOrderNo++;
                    }

                    dbObj.OrderNo = lastOrderNo;
                }
                else
                {
                    var laterPlans = activePlans.Where(d => d.OrderNo >= model.OrderNo &&
                        d.Id != dbObj.Id
                    )
                        .OrderBy(d => d.OrderNo)
                        .ToArray();

                    var lastOrderNo = model.OrderNo;
                    foreach (var item in laterPlans)
                    {
                        if (item.OrderNo - lastOrderNo > 1)
                            item.OrderNo = lastOrderNo + 1;
                        else
                            item.OrderNo++;

                        lastOrderNo = item.OrderNo;
                    }

                    dbObj.OrderNo = model.OrderNo;
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

        public WorkOrderDetailModel[] GetWaitingWorkOrders()
        {
            WorkOrderDetailModel[] data = new WorkOrderDetailModel[0];

            var repo = _unitOfWork.GetRepository<WorkOrderDetail>();
            data = repo.Filter(d =>
                d.WorkOrder.WorkOrderStatus != (int)WorkOrderStatusType.Delivered
                && !d.DeliveryPlan.Any()
            ).ToList().Select(d => new WorkOrderDetailModel
            {
                Id = d.Id,
                WorkOrderDateStr = string.Format("{0:dd.MM.yyyy}", d.WorkOrder.WorkOrderDate),
                SaleOrderDeadline = d.ItemOrderDetail != null ?
                    string.Format("{0:dd.MM.yyyy}", d.ItemOrderDetail.ItemOrder.DateOfNeed) : "",
                ProductCode = d.Item != null ? d.Item.ItemNo : "",
                ProductName = d.Item != null ? d.Item.ItemName : "",
                FirmName = d.WorkOrder.Firm != null ?
                    d.WorkOrder.Firm.FirmName : "",
                Quantity = d.Quantity,
            }).ToArray();

            return data;
        }

        public DeliveryPlanModel[] GetDeliveryPlans()
        {
            DeliveryPlanModel[] data = new DeliveryPlanModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<DeliveryPlan>();
                data = repo.GetAll().ToList().Select(d => new DeliveryPlanModel
                {
                    Id = d.Id,
                    PlanDate = d.PlanDate,
                    PlanDateStr = string.Format("{0:dd.MM.yyyy}", d.PlanDate),
                    OrderNo = d.OrderNo,
                    WorkOrderDetailId = d.WorkOrderDetailId,
                    WorkOrder = new WorkOrderDetailModel
                    {
                        ProductCode = d.WorkOrderDetail.Item.ItemNo,
                        ProductName = d.WorkOrderDetail.Item.ItemName,
                        WorkOrderId = d.WorkOrderDetail.WorkOrderId,
                        MoldId = d.WorkOrderDetail.MoldId,
                        MoldCode = d.WorkOrderDetail.Mold != null ? d.WorkOrderDetail.Mold.MoldCode : "",
                        MoldName = d.WorkOrderDetail.Mold != null ? d.WorkOrderDetail.Mold.MoldName : "",
                        CreatedDate = d.WorkOrderDetail.CreatedDate,
                        Quantity = d.WorkOrderDetail.Quantity,
                        WorkOrderNo = d.WorkOrderDetail.WorkOrder.WorkOrderNo,
                        WorkOrderDateStr = string.Format("{0:dd.MM.yyyy}", d.WorkOrderDetail.WorkOrder.WorkOrderDate),
                        FirmCode = d.WorkOrderDetail.WorkOrder.Firm != null ?
                            d.WorkOrderDetail.WorkOrder.Firm.FirmCode : "",
                        FirmName = d.WorkOrderDetail.WorkOrder.Firm != null ?
                            d.WorkOrderDetail.WorkOrder.Firm.FirmName : "",
                        WorkOrderStatus = d.WorkOrderDetail.WorkOrderStatus,
                        WorkOrderStatusStr = ((WorkOrderStatusType)d.WorkOrderDetail.WorkOrderStatus).ToCaption(),
                        CompleteQuantity = d.WorkOrderDetail.WorkOrderSerial.Count()
                    }
                }).OrderBy(d => d.OrderNo).ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }

        public BusinessResult DeletePlan(int deliveryPlanId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<DeliveryPlan>();
                var repoWorkOrderDetail = _unitOfWork.GetRepository<WorkOrderDetail>();
                var repoWorkOrder = _unitOfWork.GetRepository<WorkOrder>();

                var dbObj = repo.Get(d => d.Id == deliveryPlanId);
                if (dbObj == null)
                    throw new Exception("Silmeye çalıştığınız plan kaydı bulunamadı.");

                var dbWorkOrderDetail = dbObj.WorkOrderDetail;
                int saleOrderDetailId = dbWorkOrderDetail.SaleOrderDetailId ?? 0;

                repo.Delete(dbObj);

                // RE-ORDER DELIVERY PLANS AFTER DELETION
                var plansOfDate = repo.Filter(d => d.PlanDate == dbObj.PlanDate && d.Id != dbObj.Id)
                    .OrderBy(d => d.OrderNo).ToArray();

                int newOrderNo = 1;
                foreach (var item in plansOfDate)
                {
                    item.OrderNo = newOrderNo;
                    newOrderNo++;
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

        public DeliveryPlanModel[] GetDeliveryDateQueue(DateTime planDate)
        {
            DeliveryPlanModel[] data = new DeliveryPlanModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<DeliveryPlan>();
                data = repo.Filter(d =>
                    d.PlanDate == planDate
                ).ToList().Select(d => new DeliveryPlanModel
                {
                    Id = d.Id,
                    PlanDate = d.PlanDate,
                    OrderNo = d.OrderNo,
                    WorkOrderDetailId = d.WorkOrderDetailId,
                    WorkOrder = new WorkOrderDetailModel
                    {
                        Id = d.WorkOrderDetail.Id,
                        ProductCode = d.WorkOrderDetail.Item.ItemNo,
                        ProductName = d.WorkOrderDetail.Item.ItemName,
                        WorkOrderId = d.WorkOrderDetail.WorkOrderId,
                        MoldId = d.WorkOrderDetail.MoldId,
                        MoldCode = d.WorkOrderDetail.Mold != null ? d.WorkOrderDetail.Mold.MoldCode : "",
                        MoldName = d.WorkOrderDetail.Mold != null ? d.WorkOrderDetail.Mold.MoldName : "",
                        CreatedDate = d.WorkOrderDetail.CreatedDate,
                        Quantity = d.WorkOrderDetail.Quantity,
                        WorkOrderNo = d.WorkOrderDetail.WorkOrder.WorkOrderNo,
                        WorkOrderDateStr = string.Format("{0:dd.MM.yyyy}", d.WorkOrderDetail.WorkOrder.WorkOrderDate),
                        FirmCode = d.WorkOrderDetail.WorkOrder.Firm != null ?
                            d.WorkOrderDetail.WorkOrder.Firm.FirmCode : "",
                        FirmName = d.WorkOrderDetail.WorkOrder.Firm != null ?
                            d.WorkOrderDetail.WorkOrder.Firm.FirmName : "",
                        WorkOrderStatus = d.WorkOrderDetail.WorkOrderStatus,
                        WorkOrderStatusStr = ((WorkOrderStatusType)d.WorkOrderDetail.WorkOrderStatus).ToCaption(),
                        CompleteQuantity = d.WorkOrderDetail.WorkOrderSerial.Count()
                    }
                }).OrderBy(d => d.OrderNo).ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }
        #endregion
    }
}
