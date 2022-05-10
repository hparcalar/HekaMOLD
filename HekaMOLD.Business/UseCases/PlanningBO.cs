using Heka.DataAccess.Context;
using Heka.DataAccess.Context.Models;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Logistics;
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
                var repoMoldTest = _unitOfWork.GetRepository<MoldTest>();

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
                        FirmId = dbSaleOrderDetail.ItemOrder.CustomerFirmId,
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
                dbObjDetail.MachineId = model.MachineId;

                #region ASSIGN PROPER MOLD TEST DATA TO WORK ORDER
                var dbProduct = repoItem.Get(d => d.Id == (dbObjDetail.ItemId ?? 0));
                if (dbProduct != null)
                {
                    var dbMoldTest = repoMoldTest.Filter(d => d.ProductCode == dbProduct.ItemNo)
                        .OrderByDescending(d => d.Id).FirstOrDefault();
                    if (dbMoldTest != null)
                    {
                        dbObjDetail.MoldTestId = dbMoldTest.Id;
                        dbObjDetail.RawGr = dbMoldTest.RawMaterialGr;
                        dbObjDetail.RawGrToleration = dbMoldTest.RawMaterialTolerationGr;
                        dbObjDetail.InPackageQuantity = dbMoldTest.InPackageQuantity;
                        dbObjDetail.InPalletPackageQuantity = dbMoldTest.InPalletPackageQuantity;
                        dbObjDetail.InflationTimeSeconds = dbMoldTest.InflationTimeSeconds;
                        dbObjDetail.MoldId = dbMoldTest.MoldId;
                    }
                }
                #endregion

                #region CHECK SALE ORDER REMAINING QUANTITY & STATUS
                var saleOrderPlannedQuantity = dbSaleOrderDetail.WorkOrderDetail
                    .Sum(d => d.Quantity);
                if (dbSaleOrderDetail.Quantity <= saleOrderPlannedQuantity)
                {
                    dbSaleOrderDetail.OrderStatus = (int)OrderStatusType.Planned;
                    dbSaleOrderDetail.ItemOrder.OrderStatus = (int)OrderStatusType.Planned;
                }
                else
                {
                    dbSaleOrderDetail.OrderStatus = (int)OrderStatusType.Approved;
                    dbSaleOrderDetail.ItemOrder.OrderStatus = (int)OrderStatusType.Approved;
                }
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

        public BusinessResult CreateMachinePlan(int workOrderDetailId, int machineId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<MachinePlan>();
                var repoDetail = _unitOfWork.GetRepository<WorkOrderDetail>();

                var dbDetail = repoDetail.Get(d => d.Id == workOrderDetailId);

                // CREATE MACHINE PLAN
                var dbMachinePlan = repo.Get(d => d.WorkOrderDetailId == workOrderDetailId);
                if (dbMachinePlan == null)
                {
                    int? lastOrderNo = repo.Filter(d => d.MachineId == machineId)
                        .Max(d => d.OrderNo);
                    if ((lastOrderNo ?? 0) == 0)
                        lastOrderNo = 0;

                    lastOrderNo++;

                    dbMachinePlan = new MachinePlan
                    {
                        WorkOrderDetail = dbDetail,
                        MachineId = machineId,
                        OrderNo = lastOrderNo
                    };
                    repo.Add(dbMachinePlan);
                }

                _unitOfWork.SaveChanges();

                result.RecordId = dbMachinePlan.Id;
                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }
        public BusinessResult CopyFromWorkOrder(int fromPlanId, int quantity,
            int firmId, int targetMachineId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<MachinePlan>();
                var repoWorkOrder = _unitOfWork.GetRepository<WorkOrder>();
                var repoWorkOrderDetail = _unitOfWork.GetRepository<WorkOrderDetail>();

                var dbExistingPlan = repo.Get(d => d.Id == fromPlanId);
                if (dbExistingPlan == null)
                    throw new Exception("Kopyalanması istenen plan bilgisine ulaşılamadı.");
                
                // CREATE WORK ORDER
                var newWorkOrder = new WorkOrder
                {
                    FirmId = firmId,
                    CreatedDate = DateTime.Now,
                    WorkOrderDate = DateTime.Now,
                    DocumentNo = "",
                    WorkOrderNo = GetNextWorkOrderNo(),
                    Explanation = dbExistingPlan.WorkOrderDetail.WorkOrder.Explanation,
                    WorkOrderStatus = (int)WorkOrderStatusType.Planned,
                    PlantId = dbExistingPlan.WorkOrderDetail.WorkOrder.PlantId,
                };
                repoWorkOrder.Add(newWorkOrder);

                // CREATE WORK ORDER DETAIL
                var newWorkOrderDetail = new WorkOrderDetail
                {
                    WorkOrder = newWorkOrder,
                    CreatedDate = DateTime.Now,
                    Dye = dbExistingPlan.WorkOrderDetail.Dye,
                    InflationTimeSeconds = dbExistingPlan.WorkOrderDetail.InflationTimeSeconds,
                    InPackageQuantity = dbExistingPlan.WorkOrderDetail.InPackageQuantity,
                    InPalletPackageQuantity = dbExistingPlan.WorkOrderDetail.InPalletPackageQuantity,
                    ItemId = dbExistingPlan.WorkOrderDetail.ItemId,
                    MachineId = targetMachineId,
                    MoldId = dbExistingPlan.WorkOrderDetail.MoldId,
                    MoldTestId = dbExistingPlan.WorkOrderDetail.MoldTestId,
                    Quantity = quantity,
                    RawGr = dbExistingPlan.WorkOrderDetail.RawGr,
                    RawGrToleration = dbExistingPlan.WorkOrderDetail.RawGrToleration,
                    WorkOrderStatus = (int)WorkOrderStatusType.Planned,
                };
                repoWorkOrderDetail.Add(newWorkOrderDetail);

                // CREATE MACHINE PLAN
                var dbMachinePlan = repo.Get(d => d.WorkOrderDetailId == newWorkOrderDetail.Id);
                if (dbMachinePlan == null)
                {
                    int? lastOrderNo = repo.Filter(d => d.MachineId == targetMachineId)
                        .Max(d => d.OrderNo);
                    if ((lastOrderNo ?? 0) == 0)
                        lastOrderNo = 0;

                    lastOrderNo++;

                    dbMachinePlan = new MachinePlan
                    {
                        WorkOrderDetail = newWorkOrderDetail,
                        MachineId = targetMachineId,
                        OrderNo = lastOrderNo
                    };
                    repo.Add(dbMachinePlan);
                }

                _unitOfWork.SaveChanges();

                result.RecordId = dbMachinePlan.Id;
                result.Result = true;
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
                dbObj.WorkOrderDetail.MachineId = model.MachineId;

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

                if (activePlans.Any(d => d.OrderNo == dbObj.OrderNo && d.Id != dbObj.Id))
                {
                    var sameData = activePlans.Where(d => d.OrderNo == dbObj.OrderNo && d.Id != dbObj.Id);
                    foreach (var smData in sameData)
                    {
                        smData.OrderNo++;
                    }
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
                ItemName = d.Item != null ? d.Item.ItemName : "",
                FirmName = d.ItemOrder.Firm != null ? 
                    d.ItemOrder.Firm.FirmName : "",
                Quantity = d.Quantity,
                DeadlineDateStr = d.ItemOrder.DateOfNeed != null ?
                    string.Format("{0:dd.MM.yyyy}", d.ItemOrder.DateOfNeed) : "",
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
                        ProductCode = d.WorkOrderDetail.Item != null ? d.WorkOrderDetail.Item.ItemNo : "",
                        ProductName = d.WorkOrderDetail.Item != null ? d.WorkOrderDetail.Item.ItemName : d.WorkOrderDetail.TrialProductName,
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
                            d.WorkOrderDetail.WorkOrder.Firm.FirmName : d.WorkOrderDetail.WorkOrder.TrialFirmName,
                        WorkOrderStatus = d.WorkOrderDetail.WorkOrderStatus,
                        WorkOrderStatusStr = ((WorkOrderStatusType)d.WorkOrderDetail.WorkOrderStatus).ToCaption(),
                        WastageQuantity = d.WorkOrderDetail.ProductWastage.Sum(m => m.Quantity) ?? 0,
                        CompleteQuantity = Convert.ToInt32(d.WorkOrderDetail.WorkOrderSerial
                            .Where(m => m.SerialNo != null && m.SerialNo.Length > 0)
                            .Sum(m => m.FirstQuantity) ?? 0),
                        WorkOrderType = d.WorkOrderDetail.WorkOrderType ?? 1,
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

        public MachinePlanModel[] GetMachineQueue(int machineId)
        {
            MachinePlanModel[] data = new MachinePlanModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<MachinePlan>();
                data = repo.Filter(d => 
                    (d.WorkOrderDetail.WorkOrderStatus == (int)WorkOrderStatusType.Created ||
                    d.WorkOrderDetail.WorkOrderStatus == (int)WorkOrderStatusType.Planned ||
                    d.WorkOrderDetail.WorkOrderStatus == (int)WorkOrderStatusType.InProgress ||
                    d.WorkOrderDetail.WorkOrderStatus == (int)WorkOrderStatusType.OnHold)
                    &&
                    d.MachineId == machineId
                ).ToList().Select(d => new MachinePlanModel
                {
                    Id = d.Id,
                    MachineId = d.MachineId,
                    OrderNo = d.OrderNo,
                    WorkOrderDetailId = d.WorkOrderDetailId,
                    WorkOrder = new WorkOrderDetailModel
                    {
                        Id= d.WorkOrderDetail.Id,
                        ProductCode = d.WorkOrderDetail.Item != null ? d.WorkOrderDetail.Item.ItemNo : "",
                        ProductName = d.WorkOrderDetail.Item != null ? d.WorkOrderDetail.Item.ItemName : d.WorkOrderDetail.TrialProductName,
                        Explanation = d.WorkOrderDetail.WorkOrder.Explanation,
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
                            d.WorkOrderDetail.WorkOrder.Firm.FirmName : d.WorkOrderDetail.WorkOrder.TrialFirmName,
                        WorkOrderStatus = d.WorkOrderDetail.WorkOrderStatus,
                        WorkOrderStatusStr = ((WorkOrderStatusType)d.WorkOrderDetail.WorkOrderStatus).ToCaption(),
                        CompleteQuantity = d.WorkOrderDetail.MachineSignal.Any() ?
                            d.WorkOrderDetail.MachineSignal.Where(m => m.SignalStatus == 1).Count() :
                            Convert.ToInt32(d.WorkOrderDetail.WorkOrderSerial.Sum(m => m.FirstQuantity ?? 0))
                    }
                }).OrderBy(d => d.OrderNo).ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }

        public WorkOrderDetailModel GetWorkOrderDetail(int workOrderDetailId)
        {
            WorkOrderDetailModel data = new WorkOrderDetailModel();

            try
            {
                var repo = _unitOfWork.GetRepository<WorkOrderDetail>();
                var dbObj = repo.Get(d => d.Id == workOrderDetailId);
                if (dbObj != null)
                {
                    data.Id = workOrderDetailId;
                    data.WorkOrderNo = dbObj.WorkOrder.WorkOrderNo;
                    data.FirmCode = dbObj.WorkOrder.Firm != null ? dbObj.WorkOrder.Firm.FirmCode : "";
                    data.FirmName = dbObj.WorkOrder.Firm != null ? dbObj.WorkOrder.Firm.FirmName : dbObj.WorkOrder.TrialFirmName;
                    data.ItemOrderDocumentNo = dbObj.ItemOrderDetail != null ? dbObj.ItemOrderDetail.ItemOrder.DocumentNo : "";
                    data.ProductCode = dbObj.Item != null ? dbObj.Item.ItemNo : "";
                    data.Explanation = dbObj.WorkOrder.Explanation;
                    data.ProductName = dbObj.Item != null ? dbObj.Item.ItemName : dbObj.TrialProductName;
                    data.Quantity = dbObj.Quantity;
                    data.CompleteQuantity = Convert.ToInt32(dbObj.MachineSignal.Any() ?
                        dbObj.MachineSignal.Where(m => m.SignalStatus == 1).Count() :
                        dbObj.WorkOrderSerial.Sum(d => d.FirstQuantity) ?? 0);
                    data.MoldCode = dbObj.Mold != null ? dbObj.Mold.MoldCode : "";
                    data.MoldName = dbObj.Mold != null ? dbObj.Mold.MoldName : "";
                    data.WastageQuantity = dbObj.ProductWastage.Sum(d => d.Quantity) ?? 0;
                    data.OrderDeadline = dbObj.ItemOrderDetail != null &&
                            dbObj.ItemOrderDetail.ItemOrder.DateOfNeed != null ?
                                string.Format("{0:dd.MM.yyyy}", dbObj.ItemOrderDetail.ItemOrder.DateOfNeed) : "";
                }
            }
            catch (Exception)
            {

            }

            return data;
        }

        public BusinessResult EditWorkOrder(WorkOrderDetailModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<WorkOrderDetail>();
                var repoSerial = _unitOfWork.GetRepository<WorkOrderSerial>();
                var repoWastage = _unitOfWork.GetRepository<ProductWastage>();
                var repoShift = _unitOfWork.GetRepository<Shift>();

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                    throw new Exception("Düzenlenecek iş emri bilgisi bulunamadı.");

                // SET DESCRIPTION
                dbObj.WorkOrder.Explanation = model.Explanation;

                // SET TARGET COUNT
                dbObj.Quantity = model.Quantity;

                // UPDATE PRODUCED SERIALS
                //int exComplete = dbObj.WorkOrderSerial.Sum(d => Convert.ToInt32(d.FirstQuantity ?? 0));
                //if (exComplete < model.CompleteQuantity)
                //{
                //    int diffQty = model.CompleteQuantity - exComplete;
                //    for (int i = 0; i < diffQty; i++)
                //    {
                //        WorkOrderSerial newSerial = new WorkOrderSerial();
                //        newSerial.SerialNo = "";
                //        newSerial.FirstQuantity = 1;
                //        newSerial.LiveQuantity = 1;
                //        newSerial.CreatedDate = DateTime.Now;
                //        newSerial.IsGeneratedBySignal = false;
                //        newSerial.SerialType = (int)WorkOrderSerialType.ProductPackage;
                //        newSerial.SerialStatus = (int)SerialStatusType.Created;
                //        newSerial.WorkOrderDetail = dbObj;
                //        repoSerial.Add(newSerial);
                //    }
                //}
                //else if (exComplete > 0 && model.CompleteQuantity > -1 && exComplete > model.CompleteQuantity)
                //{
                //    var oldSerials = dbObj.WorkOrderSerial.ToArray();

                //    int diffQty = exComplete - model.CompleteQuantity;
                //    for (int i = 0; i < diffQty; i++)
                //    {
                //        if (oldSerials.Length > i)
                //            repoSerial.Delete(oldSerials[i]);
                //    }
                //}

                // UPDATE WASTAGE DATA
                decimal exWst = dbObj.ProductWastage.Sum(d => d.Quantity) ?? 0;
                if (exWst < model.WastageQuantity)
                {
                    // RESOLVE CURRENT SHIFT
                    DateTime entryTime = DateTime.Now;
                    Shift dbShift = null;
                    var shiftList = repoShift.Filter(d => d.StartTime != null && d.EndTime != null).ToArray();
                    foreach (var shift in shiftList)
                    {
                        DateTime startTime = DateTime.Now.Date.Add(shift.StartTime.Value);
                        DateTime endTime = DateTime.Now.Date.Add(shift.EndTime.Value);

                        if (shift.StartTime > shift.EndTime)
                        {
                            if (DateTime.Now.Hour >= shift.StartTime.Value.Hours)
                                endTime = DateTime.Now.Date.AddDays(1).Add(shift.EndTime.Value);
                            else
                                startTime = DateTime.Now.Date.AddDays(-1).Add(shift.StartTime.Value);
                        }

                        if (entryTime >= startTime && entryTime <= endTime)
                        {
                            dbShift = shift;
                            break;
                        }
                    }

                    // ADD NEW WASTAGE
                    decimal diffQty = (model.WastageQuantity ?? 0) - exWst;

                    ProductWastage newWst = new ProductWastage
                    {
                        WorkOrderDetail = dbObj,
                        ProductId = dbObj.ItemId,
                        CreatedDate = DateTime.Now,
                        MachineId = dbObj.MachineId,
                        EntryDate = DateTime.Now,
                        Quantity = diffQty,
                        Shift = dbShift,
                        WastageStatus = 0,
                    };
                    repoWastage.Add(newWst);
                }
                else if (exWst > 0 && model.WastageQuantity > -1 && exWst > model.WastageQuantity)
                {
                    var oldWastages = dbObj.ProductWastage.OrderByDescending(d => d.Id).ToArray();

                    decimal diffQty = exWst - (model.WastageQuantity ?? 0);

                    foreach (var oldWst in oldWastages)
                    {
                        if (diffQty <= 0)
                            break;

                        if (oldWst.Quantity > diffQty)
                        {
                            oldWst.Quantity -= diffQty;
                            diffQty = 0;
                        }
                        else if (oldWst.Quantity == diffQty)
                        {
                            repoWastage.Delete(oldWst);
                            diffQty = 0;
                        }
                        else if (oldWst.Quantity < diffQty)
                        {
                            diffQty -= oldWst.Quantity ?? 0;
                            repoWastage.Delete(oldWst);
                        }
                    }
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

        public BusinessResult CompleteWorkOrder(int workOrderDetailId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<WorkOrderDetail>();
                var repoPlan = _unitOfWork.GetRepository<MachinePlan>();

                var dbObj = repo.Get(d => d.Id == workOrderDetailId);
                if (dbObj == null)
                    throw new Exception("İş emri kaydı bulunamadı.");

                dbObj.WorkOrderStatus = (int)WorkOrderStatusType.Completed;

                if (dbObj.MachinePlan.Any())
                {
                    var plans = dbObj.MachinePlan.ToArray();

                    foreach (var plan in plans)
                    {
                        repoPlan.Delete(plan);
                    }
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

        public BusinessResult HoldWorkOrder(int workOrderDetailId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<WorkOrderDetail>();
                var dbObj = repo.Get(d => d.Id == workOrderDetailId);
                if (dbObj == null)
                    throw new Exception("Durumu değiştirilmek istenen iş emri kaydına ulaşılamadı.");

                dbObj.WorkOrderStatus = (int)WorkOrderStatusType.OnHold;

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
        public BusinessResult ToggleWorkOrderStatus(int workOrderDetailId, int? userId = null)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<WorkOrderDetail>();
                var repoUserHistory = _unitOfWork.GetRepository<UserWorkOrderHistory>();
                var repoPosture = _unitOfWork.GetRepository<ProductionPosture>();

                var dbObj = repo.Get(d => d.Id == workOrderDetailId);
                if (dbObj == null)
                    throw new Exception("Durumu değiştirilmek istenen iş emri kaydına ulaşılamadı.");

                if (dbObj.WorkOrderStatus == (int)WorkOrderStatusType.Planned 
                    || dbObj.WorkOrderStatus == (int)WorkOrderStatusType.Created 
                    || dbObj.WorkOrderStatus == (int)WorkOrderStatusType.OnHold)
                {
                    //if (repo.Any(d => d.MachineId == dbObj.MachineId
                    //    && d.Id != dbObj.Id
                    //    && d.WorkOrderStatus == (int)WorkOrderStatusType.InProgress))
                    //    throw new Exception("Bu makinede zaten bir aktif üretim mevcuttur. Önce aktif işi bitirip sonra yenisine başlayabilirsiniz.");

                    // CHECK IF THERE IS AN ONGOING POSTURE THEN STOP IT
                    if (repoPosture.Any(d => d.MachineId == dbObj.MachineId && d.PostureStatus != (int)PostureStatusType.Resolved))
                    {
                        var dbOngoingPostureList = repoPosture.Filter(d => d.MachineId == dbObj.MachineId
                            && d.PostureStatus != (int)PostureStatusType.Resolved).ToArray();
                        foreach (var dbOngoingPosture in dbOngoingPostureList)
                        {
                            dbOngoingPosture.PostureStatus = (int)PostureStatusType.Resolved;
                            dbOngoingPosture.EndDate = DateTime.Now;
                            dbOngoingPosture.UpdatedUserId = userId;
                        }
                    }

                    dbObj.WorkOrderStatus = (int)WorkOrderStatusType.InProgress;
                }
                else
                {
                    if ((dbObj.WorkOrderSerial.Sum(d => d.FirstQuantity) ?? 0) < dbObj.Quantity)
                        dbObj.WorkOrderStatus = (int)WorkOrderStatusType.Cancelled;
                    else
                        dbObj.WorkOrderStatus = (int)WorkOrderStatusType.Completed;
                }

                _unitOfWork.SaveChanges();

                // MOVED TO USER LOGIN WITH MACHINE SELECTION
                //using (ProductionBO bObj = new ProductionBO())
                //{
                //    bObj.UpdateUserHistory(dbObj.MachineId ?? 0, userId ?? 0);
                //}

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

        #region LOGISTICS PLAN
        public ItemLoadModel[] GetWaitingLoads()
        {
            ItemLoadModel[] data = new ItemLoadModel[0];

            var repo = _unitOfWork.GetRepository<ItemLoad>();
            data = repo.Filter(d =>
                (
                    (d.LoadStatusType == (int)LoadStatusType.Ready || d.LoadStatusType == (int)LoadStatusType.InWarehouse || d.LoadStatusType == (int)LoadStatusType.ToBeLoadedFromCustomer)
                    && (d.VoyageConverted == null || d.VoyageConverted == false)
                )
            ).ToList().Select(d => new ItemLoadModel
            {
                Id = d.Id,
                LoadCode = d.LoadCode,
                LoadingDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadingDate),
                LoadOutDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadOutDate),
                LoadingDate = d.LoadingDate,
                CustomerFirmName = d.FirmCustomer != null ? d.FirmCustomer.FirmName :"",
                ShipperFirmName = d.FirmShipper != null ? d.FirmShipper.FirmName : "",
                BuyerFirmName = d.FirmBuyer != null ? d.FirmBuyer.FirmName : "",
                //OrderTransactionDirectionType = d.OrderTransactionDirectionType,
                //OrderTransactionDirectionTypeStr = d.OrderTransactionDirectionType != null ? ((OrderTransactionDirectionType) d.OrderTransactionDirectionType).ToCaption():"",
                //OrderCalculationType = d.OrderCalculationType,
                //OrderCalculationTypeStr = d.OrderCalculationType != null ?((OrderCalculationType) d.OrderCalculationType).ToCaption():"",
                OveralQuantity = d.OveralQuantity,
                OveralWeight = d.OveralWeight,
                OveralLadametre = d.OveralLadametre,
                OveralVolume = d.OveralVolume,
                OverallTotal = d.OverallTotal,
                //ForexTypeCode = d.ForexType != null ? d.ForexType.ForexTypeCode : "",
                //OrderNo = d.OrderNo,
                LoadDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadDate),
                T1T2StartDateStr = string.Format("{0:dd.MM.yyyy}", d.T1T2StartDate),
                DischargeDateStr = string.Format("{0:dd.MM.yyyy}", d.DischargeDate),
                //CalculationTypePrice = d.CalculationTypePrice,
                //DocumentNo = d.DocumentNo,
                OrderUploadType = d.OrderUploadType,
                //OrderUploadTypeStr = d.OrderUploadType != null ? ((OrderUploadType) d.OrderUploadType).ToCaption():"",
                OrderUploadPointType = d.OrderUploadPointType,
                OrderUploadPointTypeStr = d.OrderUploadPointType == 1 ? "Müşteriden Yükleme" : d.OrderUploadPointType == 2 ? "Depodan Yükleme" : "",
                //ScheduledUploadDateStr = string.Format("{0:dd.MM.yyyy}", d.ScheduledUploadDate),
                //DateOfNeedStr = string.Format("{0:dd.MM.yyyy}", d.DateOfNeed),
                //InvoiceId = d.InvoiceId,
                ForexTypeId = d.ForexTypeId,
                //VehicleTraillerId = d.VehicleTraillerId,
                //InvoiceStatus = d.InvoiceStatus,
                //InvoiceFreightPrice = d.InvoiceFreightPrice,
                CmrNo = d.CmrNo,
                CmrStatus = d.CmrStatus,
                ShipperFirmExplanation = d.ShipperFirmExplanation,
                BuyerFirmExplanation = d.BuyerFirmExplanation,
                //ReadinessDateStr = string.Format("{0:dd.MM.yyyy}", d.ReadinessDate),
                //DeliveryFromCustomerDateStr = string.Format("{0:dd.MM.yyyy}", d.DeliveryFromCustomerDate),
                //IntendedArrivalDateStr = string.Format("{0:dd.MM.yyyy}", d.IntendedArrivalDate),
                FirmCustomsArrivalId = d.FirmCustomsArrivalId,
                CustomsExplanation = d.CustomsExplanation,
                T1T2No = d.T1T2No,
                TClosingDateStr = string.Format("{0:dd.MM.yyyy}", d.TClosingDate),
                //HasCmrDeliveryed = d.HasCmrDeliveryed,
                //ItemPrice = d.ItemPrice,
                //TrailerType = d.TrailerType,
                //HasItemInsurance = d.HasItemInsurance,
                //HasItemDangerous = d.HasItemDangerous,
                CmrCustomerDeliveryDateStr = string.Format("{0:dd.MM.yyyy}", d.CmrCustomerDeliveryDate),
                //BringingToWarehousePlate = d.BringingToWarehousePlate,
                ShipperCityId = d.ShipperCityId,
                ShipperCityName = d.CityShipper != null ? d.CityShipper.CityName : "",
                BuyerCityId = d.BuyerCityId,
                BuyerCityName = d.CityBuyer != null ? d.CityBuyer.CityName : "",
                ShipperCountryId = d.ShipperCountryId,
                ShipperCountryName = d.CountryShipper != null ? d.CountryShipper.CountryName : "",
                BuyerCountryId = d.BuyerCountryId,
                BuyerCountryName = d.CountryBuyer != null ? d.CountryBuyer.CountryName : "",
                CustomerFirmId = d.CustomerFirmId,
                ShipperFirmId = d.ShipperFirmId,
                BuyerFirmId = d.BuyerFirmId,
                EntryCustomsId = d.EntryCustomsId,
                ExitCustomsId = d.ExitCustomsId,
                CmrBuyerCountryId =d.CmrBuyerCountryId,
                CmrBuyerFirmId = d.CmrBuyerFirmId,
                CmrBuyerCityId = d.CmrBuyerCityId,
                CmrShipperCityId  =d.CmrShipperCityId,
                CmrShipperCountryId = d.CmrShipperCountryId,
                FirmCustomsExitId = d.FirmCustomsExitId,
                CmrShipperFirmId =d.CmrShipperFirmId,
                ManufacturerFirmId =d.ManufacturerFirmId,
                ReelOwnerFirmId =d.ReelOwnerFirmId,
                DeclarationX1No = d.DeclarationX1No,
                ShipperFirmAddress = d.ShipperFirmAddress,
                BuyerFirmAddress = d.BuyerFirmAddress,
                //PlantId = d.PlantId
            }).ToArray();

            return data;
        }
        #endregion
    }
}
