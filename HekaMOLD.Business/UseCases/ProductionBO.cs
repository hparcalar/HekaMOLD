using Heka.DataAccess.Context;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.Filters;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases.Core;
using System;
using System.Collections;
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

        public MachinePlanModel GetActiveWorkOrderOnMachine(int machineId)
        {
            MachinePlanModel model = new MachinePlanModel();

            try
            {
                var repo = _unitOfWork.GetRepository<MachinePlan>();
                var dbObj = repo.Filter(d => d.WorkOrderDetail.WorkOrderStatus == (int)WorkOrderStatusType.InProgress)
                    .OrderBy(d => d.OrderNo).FirstOrDefault();
                if (dbObj != null)
                {
                    model = new MachinePlanModel
                    {
                        Id = dbObj.Id,
                        MachineId = dbObj.MachineId,
                        OrderNo = dbObj.OrderNo,
                        WorkOrderDetailId = dbObj.WorkOrderDetailId,
                        WorkOrder = new WorkOrderDetailModel
                        {
                            Id = dbObj.WorkOrderDetail.Id,
                            ProductCode = dbObj.WorkOrderDetail.Item.ItemNo,
                            ProductName = dbObj.WorkOrderDetail.Item.ItemName,
                            WorkOrderId = dbObj.WorkOrderDetail.WorkOrderId,
                            MoldId = dbObj.WorkOrderDetail.MoldId,
                            MoldCode = dbObj.WorkOrderDetail.Mold != null ? dbObj.WorkOrderDetail.Mold.MoldCode : "",
                            MoldName = dbObj.WorkOrderDetail.Mold != null ? dbObj.WorkOrderDetail.Mold.MoldName : "",
                            CreatedDate = dbObj.WorkOrderDetail.CreatedDate,
                            Quantity = dbObj.WorkOrderDetail.Quantity,
                            WorkOrderNo = dbObj.WorkOrderDetail.WorkOrder.WorkOrderNo,
                            WorkOrderDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.WorkOrderDetail.WorkOrder.WorkOrderDate),
                            FirmCode = dbObj.WorkOrderDetail.WorkOrder.Firm != null ?
                            dbObj.WorkOrderDetail.WorkOrder.Firm.FirmCode : "",
                            FirmName = dbObj.WorkOrderDetail.WorkOrder.Firm != null ?
                            dbObj.WorkOrderDetail.WorkOrder.Firm.FirmName : "",
                            WorkOrderStatus = dbObj.WorkOrderDetail.WorkOrderStatus,
                            WorkOrderStatusStr = ((WorkOrderStatusType)dbObj.WorkOrderDetail.WorkOrderStatus).ToCaption(),
                            CompleteQuantity = Convert.ToInt32(dbObj.WorkOrderDetail.WorkOrderSerial.Sum(m => m.FirstQuantity) ?? 0),
                            CompleteQuantitySingleProduct = dbObj.WorkOrderDetail.WorkOrderSerial.Count(),
                        }
                    };
                }
            }
            catch (Exception)
            {

            }

            return model;
        }

        public BusinessResult AddProductEntry(int workOrderDetailId, int userId, WorkOrderSerialType serialType)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<WorkOrderDetail>();
                var repoSerial = _unitOfWork.GetRepository<WorkOrderSerial>();

                var dbObj = repo.Get(d => d.Id == workOrderDetailId);
                if (dbObj == null)
                    throw new Exception("İş emri kaydına ulaşılamadı.");

                if ((dbObj.InPackageQuantity ?? 0) <= 0)
                    throw new Exception("Koli içi miktarı iş emrinde tanımlı değil!");

                if (serialType == WorkOrderSerialType.ProductPackage) // BATUSAN
                {
                    // CHECK TARGET QUANTITY FOR CHANGING STATUS TO COMPLETE
                    if (dbObj.Quantity - dbObj.InPackageQuantity 
                        <= (dbObj.WorkOrderSerial.Sum(d => d.FirstQuantity) ?? 0))
                    {
                        dbObj.WorkOrderStatus = (int)WorkOrderStatusType.Completed;

                        var dbWorkOrder = dbObj.WorkOrder;
                        if (!dbWorkOrder.WorkOrderDetail.Any(d => d.WorkOrderStatus != (int)WorkOrderStatusType.Completed &&
                            d.Id != dbObj.Id))
                            dbWorkOrder.WorkOrderStatus = (int)WorkOrderStatusType.Completed;
                    }

                    var product = new WorkOrderSerial
                    {
                        CreatedDate = DateTime.Now,
                        InPackageQuantity = dbObj.InPackageQuantity,
                        FirstQuantity = dbObj.InPackageQuantity,
                        IsGeneratedBySignal = false,
                        LiveQuantity = dbObj.InPackageQuantity,
                        SerialNo = GetNextSerialNo(),
                        SerialStatus = (int)SerialStatusType.Created,
                        SerialType = (int)serialType,
                        WorkOrderDetail = dbObj,
                        WorkOrder = dbObj.WorkOrder,
                        CreatedUserId = userId,
                    };

                    repoSerial.Add(product);
                }
                else if (serialType == WorkOrderSerialType.SingleProduct) // MICROMAX
                {

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

        #region POSTURE MANAGEMENT
        public BusinessResult SaveOrUpdatePosture(ProductionPostureModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ProductionPosture>();
                if (model.MachineId == null)
                    throw new Exception("Makine bilgisi duruş için girilmelidir.");

                if (model.StartDate == null)
                    throw new Exception("Duruş başlangıç tarihi belirtilmelidir.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    model.PostureStatus = 0;
                    model.CreatedDate = DateTime.Now;

                    dbObj = new ProductionPosture
                    {
                        CreatedDate = DateTime.Now,
                        PostureStatus = 0,
                    };
                    repo.Add(dbObj);
                }

                var crDate = dbObj.CreatedDate;
                var stDate = dbObj.StartDate;
                var edDate = dbObj.EndDate;

                model.MapTo(dbObj);

                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = crDate;
                if (dbObj.StartDate == null)
                    dbObj.StartDate = stDate;
                if (dbObj.EndDate == null)
                    dbObj.EndDate = edDate;

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

        public ProductionPostureModel[] GetPostureList(BasicRangeFilter filter)
        {
            ProductionPostureModel[] data = new ProductionPostureModel[0];

            try
            {
                DateTime dtStart, dtEnd;

                if (string.IsNullOrEmpty(filter.StartDate))
                    filter.StartDate = "01.01." + DateTime.Now.Year;
                if (string.IsNullOrEmpty(filter.EndDate))
                    filter.EndDate = "31.12." + DateTime.Now.Year;

                dtStart = DateTime.ParseExact(filter.StartDate + " 00:00:00", "dd.MM.yyyy HH:mm:ss",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                dtEnd = DateTime.ParseExact(filter.EndDate + " 23:59:59", "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));

                var repo = _unitOfWork.GetRepository<ProductionPostureModel>();
                data = repo.Filter(d => d.StartDate >= dtStart && d.StartDate <= dtEnd)
                    .ToList()
                    .Select(d => new ProductionPostureModel
                    {
                        Id = d.Id,
                        CreatedDate = d.CreatedDate,
                        CreatedUserId = d.CreatedUserId,
                        EndDate = d.EndDate,
                        Explanation = d.Explanation,
                        MachineId = d.MachineId,
                        PostureStatus = d.PostureStatus,
                        Reason = d.Reason,
                        StartDate = d.StartDate,
                        UpdatedDate = d.UpdatedDate,
                        UpdatedUserId = d.UpdatedUserId,
                        WorkOrderDetailId = d.WorkOrderDetailId,
                        PostureStatusStr = ((PostureStatusType)d.PostureStatus).ToCaption(),
                        StartDateStr = string.Format("{0:dd.MM.yyyy HH:mm}", d.StartDate),
                        EndDateStr = d.EndDate != null ?
                            string.Format("{0:dd.MM.yyyy HH:mm}", d.EndDate) : "",
                    }).ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }

        public ProductionPostureModel[] GetOngoingPostures()
        {
            ProductionPostureModel[] data = new ProductionPostureModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<ProductionPostureModel>();
                data = repo.Filter(d => d.PostureStatus == (int)PostureStatusType.Started
                    || d.PostureStatus == (int)PostureStatusType.WorkingOn)
                    .ToList()
                    .Select(d => new ProductionPostureModel
                    {
                        Id = d.Id,
                        CreatedDate = d.CreatedDate,
                        CreatedUserId = d.CreatedUserId,
                        EndDate = d.EndDate,
                        Explanation = d.Explanation,
                        MachineId = d.MachineId,
                        PostureStatus = d.PostureStatus,
                        Reason = d.Reason,
                        StartDate = d.StartDate,
                        UpdatedDate = d.UpdatedDate,
                        UpdatedUserId = d.UpdatedUserId,
                        WorkOrderDetailId = d.WorkOrderDetailId,
                        PostureStatusStr = ((PostureStatusType)d.PostureStatus).ToCaption(),
                        StartDateStr = string.Format("{0:dd.MM.yyyy HH:mm}", d.StartDate),
                        EndDateStr = d.EndDate != null ?
                            string.Format("{0:dd.MM.yyyy HH:mm}", d.EndDate) : "",
                    }).ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }
        #endregion

        #region PRODUCTION ITEM NEEDS MANAGEMENET
        public WorkOrderItemNeedsModel[] GetWorkOrderItemNeeds(BasicRangeFilter filter)
        {
            WorkOrderItemNeedsModel[] data = new WorkOrderItemNeedsModel[0];

            try
            {
                //DateTime dtStart, dtEnd;

                //if (string.IsNullOrEmpty(filter.StartDate))
                //    filter.StartDate = "01.01." + DateTime.Now.Year;
                //if (string.IsNullOrEmpty(filter.EndDate))
                //    filter.EndDate = "31.12." + DateTime.Now.Year;

                //dtStart = DateTime.ParseExact(filter.StartDate + " 00:00:00", "dd.MM.yyyy HH:mm:ss",
                //        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                //dtEnd = DateTime.ParseExact(filter.EndDate + " 23:59:59", "dd.MM.yyyy",
                //        System.Globalization.CultureInfo.GetCultureInfo("tr"));

                var repo = _unitOfWork.GetRepository<WorkOrderItemNeeds>();
                data = repo.Filter(d => d.RemainingNeedsQuantity > 0)
                .ToList()    
                .Select(d => new WorkOrderItemNeedsModel
                {
                    CalculatedDate = d.CalculatedDate,
                    CalculatedDateStr = d.CalculatedDate != null ?
                        string.Format("{0:dd.MM.yyyy HH:mm}", d.CalculatedDate) : "",
                    Id = d.Id,
                    ItemId = d.ItemId,
                    ItemName = d.Item != null ? d.Item.ItemName : "",
                    ItemNo = d.Item != null ? d.Item.ItemNo : "",
                    ProductCode = d.WorkOrderDetail.Item != null ? d.WorkOrderDetail.Item.ItemNo : "",
                    ProductName = d.WorkOrderDetail.Item != null ? d.WorkOrderDetail.Item.ItemName : "",
                    NeedsDateStr = d.WorkOrder.WorkOrderDate != null ?
                        string.Format("{0:dd.MM.yyyy HH:mm}", d.WorkOrder.WorkOrderDate) : "",
                    Quantity = d.Quantity,
                    RemainingQuantity = d.RemainingNeedsQuantity,
                    WorkOrderDateStr = d.WorkOrder.WorkOrderDate != null ?
                        string.Format("{0:dd.MM.yyyy HH:mm}", d.WorkOrder.WorkOrderDate) : "",
                    WorkOrderNo = d.WorkOrder.WorkOrderNo,
                    WorkOrderDetailId = d.WorkOrderDetailId,
                    WorkOrderId = d.WorkOrderId,
                }).ToArray();
            }
            catch (Exception ex)
            {

            }

            return data;
        }
        public BusinessResult CalculateWorkOrderNeeds()
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repoWorkOrderDetail = _unitOfWork.GetRepository<WorkOrderDetail>();
                var repoRecipe = _unitOfWork.GetRepository<ProductRecipe>();
                var repoNeeds = _unitOfWork.GetRepository<WorkOrderItemNeeds>();

                var openWorkOrders = repoWorkOrderDetail
                    .Filter(d => d.WorkOrderStatus == (int)WorkOrderStatusType.Created
                        || d.WorkOrderStatus == (int)WorkOrderStatusType.Planned)
                    .OrderBy(d => new { d.WorkOrder.WorkOrderDate, d.Id })
                    .ToArray();

                foreach (var item in openWorkOrders)
                {
                    // CLEAR CURRENT NEEDS
                    var currentNeeds = repoNeeds.Filter(d => d.WorkOrderDetailId == item.Id).ToArray();
                    foreach (var needsItem in currentNeeds)
                        repoNeeds.Delete(needsItem);

                    // WRITE NEW NEEDS
                    Hashtable itemStatusList = new Hashtable();

                    var dbRecipe = repoRecipe.Get(d => d.IsActive == true && d.ProductId == item.ItemId);
                    if (dbRecipe != null)
                    {
                        foreach (var recipeItem in dbRecipe.ProductRecipeDetail)
                        {
                            // GET ITEM USABLE QUANTITY FROM WAREHOUSES
                            decimal? itemStatus = null;
                            if (itemStatusList.ContainsKey(recipeItem.ItemId.Value))
                                itemStatus = (decimal?)itemStatusList[recipeItem.ItemId.Value];
                            else
                            {
                                itemStatus = recipeItem.Item.ItemLiveStatus.Sum(d => d.LiveQuantity ?? 0);
                                itemStatusList[recipeItem.ItemId.Value] = itemStatus;
                            }

                            // CALCULATE NEEDS QTY
                            var pureNeedsQty = recipeItem.Quantity * item.Quantity;
                            var usableQty = itemStatus >= pureNeedsQty ? pureNeedsQty : itemStatus;
                            if (usableQty < 0)
                                usableQty = 0;
                            var finalNeedsQty = pureNeedsQty - usableQty;

                            // UPDATE TOTAL ITEM STATUS HASHTABLE
                            itemStatusList[recipeItem.ItemId.Value] = itemStatus - usableQty;

                            if (finalNeedsQty > 0)
                            {
                                var newItemNeeds = new WorkOrderItemNeeds
                                {
                                    CalculatedDate = DateTime.Now,
                                    ItemId = recipeItem.ItemId,
                                    Quantity = finalNeedsQty,
                                    RemainingNeedsQuantity = finalNeedsQty,
                                    WorkOrderDetail = item,
                                    WorkOrder = item.WorkOrder,
                                };
                                repoNeeds.Add(newItemNeeds);
                            }
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
        #endregion
    }
}
