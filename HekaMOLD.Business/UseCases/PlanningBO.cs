using Heka.DataAccess.Context;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Order;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.UseCases
{
    public class PlanningBO : CoreProductionBO
    {
        #region PLANNING
        /// <summary>
        /// CREATES WORK ORDER FROM SALE ORDER OR UPDATES IT
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BusinessResult CreateMachinePlan(ItemOrderDetailModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<WorkOrder>();
                var repoDetails = _unitOfWork.GetRepository<WorkOrderDetail>();
                var repoOrder = _unitOfWork.GetRepository<ItemOrder>();
                var repoOrderDetail = _unitOfWork.GetRepository<ItemOrderDetail>();
                var repoItem = _unitOfWork.GetRepository<Item>();
                var repoMachinePlan = _unitOfWork.GetRepository<MachinePlan>();

                var dbSaleOrderDetail = repoOrderDetail.Get(d => d.Id == model.Id);
                if (dbSaleOrderDetail == null)
                    throw new Exception("Planlanması istenen siparişin kaydına ulaşılamadı.");

                dbSaleOrderDetail.MapTo(model);

                var dbObjDetail = repoDetails.Get(d => d.Id == model.Id);
                if (dbObjDetail == null)
                {
                    dbObjDetail = new WorkOrderDetail();
                    dbObjDetail.CreatedDate = DateTime.Now;
                    dbObjDetail.CreatedUserId = model.CreatedUserId;
                    dbObjDetail.WorkOrderStatus = (int)WorkOrderStatusType.Planned;
                    repoDetails.Add(dbObjDetail);

                    // SAVE RELATION
                    dbSaleOrderDetail.WorkOrderDetail.Add(dbObjDetail);

                    dbObjDetail.WorkOrder = new WorkOrder
                    {
                        WorkOrderNo = GetNextWorkOrderNo(),
                        WorkOrderDate = DateTime.Now,
                        CreatedDate = DateTime.Now,
                        CreatedUserId = model.CreatedUserId,
                        DocumentNo = "",
                        Explanation = "",
                        WorkOrderStatus = (int)WorkOrderStatusType.Planned,
                        FirmId = dbSaleOrderDetail.ItemOrder.FirmId,
                        PlantId = dbSaleOrderDetail.ItemOrder.PlantId,
                    };
                    repo.Add(dbObjDetail.WorkOrder);
                }

                var crDate = dbObjDetail.CreatedDate;
                var crUser = dbObjDetail.CreatedUserId;

                model.MapTo(dbObjDetail);

                if (dbObjDetail.CreatedDate == null)
                    dbObjDetail.CreatedDate = crDate;
                if (dbObjDetail.CreatedUserId == null)
                    dbObjDetail.CreatedUserId = crUser;

                dbObjDetail.UpdatedDate = DateTime.Now;
                dbObjDetail.UpdatedUserId = model.CreatedUserId;
                dbObjDetail.SaleOrderDetailId = dbSaleOrderDetail.Id;

                #region CHECK SALE ORDER REMAINING QUANTITY & STATUS
                var saleOrderPlannedQuantity = dbSaleOrderDetail.WorkOrderDetail
                    .Sum(d => d.Quantity);
                if (dbSaleOrderDetail.Quantity <= saleOrderPlannedQuantity)
                    dbSaleOrderDetail.OrderStatus = (int)OrderStatusType.Planned;
                else
                    dbSaleOrderDetail.OrderStatus = (int)OrderStatusType.Approved;
                #endregion

                #region CHECK/ADD MACHINE PLAN QUEUE
                var dbMachinePlan = repoMachinePlan.Get(d => d.WorkOrderDetailId == dbObjDetail.Id);
                if (dbMachinePlan == null)
                {
                    int? lastOrderNo = repoMachinePlan.Filter(d => d.MachineId == model.MachineId)
                        .Max(d => d.OrderNo);
                    if ((lastOrderNo ?? 0) == 0)
                        lastOrderNo = 0;

                    lastOrderNo++;

                    dbMachinePlan = new MachinePlan
                    {
                        WorkOrderDetail = dbObjDetail,
                        MachineId = model.MachineId,
                        OrderNo = lastOrderNo
                    };
                    repoMachinePlan.Add(dbMachinePlan);
                }
                #endregion

                _unitOfWork.SaveChanges();

                result.Result = true;
                result.RecordId = dbObjDetail.Id;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult ReOrderPlan(MachinePlanModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<MachinePlan>();
                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                    throw new Exception("İşlem yapmak istediğiniz üretim planı bulunamadı.");

                // REORDER EX MACHINE PLAN LIST
                if (dbObj.MachineId != model.MachineId)
                {
                    var exMachinePlans = repo.Filter(d => d.MachineId == dbObj.MachineId &&
                        (
                            d.WorkOrderDetail.WorkOrderStatus == (int)WorkOrderStatusType.Planned
                            ||
                            d.WorkOrderDetail.WorkOrderStatus == (int)WorkOrderStatusType.InProgress
                        )
                            && d.Id != dbObj.Id)
                        .OrderBy(d => d.OrderNo)
                        .ToArray();

                    var orderNo = 1;
                    foreach (var item in exMachinePlans)
                    {
                        item.OrderNo = orderNo++;
                    }
                }

                dbObj.MachineId = model.MachineId;
                var activePlans = repo.Filter(d => d.MachineId == dbObj.MachineId &&
                    (
                        d.WorkOrderDetail.WorkOrderStatus == (int)WorkOrderStatusType.Planned
                        ||
                        d.WorkOrderDetail.WorkOrderStatus == (int)WorkOrderStatusType.InProgress
                    )
                ).ToArray();

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

        public ItemOrderDetailModel[] GetWaitingSaleOrders()
        {
            ItemOrderDetailModel[] data = new ItemOrderDetailModel[0];

            var repo = _unitOfWork.GetRepository<ItemOrderDetail>();
            data = repo.Filter(d =>
                d.ItemOrder.OrderType == (int)ItemOrderType.Sale &&
                (
                    d.OrderStatus == (int)OrderStatusType.Created
                    ||
                    d.OrderStatus == (int)OrderStatusType.Approved
                )
            ).ToList().Select(d => new ItemOrderDetailModel { 
                Id = d.Id,
                OrderDateStr = string.Format("{0:dd.MM.yyyy}", d.ItemOrder.OrderDate),
                ItemNo = d.Item != null ? d.Item.ItemNo : "",
                FirmName = d.ItemOrder.Firm != null ? 
                    d.ItemOrder.Firm.FirmName : "",
                Quantity = d.Quantity
            }).ToArray();

            return data;
        }

        public MachinePlanModel[] GetProductionPlans()
        {
            MachinePlanModel[] data = new MachinePlanModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<MachinePlan>();
                data = repo.GetAll().ToList().Select(d => new MachinePlanModel
                {
                    Id = d.Id,
                    MachineId = d.MachineId,
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

        public BusinessResult DeletePlan(int machinePlanId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<MachinePlan>();
                var repoWorkOrderDetail = _unitOfWork.GetRepository<WorkOrderDetail>();
                var repoWorkOrder = _unitOfWork.GetRepository<WorkOrder>();
                var repoItemOrderDetail = _unitOfWork.GetRepository<ItemOrderDetail>();

                var dbObj = repo.Get(d => d.Id == machinePlanId);
                if (dbObj == null)
                    throw new Exception("Silmeye çalıştığınız plan kaydı bulunamadı.");

                if (dbObj.WorkOrderDetail.WorkOrderStatus <= (int)WorkOrderStatusType.Planned)
                {
                    var dbWorkOrderDetail = dbObj.WorkOrderDetail;
                    int saleOrderDetailId = dbWorkOrderDetail.SaleOrderDetailId ?? 0;

                    repo.Delete(dbObj);

                    var dbWorkOrder = dbWorkOrderDetail.WorkOrder;
                    repoWorkOrderDetail.Delete(dbWorkOrderDetail);

                    if (!dbWorkOrder.WorkOrderDetail.Any(d => d.Id != dbWorkOrderDetail.Id))
                        repoWorkOrder.Delete(dbWorkOrder);

                    // UPDATE SALE ORDER STATUS FROM PLANNED TO APPROVED
                    var dbItemOrderDetail = repoItemOrderDetail.Get(d => d.Id == saleOrderDetailId);
                    if (dbItemOrderDetail != null)
                    {
                        dbItemOrderDetail.OrderStatus = (int)OrderStatusType.Approved;

                        var dbItemOrder = dbItemOrderDetail.ItemOrder;
                        if (!dbItemOrder.ItemOrderDetail.Any(d => d.Id != dbItemOrderDetail.Id
                            && d.OrderStatus > (int)OrderStatusType.Approved))
                            dbItemOrder.OrderStatus = (int)OrderStatusType.Approved;
                    }

                    // RE-ORDER MACHINE PLANS AFTER DELETION
                    var plansOfMachine = repo.Filter(d => d.MachineId == dbObj.MachineId && d.Id != dbObj.Id)
                        .OrderBy(d => d.OrderNo).ToArray();

                    int newOrderNo = 1;
                    foreach (var item in plansOfMachine)
                    {
                        item.OrderNo = newOrderNo;
                        newOrderNo++;
                    }

                    _unitOfWork.SaveChanges();
                }
                else
                    throw new Exception("İşleme başlanmış veya tamamlanmış olan bir plan kaydını silemezsiniz.");

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
