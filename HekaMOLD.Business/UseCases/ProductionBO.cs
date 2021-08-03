using Heka.DataAccess.Context;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
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
    public class ProductionBO : CoreProductionBO
    {
        #region WORK ORDER & DETAILS
        public WorkOrderDetailModel[] GetWorkOrderDetailList()
        {
            List<WorkOrderDetailModel> data = new List<WorkOrderDetailModel>();

            var repo = _unitOfWork.GetRepository<WorkOrderDetail>();

            repo.GetAll().ToList().ForEach(d =>
            {
                WorkOrderDetailModel containerObj = new WorkOrderDetailModel();
                d.MapTo(containerObj);

                containerObj.ProductCode = d.Item != null ? d.Item.ItemNo : "";
                containerObj.ProductName = d.Item != null ? d.Item.ItemName : "";
                containerObj.WorkOrderDateStr = d.WorkOrder.WorkOrderDate != null ? 
                    string.Format("{0:dd.MM.yyyy}", d.WorkOrder.WorkOrderDate) : "";
                containerObj.WorkOrderNo = d.WorkOrder.WorkOrderNo;
                containerObj.FirmCode = d.WorkOrder.Firm != null ? d.WorkOrder.Firm.FirmCode : "";
                containerObj.FirmName = d.WorkOrder.Firm != null ? d.WorkOrder.Firm.FirmName : "";
                containerObj.DyeCode = d.Dye != null ? d.Dye.DyeCode : "";
                containerObj.RalCode = d.Dye != null ? d.Dye.RalCode : "";
                containerObj.DyeName = d.Dye != null ? d.Dye.DyeName : "";
                containerObj.MachineCode = d.Machine != null ? d.Machine.MachineCode : "";
                containerObj.MachineName = d.Machine != null ? d.Machine.MachineName : "";
                containerObj.SaleOrderDocumentNo = d.ItemOrderDetail != null ? d.ItemOrderDetail.ItemOrder.DocumentNo : "";
                containerObj.SaleOrderReceiptNo = d.ItemOrderDetail != null ? d.ItemOrderDetail.ItemOrder.OrderNo : "";
                containerObj.SaleOrderDate = d.ItemOrderDetail != null ?
                    string.Format("{0:dd.MM.yyyy}", d.ItemOrderDetail.ItemOrder.OrderDate) : "";
                containerObj.SaleOrderDeadline = d.ItemOrderDetail != null ?
                    string.Format("", d.ItemOrderDetail.ItemOrder.DateOfNeed) : "";

                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult DeleteWorkOrderDetail(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<WorkOrder>();
                var repoDetails = _unitOfWork.GetRepository<WorkOrderDetail>();
                var repoMachinePlan = _unitOfWork.GetRepository<MachinePlan>();

                var dbObjDetail = repoDetails.Get(d => d.Id == id);
                if (dbObjDetail == null)
                    throw new Exception("Silinmesi istenen üretim emri bilgisine ulaşılamadı.");

                if (!dbObjDetail.WorkOrder.WorkOrderDetail.Any(d => d.Id != dbObjDetail.Id))
                    repo.Delete(dbObjDetail.WorkOrder);

                if (repoMachinePlan.Any(d => d.WorkOrderDetailId == dbObjDetail.Id))
                {
                    var machinePlans = repoMachinePlan.Filter(d => d.WorkOrderDetailId == dbObjDetail.Id)
                        .ToArray();

                    foreach (var item in machinePlans)
                    {
                        repoMachinePlan.Delete(item);
                    }
                }

                repoDetails.Delete(dbObjDetail);

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

        public WorkOrderDetailModel[] GetActiveWorkOrderDetailList()
        {
            List<WorkOrderDetailModel> data = new List<WorkOrderDetailModel>();

            var repo = _unitOfWork.GetRepository<WorkOrderDetail>();

            repo.Filter(d => d.WorkOrderStatus <= (int)WorkOrderStatusType.Completed).ToList().ForEach(d =>
            {
                WorkOrderDetailModel containerObj = new WorkOrderDetailModel();
                d.MapTo(containerObj);

                containerObj.ProductCode = d.Item != null ? d.Item.ItemNo : "";
                containerObj.ProductName = d.Item != null ? d.Item.ItemName : "";
                containerObj.WorkOrderDateStr = d.WorkOrder.WorkOrderDate != null ?
                    string.Format("{0:dd.MM.yyyy}", d.WorkOrder.WorkOrderDate) : "";
                containerObj.WorkOrderNo = d.WorkOrder.WorkOrderNo;
                containerObj.FirmCode = d.WorkOrder.Firm != null ? d.WorkOrder.Firm.FirmCode : "";
                containerObj.FirmName = d.WorkOrder.Firm != null ? d.WorkOrder.Firm.FirmName : "";
                containerObj.DyeCode = d.Dye != null ? d.Dye.DyeCode : "";
                containerObj.RalCode = d.Dye != null ? d.Dye.RalCode : "";
                containerObj.DyeName = d.Dye != null ? d.Dye.DyeName : "";
                containerObj.MachineCode = d.Machine != null ? d.Machine.MachineCode : "";
                containerObj.MachineName = d.Machine != null ? d.Machine.MachineName : "";
                containerObj.SaleOrderDocumentNo = d.ItemOrderDetail != null ? d.ItemOrderDetail.ItemOrder.DocumentNo : "";
                containerObj.SaleOrderReceiptNo = d.ItemOrderDetail != null ? d.ItemOrderDetail.ItemOrder.OrderNo : "";
                containerObj.SaleOrderDate = d.ItemOrderDetail != null ?
                    string.Format("{0:dd.MM.yyyy}", d.ItemOrderDetail.ItemOrder.OrderDate) : "";
                containerObj.SaleOrderDeadline = d.ItemOrderDetail != null ?
                    string.Format("", d.ItemOrderDetail.ItemOrder.DateOfNeed) : "";

                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public WorkOrderModel GetWorkOrder(int workOrderId)
        {
            WorkOrderModel model = new WorkOrderModel();

            try
            {
                var repo = _unitOfWork.GetRepository<WorkOrder>();
                var dbObj = repo.Get(d => d.Id == workOrderId);
                if (dbObj != null)
                {
                    dbObj.MapTo(model);

                    List<WorkOrderDetailModel> details = new List<WorkOrderDetailModel>();
                    dbObj.WorkOrderDetail.ToList()
                        .ForEach(d =>
                        {
                            var containerDetail = new WorkOrderDetailModel();
                            d.MapTo(containerDetail);
                            containerDetail.ProductCode = d.Item != null ? d.Item.ItemNo : "";
                            containerDetail.ProductName = d.Item != null ? d.Item.ItemName : "";
                            containerDetail.MachineCode = d.Machine != null ? d.Machine.MachineCode : "";
                            containerDetail.MachineName = d.Machine != null ? d.Machine.MachineName : "";
                            containerDetail.MoldCode = d.Mold != null ? d.Mold.MoldCode : "";
                            containerDetail.MoldName = d.Mold != null ? d.Mold.MoldName : "";
                            containerDetail.ProductDescription = d.MoldTest != null ? d.MoldTest.ProductDescription : "";
                            details.Add(containerDetail);
                        });

                    model.WorkOrderDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.WorkOrderDate);
                    model.FirmCode = dbObj.Firm != null ? dbObj.Firm.FirmCode : "";
                    model.FirmName = dbObj.Firm != null ? dbObj.Firm.FirmName : "";
                    model.Details = details.ToArray();
                }
            }
            catch (Exception)
            {

            }

            return model;
        }

        public BusinessResult SaveOrUpdateWorkOrder(WorkOrderModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<WorkOrder>();
                var repoDetail = _unitOfWork.GetRepository<WorkOrderDetail>();
                var repoItemOrderDetail = _unitOfWork.GetRepository<ItemOrderDetail>();
                var repoMoldTest = _unitOfWork.GetRepository<MoldTest>();

                bool newRecord = false;
                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new WorkOrder();
                    dbObj.WorkOrderNo = GetNextWorkOrderNo();
                    dbObj.CreatedDate = DateTime.Now;
                    dbObj.CreatedUserId = model.CreatedUserId;
                    dbObj.WorkOrderStatus = (int)OrderStatusType.Created;
                    repo.Add(dbObj);
                    newRecord = true;
                }

                var crDate = dbObj.CreatedDate;
                var woStats = dbObj.WorkOrderStatus;
                var crUserId = dbObj.CreatedUserId;

                model.MapTo(dbObj);

                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = crDate;
                if (dbObj.WorkOrderStatus == null)
                    dbObj.WorkOrderStatus = woStats;
                if (dbObj.CreatedUserId == null)
                    dbObj.CreatedUserId = crUserId;

                dbObj.UpdatedDate = DateTime.Now;

                #region SAVE DETAILS
                if (model.Details == null)
                    throw new Exception("Detay bilgisi olmadan iş emri kaydedilemez.");

                foreach (var item in model.Details)
                {
                    if (item.NewDetail)
                        item.Id = 0;
                }

                if (dbObj.WorkOrderStatus != (int)OrderStatusType.Completed &&
                    dbObj.WorkOrderStatus != (int)OrderStatusType.Cancelled)
                {
                    var newDetailIdList = model.Details.Select(d => d.Id).ToArray();
                    var deletedDetails = dbObj.WorkOrderDetail.Where(d => !newDetailIdList.Contains(d.Id)).ToArray();
                    foreach (var item in deletedDetails)
                    {
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
                        var dbDetail = repoDetail.Get(d => d.Id == item.Id);
                        if (dbDetail == null)
                        {
                            dbDetail = new WorkOrderDetail
                            {
                                WorkOrder = dbObj,
                                WorkOrderStatus = dbObj.WorkOrderStatus
                            };

                            repoDetail.Add(dbDetail);
                        }

                        item.MapTo(dbDetail);
                        dbDetail.WorkOrder = dbObj;
                        dbDetail.WorkOrderStatus = dbObj.WorkOrderStatus;
                        if (dbObj.Id > 0)
                            dbDetail.WorkOrderId = dbObj.Id;

                        #region MOVE MOLD TEST VALUES TO WORK ORDER DETAIL
                        if (item.MoldTestId > 0)
                        {
                            var dbMoldTest = repoMoldTest.Get(d => d.Id == item.MoldTestId);
                            if (dbMoldTest != null)
                            {
                                dbDetail.InflationTimeSeconds = dbMoldTest.InflationTimeSeconds;
                                dbDetail.RawGr = dbMoldTest.RawMaterialGr;
                                dbDetail.RawGrToleration = dbMoldTest.RawMaterialTolerationGr;
                            }
                        }
                        #endregion

                        #region SET ORDER & DETAIL STATUS TO PLANNED
                        if (dbDetail.SaleOrderDetailId > 0)
                        {
                            var dbItemOrderDetail = repoItemOrderDetail.Get(d => d.Id == dbDetail.SaleOrderDetailId);
                            if (dbItemOrderDetail != null)
                            {
                                dbItemOrderDetail.OrderStatus = (int)OrderStatusType.Planned;

                                if (!dbItemOrderDetail.ItemOrder
                                    .ItemOrderDetail.Any(d => d.OrderStatus != (int)OrderStatusType.Planned))
                                {
                                    dbItemOrderDetail.ItemOrder.OrderStatus = (int)OrderStatusType.Planned;
                                }
                            }
                        }
                        #endregion

                        lineNo++;
                    }
                }
                #endregion

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

        public BusinessResult DeleteWorkOrder(int workOrderId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<MachinePlan>();
                var repoWorkOrderDetail = _unitOfWork.GetRepository<WorkOrderDetail>();
                var repoWorkOrder = _unitOfWork.GetRepository<WorkOrder>();
                var repoItemOrderDetail = _unitOfWork.GetRepository<ItemOrderDetail>();

                var dbWorkOrder = repoWorkOrder.Get(d => d.Id == workOrderId);
                if (dbWorkOrder == null)
                    throw new Exception("Silmek istediğiniz iş emri kaydına bulunamadı.");

                if (dbWorkOrder.WorkOrderStatus <= (int)WorkOrderStatusType.Planned)
                {
                    var workOrderDetailList = dbWorkOrder.WorkOrderDetail.ToArray();
                    foreach (var dbWorkOrderDetail in workOrderDetailList)
                    {
                        var dbMachinePlan = repo.Get(d => d.WorkOrderDetailId == dbWorkOrderDetail.Id);
                        if (dbMachinePlan != null)
                        {
                            // RE-ORDER MACHINE PLANS AFTER DELETION
                            var plansOfMachine = repo.Filter(d => d.MachineId == dbMachinePlan.MachineId && d.Id != dbMachinePlan.Id)
                                    .OrderBy(d => d.OrderNo).ToArray();

                            int newOrderNo = 1;
                            foreach (var item in plansOfMachine)
                            {
                                item.OrderNo = newOrderNo;
                                newOrderNo++;
                            }

                            repo.Delete(dbMachinePlan);
                        }

                        int saleOrderDetailId = dbWorkOrderDetail.SaleOrderDetailId ?? 0;

                        repoWorkOrderDetail.Delete(dbWorkOrderDetail);

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
                    }

                    repoWorkOrder.Delete(dbWorkOrder);

                    _unitOfWork.SaveChanges();
                }
                else
                    throw new Exception("İşleme başlanmış veya tamamlanmış olan bir plan kaydını silemezsiniz.");

                repoWorkOrder.Delete(dbWorkOrder);

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

        #region MACHINE BUSINESS
        public MachineModel[] GetMachineList()
        {
            var repo = _unitOfWork.GetRepository<Machine>();

            List<MachineModel> containerList = new List<MachineModel>();
            repo.GetAll().ToList().ForEach(d =>
            {
                MachineModel containerObj = new MachineModel();
                d.MapTo(containerObj);
                containerList.Add(containerObj);
            });

            return containerList.ToArray();
        }
        #endregion
    }
}
