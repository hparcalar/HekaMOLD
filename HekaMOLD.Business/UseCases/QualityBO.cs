﻿using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.Models.DataTransfer.Quality;
using Heka.DataAccess.Context;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.Filters;

namespace HekaMOLD.Business.UseCases
{
    public class QualityBO : IBusinessObject
    {
        #region ENTRY QUALITY BUSINESS
        // ENTRY QUALITY PLANS
        public EntryQualityPlanModel[] GetEntryPlanList()
        {
            List<EntryQualityPlanModel> data = new List<EntryQualityPlanModel>();

            var repo = _unitOfWork.GetRepository<EntryQualityPlan>();

            repo.GetAll().ToList().ForEach(d =>
            {
                EntryQualityPlanModel containerObj = new EntryQualityPlanModel();
                d.MapTo(containerObj);
                containerObj.ItemGroupCode = d.ItemGroup != null ? d.ItemGroup.ItemGroupCode : "";
                containerObj.ItemGroupName = d.ItemGroup != null ? d.ItemGroup.ItemGroupName : "";
                containerObj.ItemCategoryCode = d.ItemCategory != null ? d.ItemCategory.ItemCategoryCode : "";
                containerObj.ItemCategoryName = d.ItemCategory != null ? d.ItemCategory.ItemCategoryName : "";
                containerObj.Details = d.EntryQualityPlanDetail.Select(m => new EntryQualityPlanDetailModel
                {
                    Id = m.Id,
                    CheckProperty = m.CheckProperty
                }).ToArray();

                string checkList = "";
                foreach (var checkItem in d.EntryQualityPlanDetail)
                {
                    checkList += checkItem.CheckProperty + "/";
                }

                containerObj.CheckPropertyList = checkList;

                data.Add(containerObj);
            });

            return data.OrderBy(d => d.OrderNo).ToArray();
        }

        public EntryQualityPlanModel[] GetEntryPlanListByItemGroup(int itemGroupId)
        {
            List<EntryQualityPlanModel> data = new List<EntryQualityPlanModel>();

            var repo = _unitOfWork.GetRepository<EntryQualityPlan>();

            repo.Filter(d => d.ItemGroupId == itemGroupId).ToList().ForEach(d =>
            {
                EntryQualityPlanModel containerObj = new EntryQualityPlanModel();
                d.MapTo(containerObj);
                containerObj.ItemGroupCode = d.ItemGroup != null ? d.ItemGroup.ItemGroupCode : "";
                containerObj.ItemGroupName = d.ItemGroup != null ? d.ItemGroup.ItemGroupName : "";
                containerObj.ItemCategoryCode = d.ItemCategory != null ? d.ItemCategory.ItemCategoryCode : "";
                containerObj.ItemCategoryName = d.ItemCategory != null ? d.ItemCategory.ItemCategoryName : "";
                containerObj.Details = d.EntryQualityPlanDetail.Select(m => new EntryQualityPlanDetailModel
                {
                    Id = m.Id,
                    CheckProperty = m.CheckProperty
                }).ToArray();

                string checkList = "";
                foreach (var checkItem in d.EntryQualityPlanDetail)
                {
                    checkList += checkItem.CheckProperty + "/";
                }

                containerObj.CheckPropertyList = checkList;

                data.Add(containerObj);
            });

            return data.OrderBy(d => d.OrderNo).ToArray();
        }

        public EntryQualityPlanModel[] GetEntryPlanView()
        {
            EntryQualityPlanModel[] data = new EntryQualityPlanModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<EntryQualityPlan>();

                data = repo.Filter(d => d.Id > 0)
                    .ToList()
                    .Select(m => new EntryQualityPlanModel
                    {
                        Id = m.Id,
                        OrderNo = m.OrderNo,
                        ItemGroupText = m.ItemGroupText,
                        Details = m.EntryQualityPlanDetail
                            .Select(d => new EntryQualityPlanDetailModel
                            {
                                Id = d.Id,
                                AcceptanceCriteria = d.AcceptanceCriteria,
                                CheckProperty = d.CheckProperty,
                                ControlDevice = d.ControlDevice,
                                EntryQualityPlanId = d.EntryQualityPlanId,
                                IsRequired = d.IsRequired,
                                Method = d.Method,
                                NewDetail = false,
                                OrderNo = d.OrderNo,
                                PeriodType = d.PeriodType,
                                Responsible = d.Responsible,
                            }).ToArray()
                    }).ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }

        public BusinessResult SaveOrUpdateEntryPlan(EntryQualityPlanModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.ItemGroupText))
                    throw new Exception("Malzeme/Hammadde adı girilmelidir.");

                var repo = _unitOfWork.GetRepository<EntryQualityPlan>();
                var repoDetails = _unitOfWork.GetRepository<EntryQualityPlanDetail>();

                model.QualityControlCode = model.ItemGroupText;

                if (repo.Any(d => (d.QualityControlCode == model.QualityControlCode)
                    && d.Id != model.Id))
                    throw new Exception("Aynı koda sahip başka bir kalite planı mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new EntryQualityPlan();
                    repo.Add(dbObj);
                }

                model.MapTo(dbObj);

                #region SAVE DETAILS
                if (model.Details == null)
                    model.Details = new EntryQualityPlanDetailModel[0];

                var toBeRemovedDetails = dbObj.EntryQualityPlanDetail
                    .Where(d => !model.Details.Where(m => m.NewDetail == false)
                        .Select(m => m.Id).ToArray().Contains(d.Id)
                    ).ToArray();
                foreach (var item in toBeRemovedDetails)
                {
                    repoDetails.Delete(item);
                }

                foreach (var item in model.Details)
                {
                    if (item.NewDetail == true)
                    {
                        var dbItemAu = new EntryQualityPlanDetail();
                        item.MapTo(dbItemAu);
                        dbItemAu.EntryQualityPlan = dbObj;
                        repoDetails.Add(dbItemAu);
                    }
                    else if (!toBeRemovedDetails.Any(d => d.Id == item.Id))
                    {
                        var dbItemAu = repoDetails.GetById(item.Id);
                        item.MapTo(dbItemAu);
                        dbItemAu.EntryQualityPlan = dbObj;
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

        public BusinessResult DeleteEntryPlan(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<EntryQualityPlan>();

                var dbObj = repo.Get(d => d.Id == id);
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

        public EntryQualityPlanModel GetEntryPlan(int id)
        {
            EntryQualityPlanModel model = new EntryQualityPlanModel { };

            var repo = _unitOfWork.GetRepository<EntryQualityPlan>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);

                #region GET DETAILS
                List<EntryQualityPlanDetailModel> detailList = new List<EntryQualityPlanDetailModel>();
                dbObj.EntryQualityPlanDetail.OrderBy(m => m.OrderNo).ToList().ForEach(d =>
                {
                    EntryQualityPlanDetailModel detailModel = new EntryQualityPlanDetailModel();
                    d.MapTo(detailModel);
                    detailList.Add(detailModel);
                });
                model.Details = detailList.ToArray();
                #endregion
            }

            return model;
        }
        // END -- ENTRY QUALITY PLANS

        // ENTRY QUALITY DATA FORM WORKS
        public EntryQualityDataModel[] GetEntryFormList()
        {
            EntryQualityDataModel[] data = new EntryQualityDataModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<EntryQualityData>();
                data = repo.GetAll().ToList().Select(d => new EntryQualityDataModel
                {
                    Id = d.Id,
                    CreatedDateStr = string.Format("{0:dd.MM.yyyy}", d.CreatedDate),
                    ItemLot = d.ItemLot,
                    WaybillNo = d.WaybillNo,
                    EntryQuantity = d.EntryQuantity,
                    CheckedQuantity = d.CheckedQuantity,
                    LotNumbers = d.LotNumbers,
                    SampleQuantity = d.SampleQuantity,
                    FirmId = d.FirmId,
                    ItemNo = d.Item != null ? d.Item.ItemNo : "",
                    ItemName = d.Item != null ? d.Item.ItemName : "",
                    FirmName = d.Firm != null ? d.Firm.FirmName : "",
                    CreatedDate = d.CreatedDate,
                    CreatedUserId = d.CreatedUserId,
                    UpdatedDate = d.UpdatedDate,
                    UpdatedUserId = d.UpdatedUserId,
                })
                .OrderByDescending(m => m.CreatedDate)
                .ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }

        public EntryQualityDataModel GetEntryForm(int id)
        {
            EntryQualityDataModel model = new EntryQualityDataModel { };

            var repo = _unitOfWork.GetRepository<EntryQualityData>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);

                model.CreatedDateStr = string.Format("{0:dd.MM.yyyy}", model.CreatedDate);

                #region GET DETAILS
                List<EntryQualityDataDetailModel> detailList = new List<EntryQualityDataDetailModel>();
                dbObj.EntryQualityDataDetail.ToList().ForEach(d =>
                {
                    EntryQualityDataDetailModel detailModel = new EntryQualityDataDetailModel();
                    d.MapTo(detailModel);
                    detailList.Add(detailModel);
                });
                model.Details = detailList.ToArray();
                #endregion
            }

            return model;
        }

        public BusinessResult SaveOrUpdateEntryForm(EntryQualityDataModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (model.ItemId == null)
                    throw new Exception("Malzeme seçilmelidir.");

                if (model.FirmId == null)
                    throw new Exception("Firma seçilmelidir.");

                var repo = _unitOfWork.GetRepository<EntryQualityData>();
                var repoDetails = _unitOfWork.GetRepository<EntryQualityDataDetail>();

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new EntryQualityData();
                    repo.Add(dbObj);
                }

                if (!string.IsNullOrEmpty(model.CreatedDateStr))
                    model.CreatedDate = DateTime.ParseExact(model.CreatedDateStr, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));

                var cntDate = dbObj.CreatedDate;

                model.MapTo(dbObj);

                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = cntDate;

                #region SAVE DETAILS
                if (model.Details == null)
                    model.Details = new EntryQualityDataDetailModel[0];

                var toBeRemovedDetails = dbObj.EntryQualityDataDetail
                    .Where(d => !model.Details.Where(m => m.NewDetail == false)
                        .Select(m => m.Id).ToArray().Contains(d.Id)
                    ).ToArray();
                foreach (var item in toBeRemovedDetails)
                {
                    repoDetails.Delete(item);
                }

                foreach (var item in model.Details)
                {
                    if (item.NewDetail == true)
                    {
                        var dbItemAu = new EntryQualityDataDetail();
                        item.MapTo(dbItemAu);
                        dbItemAu.EntryQualityData = dbObj;
                        repoDetails.Add(dbItemAu);
                    }
                    else if (!toBeRemovedDetails.Any(d => d.Id == item.Id))
                    {
                        var dbItemAu = repoDetails.GetById(item.Id);
                        item.MapTo(dbItemAu);
                        dbItemAu.EntryQualityData = dbObj;
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

        public BusinessResult DeleteEntryForm(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<EntryQualityData>();
                var repoDetail = _unitOfWork.GetRepository<EntryQualityDataDetail>();

                var dbObj = repo.Get(d => d.Id == id);
                if (dbObj != null)
                {
                    var details = dbObj.EntryQualityDataDetail.ToArray();
                    foreach (var item in details)
                    {
                        repoDetail.Delete(item);
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
        // END -- ENTRY QUALITY DATA
        #endregion

        #region PRODUCT QUALITY BUSINESS
        public ProductQualityPlanModel[] GetProductPlanList()
        {
            List<ProductQualityPlanModel> data = new List<ProductQualityPlanModel>();

            var repo = _unitOfWork.GetRepository<ProductQualityPlan>();

            repo.GetAll().ToList().ForEach(d =>
            {
                ProductQualityPlanModel containerObj = new ProductQualityPlanModel();
                d.MapTo(containerObj);
                data.Add(containerObj);
            });

            return data.OrderBy(d => d.OrderNo).ToArray();
        }

        public ProductQualityPlanModel[] GetProductPlanView()
        {
            ProductQualityPlanModel[] data = new ProductQualityPlanModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<ProductQualityPlan>();

                data = repo.Filter(d => d.Id > 0)
                    .ToList()
                    .Select(m => new ProductQualityPlanModel
                    {
                        Id = m.Id,
                        OrderNo = m.OrderNo,
                        AcceptanceCriteria = m.AcceptanceCriteria,
                        CheckProperties = m.CheckProperties,
                        ControlDevice = m.ControlDevice,
                        Method = m.Method,
                        PeriodType = m.PeriodType,
                        ProductQualityCode = m.ProductQualityCode,
                        Responsible = m.Responsible,
                    }).ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }

        public BusinessResult SaveOrUpdateProductPlan(ProductQualityPlanModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.ProductQualityCode))
                    throw new Exception("Faaliyet adı girilmelidir.");

                var repo = _unitOfWork.GetRepository<ProductQualityPlan>();

                if (repo.Any(d => (d.ProductQualityCode == model.ProductQualityCode)
                    && d.Id != model.Id))
                    throw new Exception("Aynı faaliyet adına sahip başka bir kalite planı mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new ProductQualityPlan();
                    repo.Add(dbObj);
                }

                model.MapTo(dbObj);

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

        public BusinessResult DeleteProductPlan(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ProductQualityPlan>();

                var dbObj = repo.Get(d => d.Id == id);
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

        public ProductQualityPlanModel GetProductPlan(int id)
        {
            ProductQualityPlanModel model = new ProductQualityPlanModel { };

            var repo = _unitOfWork.GetRepository<ProductQualityPlan>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }

        // PRODUCT QUALITY FORM WORKS
        public ProductQualityDataModel[] GetProductFormList()
        {
            ProductQualityDataModel[] data = new ProductQualityDataModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<ProductQualityData>();
                data = repo.GetAll().ToList().Select(d => new ProductQualityDataModel
                {
                    Id = d.Id,
                    ControlDate = d.ControlDate,
                    ControlDateStr = string.Format("{0:dd.MM.yyyy}", d.ControlDate),
                    MachineId = d.MachineId,
                    ProductId = d.ProductId,
                    MachineCode = d.Machine != null ? d.Machine.MachineCode : "",
                    MachineName = d.Machine != null ? d.Machine.MachineName : "",
                    ProductCode = d.Item != null ? d.Item.ItemNo : "",
                    ProductName = d.Item != null ? d.Item.ItemName : "",
                    CreatedDate = d.CreatedDate,
                    CreatedUserId = d.CreatedUserId,
                    UpdatedDate = d.UpdatedDate,
                    UpdatedUserId = d.UpdatedUserId,
                })
                .OrderByDescending(m => m.ControlDate)    
                .ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }

        public ProductQualityDataModel GetProductForm(int id)
        {
            ProductQualityDataModel model = new ProductQualityDataModel { };

            var repo = _unitOfWork.GetRepository<ProductQualityData>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);

                model.ControlDateStr = string.Format("{0:dd.MM.yyyy}", model.ControlDate);

                #region GET DETAILS
                List<ProductQualityDataDetailModel> detailList = new List<ProductQualityDataDetailModel>();
                dbObj.ProductQualityDataDetail.ToList().ForEach(d =>
                {
                    ProductQualityDataDetailModel detailModel = new ProductQualityDataDetailModel();
                    d.MapTo(detailModel);
                    detailList.Add(detailModel);
                });
                model.Details = detailList.ToArray();
                #endregion
            }

            return model;
        }

        public BusinessResult SaveOrUpdateProductForm(ProductQualityDataModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (model.MachineId == null)
                    throw new Exception("Makine seçilmelidir.");

                var repo = _unitOfWork.GetRepository<ProductQualityData>();
                var repoDetails = _unitOfWork.GetRepository<ProductQualityDataDetail>();

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new ProductQualityData();
                    repo.Add(dbObj);
                }

                if (!string.IsNullOrEmpty(model.ControlDateStr))
                    model.ControlDate = DateTime.ParseExact(model.ControlDateStr, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));

                var cntDate = dbObj.ControlDate;

                model.MapTo(dbObj);

                if (dbObj.ControlDate == null)
                    dbObj.ControlDate = cntDate;

                #region SAVE DETAILS
                if (model.Details == null)
                    model.Details = new ProductQualityDataDetailModel[0];

                var toBeRemovedDetails = dbObj.ProductQualityDataDetail
                    .Where(d => !model.Details.Where(m => m.NewDetail == false)
                        .Select(m => m.Id).ToArray().Contains(d.Id)
                    ).ToArray();
                foreach (var item in toBeRemovedDetails)
                {
                    repoDetails.Delete(item);
                }

                foreach (var item in model.Details)
                {
                    if (item.NewDetail == true)
                    {
                        var dbItemAu = new ProductQualityDataDetail();
                        item.MapTo(dbItemAu);
                        dbItemAu.ProductQualityData = dbObj;
                        repoDetails.Add(dbItemAu);
                    }
                    else if (!toBeRemovedDetails.Any(d => d.Id == item.Id))
                    {
                        var dbItemAu = repoDetails.GetById(item.Id);
                        item.MapTo(dbItemAu);
                        dbItemAu.ProductQualityData = dbObj;
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

        public BusinessResult DeleteProductForm(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ProductQualityData>();
                var repoDetail = _unitOfWork.GetRepository<ProductQualityDataDetail>();

                var dbObj = repo.Get(d => d.Id == id);
                if (dbObj != null)
                {
                    var details = dbObj.ProductQualityDataDetail.ToArray();
                    foreach (var item in details)
                    {
                        repoDetail.Delete(item);
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

        public string GetMoldTestValueForProductPlan(int planId, int moldTestId)
        {
            string testValue = "";

            try
            {
                var repoPlan = _unitOfWork.GetRepository<ProductQualityPlan>();
                var repoTest = _unitOfWork.GetRepository<MoldTest>();

                var dbPlan = repoPlan.Get(d => d.Id == planId);
                if (dbPlan != null && !string.IsNullOrEmpty(dbPlan.MoldTestFieldName))
                {
                    var dbTest = repoTest.Get(d => d.Id == moldTestId);
                    if (dbTest != null)
                    {
                        PropertyInfo pInfo = dbTest.GetType().GetProperty(dbPlan.MoldTestFieldName);
                        if (pInfo != null)
                        {
                            var testValueObj = pInfo.GetValue(dbTest, new object[] { });
                            if (testValueObj != null)
                                testValue = testValueObj.ToString();
                        }
                    }
                }
            }
            catch (Exception)
            {

            }

            return testValue;
        }

        // SERIAL APPROVALS & DENIALS
        public BusinessResult ApproveSerials(WorkOrderSerialModel[] model, int plantId)
        {
            BusinessResult result = new BusinessResult();
            
            try
            {
                int warehouseId = 0;

                var repo = _unitOfWork.GetRepository<WorkOrderSerial>();
                foreach (var item in model)
                {
                    var dbSerial = repo.Get(d => d.Id == item.Id);
                    if (dbSerial != null)
                    {
                        dbSerial.QualityStatus = (int)QualityStatusType.Ok;
                        dbSerial.SerialStatus = (int)SerialStatusType.Approved;
                        item.QualityStatus = dbSerial.QualityStatus;

                        if (warehouseId == 0 && (dbSerial.TargetWarehouseId) > 0)
                            warehouseId = dbSerial.TargetWarehouseId.Value;
                    }
                }

                if (warehouseId == 0)
                    throw new Exception("Hedef depo seçilmeden çekilen ürünleri onaylayamazsınız.");

                _unitOfWork.SaveChanges();

                // CREATE WAREHOUSE ENTRY RECEIPT
                using (ProductionBO bObj = new ProductionBO())
                {
                    result = bObj.MakeSerialPickupForProductWarehouse(new Models.DataTransfer.Receipt.ItemReceiptModel
                    {
                        PlantId = plantId,
                        InWarehouseId = warehouseId,
                    }, model);
                }
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult DenySerials(WorkOrderSerialModel[] model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<WorkOrderSerial>();
                foreach (var item in model)
                {
                    var dbSerial = repo.Get(d => d.Id == item.Id);
                    if (dbSerial != null)
                    {
                        dbSerial.QualityStatus = (int)QualityStatusType.Nok;
                        dbSerial.QualityExplanation = item.QualityExplanation;
                        dbSerial.SerialStatus = (int)SerialStatusType.Created;
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

        public BusinessResult WaitSerials(WorkOrderSerialModel[] model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<WorkOrderSerial>();
                foreach (var item in model)
                {
                    var dbSerial = repo.Get(d => d.Id == item.Id);
                    if (dbSerial != null)
                    {
                        dbSerial.QualityStatus = (int)QualityStatusType.QualityWaiting;
                        dbSerial.SerialStatus = (int)SerialStatusType.Created;
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

        private ShiftModel GetCurrentShift()
        {
            ShiftModel data = new ShiftModel();

            try
            {
                var repoShift = _unitOfWork.GetRepository<Shift>();

                // RESOLVE CURRENT SHIFT
                DateTime entryTime = DateTime.Now;
                DateTime shiftBelongsTo = DateTime.Now.Date;

                Shift dbShift = null;
                var shiftList = repoShift.Filter(d => d.StartTime != null && d.EndTime != null).ToArray();
                foreach (var shift in shiftList)
                {
                    DateTime startTime = DateTime.Now.Date.Add(shift.StartTime.Value);
                    DateTime endTime = DateTime.Now.Date.Add(shift.EndTime.Value);

                    if (shift.StartTime > shift.EndTime)
                    {
                        if (DateTime.Now.Hour >= shift.StartTime.Value.Hours) // gece 12 den önce
                        {
                            endTime = DateTime.Now.Date.AddDays(1).Add(shift.EndTime.Value);
                            shiftBelongsTo = DateTime.Now.Date;
                        }
                        else // gece 12den sonra
                        {
                            startTime = DateTime.Now.Date.AddDays(-1).Add(shift.StartTime.Value);
                            shiftBelongsTo = DateTime.Now.Date.AddDays(-1);
                        }
                    }
                    else
                        shiftBelongsTo = DateTime.Now.Date;

                    if (entryTime >= startTime && entryTime <= endTime)
                    {
                        dbShift = shift;
                        break;
                    }
                }

                if (dbShift != null)
                {
                    dbShift.MapTo(data);
                    data.ShiftBelongsToDate = shiftBelongsTo;
                }
            }
            catch (Exception)
            {

            }

            return data;
        }

        public BusinessResult SendToWastage(WorkOrderSerialModel[] model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<WorkOrderSerial>();
                var repoWastage = _unitOfWork.GetRepository<ProductWastage>();

                foreach (var item in model)
                {
                    var dbSerial = repo.Get(d => d.Id == item.Id);
                    if (dbSerial != null)
                    {
                        dbSerial.QualityStatus = (int)QualityStatusType.Nok;
                        dbSerial.QualityExplanation = item.QualityExplanation;
                        dbSerial.SerialStatus = (int)SerialStatusType.Scrap;

                        if (dbSerial.WorkOrderDetail != null)
                        {
                            // RESOLVE CURRENT SHIFT
                            var currentShift = GetCurrentShift();

                            if (currentShift != null)
                            {
                                // CREATE WASTAGE RECORD
                                var wstg = new ProductWastage
                                {
                                    CreatedDate = DateTime.Now,
                                    EntryDate = DateTime.Now,
                                    IsAfterScrap = true,
                                    MachineId = dbSerial.WorkOrderDetail.MachineId,
                                    ProductId = dbSerial.WorkOrderDetail.ItemId,
                                    WorkOrderDetailId = dbSerial.WorkOrderDetailId,
                                    Quantity = dbSerial.FirstQuantity,
                                    WastageStatus = 0,
                                    ShiftId = currentShift.Id,
                                    ShiftBelongsToDate = currentShift.ShiftBelongsToDate,
                                };
                                repoWastage.Add(wstg);
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

        public BusinessResult ConditionalApproveSerials(WorkOrderSerialModel[] model, int plantId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                int warehouseId = 0;

                var repo = _unitOfWork.GetRepository<WorkOrderSerial>();
                foreach (var item in model)
                {
                    var dbSerial = repo.Get(d => d.Id == item.Id);
                    if (dbSerial != null)
                    {
                        dbSerial.QualityStatus = (int)QualityStatusType.ConditionalApproved;
                        dbSerial.QualityExplanation = item.QualityExplanation;
                        dbSerial.SerialStatus = (int)SerialStatusType.Approved;
                        item.QualityStatus = dbSerial.QualityStatus;

                        if (warehouseId == 0 && (dbSerial.TargetWarehouseId) > 0)
                            warehouseId = dbSerial.TargetWarehouseId.Value;
                    }
                }

                if (warehouseId == 0)
                    throw new Exception("Hedef depo seçilmeden çekilen ürünleri onaylayamazsınız.");

                _unitOfWork.SaveChanges();

                // CREATE WAREHOUSE ENTRY RECEIPT
                using (ProductionBO bObj = new ProductionBO())
                {
                    result = bObj.MakeSerialPickupForProductWarehouse(new Models.DataTransfer.Receipt.ItemReceiptModel
                    {
                        PlantId = plantId,
                        InWarehouseId = warehouseId,
                    }, model);
                }
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public ProductWastageModel[] GetScrapList(BasicRangeFilter filter)
        {
            ProductWastageModel[] data = new ProductWastageModel[0];

            try
            {
                DateTime dtStart, dtEnd;

                if (string.IsNullOrEmpty(filter.StartDate))
                    filter.StartDate = "01.01." + DateTime.Now.Year;
                if (string.IsNullOrEmpty(filter.EndDate))
                    filter.EndDate = "31.12." + DateTime.Now.Year;

                dtStart = DateTime.ParseExact(filter.StartDate + " 00:00:00", "dd.MM.yyyy HH:mm:ss",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                dtEnd = DateTime.ParseExact(filter.EndDate + " 23:59:59", "dd.MM.yyyy HH:mm:ss",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));

                var repo = _unitOfWork.GetRepository<ProductWastage>();

                data = repo.Filter(d => d.CreatedDate >= dtStart && d.CreatedDate <= dtEnd
                        && (filter.MachineId == 0 || d.MachineId == filter.MachineId)
                        && d.IsAfterScrap == true
                    )
                    .ToList()
                    .Select(d => new ProductWastageModel
                    {
                        Id = d.Id,
                        CreatedDate = d.CreatedDate,
                        EntryDate = d.EntryDate,
                        CreatedUserId = d.CreatedUserId,
                        MachineId = d.MachineId,
                        WastageStatus = d.WastageStatus,
                        MachineCode = d.Machine != null ? d.Machine.MachineCode : "",
                        MachineName = d.Machine != null ? d.Machine.MachineName : "",
                        Quantity = d.Quantity,
                        ProductId = d.ProductId,
                        ProductCode = d.Item != null ? d.Item.ItemNo : "",
                        ProductName = d.Item != null ? d.Item.ItemName : "",
                        WorkOrderNo = d.WorkOrderDetail != null ? d.WorkOrderDetail.WorkOrder.WorkOrderNo : "",
                        ItemOrderNo = d.WorkOrderDetail != null && d.WorkOrderDetail.ItemOrderDetail != null ?
                            d.WorkOrderDetail.ItemOrderDetail.ItemOrder.DocumentNo : "",
                        EntryDateStr = d.EntryDate != null ?
                            string.Format("{0:dd.MM.yyyy HH:mm}", d.EntryDate) : "",
                    })
                    .OrderByDescending(d => d.EntryDate)
                    .ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }

        public WorkOrderSerialModel[] GetConditionalApprovedSerials()
        {
            WorkOrderSerialModel[] data = new WorkOrderSerialModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<WorkOrderSerial>();
                data = repo.Filter(d =>
                    (
                        d.QualityStatus == (int)QualityStatusType.ConditionalApproved
                    )
                    && d.SerialNo != null && d.SerialNo.Length > 0)
                    .ToList()
                    .Select(d => new WorkOrderSerialModel
                    {
                        Id = d.Id,
                        WorkOrderDetailId = d.WorkOrderDetailId,
                        ItemNo = d.WorkOrderDetail != null ? d.WorkOrderDetail.Item != null ? d.WorkOrderDetail.Item.ItemNo : "" : "",
                        ItemName = d.WorkOrderDetail != null ? d.WorkOrderDetail.Item != null ? d.WorkOrderDetail.Item.ItemName : d.WorkOrderDetail.TrialProductName : "",
                        CreatedDate = d.CreatedDate,
                        CreatedDateStr = d.CreatedDate != null ?
                            string.Format("{0:dd.MM.yyyy}", d.CreatedDate) : "",
                        MachineCode = d.WorkOrderDetail != null && d.WorkOrderDetail.Machine != null ?
                            d.WorkOrderDetail.Machine.MachineCode : "",
                        MachineName = d.WorkOrderDetail != null && d.WorkOrderDetail.Machine != null ?
                            d.WorkOrderDetail.Machine.MachineName : "",
                        QualityStatus = d.QualityStatus ?? 0,
                        QualityStatusText = ((QualityStatusType)(d.QualityStatus ?? 0)).ToCaption(),
                        FirstQuantity = d.FirstQuantity,
                        QualityExplanation = d.QualityExplanation,
                        InPackageQuantity = d.InPackageQuantity,
                        ShiftId = d.ShiftId,
                        ShiftCode = d.Shift != null ? d.Shift.ShiftCode : "",
                        ShiftName = d.Shift != null ? d.Shift.ShiftName : "",
                        IsGeneratedBySignal = d.IsGeneratedBySignal,
                        LiveQuantity = d.LiveQuantity,
                        SerialNo = d.SerialNo,
                        FirmCode = d.WorkOrderDetail != null ? d.WorkOrderDetail.WorkOrder.Firm != null ? d.WorkOrderDetail.WorkOrder.Firm.FirmCode : "" : "",
                        FirmName = d.WorkOrderDetail != null ? d.WorkOrderDetail.WorkOrder.Firm != null ? d.WorkOrderDetail.WorkOrder.Firm.FirmName : d.WorkOrderDetail.WorkOrder.TrialFirmName : "",
                    })
                    .OrderByDescending(d => d.CreatedDate) 
                    .ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }
        #endregion

        #region SERIAL WINDING QUALITY BUSINESS
        public BusinessResult StartWinding(int workOrderDetailId, string serialNo, int userId, int? qualityMachineId = null)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<SerialQualityWinding>();
                var repoWorkDetail = _unitOfWork.GetRepository<WorkOrderDetail>();

                var dbWorkDetail = repoWorkDetail.Get(d => d.Id == workOrderDetailId);
                if (dbWorkDetail == null)
                    throw new Exception("Seçilen iş emri kaydı bulunamadı.");

                SerialQualityWinding dbModel = new SerialQualityWinding
                {
                    StartDate = DateTime.Now,
                    OperatorId = userId > 0 ? userId : (int?)null,
                    FaultCount = 0,
                    IsOk = null,
                    SerialNo = serialNo,
                    WorkOrderDetailId = workOrderDetailId,
                    EndDate = null,
                };
                repo.Add(dbModel);

                _unitOfWork.SaveChanges();

                result.RecordId = dbModel.Id;
                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult EndWinding(int windingId, decimal? meters, decimal? quantity)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<SerialQualityWinding>();

                var dbWinding = repo.Get(d => d.Id == windingId);
                if (dbWinding == null)
                    throw new Exception("Seçilen sarım kaydı bulunamadı.");

                dbWinding.EndDate = DateTime.Now;
                dbWinding.FaultCount = dbWinding.SerialQualityWindingFault.Count();
                dbWinding.IsOk = dbWinding.SerialQualityWindingFault.Where(d => d.FaultStatus != 1).Count() > 0 ? false : true;
                dbWinding.TotalMeters = meters;
                dbWinding.TotalQuantity = quantity;

                _unitOfWork.SaveChanges();

                result.RecordId = dbWinding.Id;
                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult AddFaultToWinding(int windingId, int faultTypeId, decimal? meter, decimal? quantity, bool isDotted=false, int? userId = null)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<SerialQualityWindingFault>();
                var repoWinding = _unitOfWork.GetRepository<SerialQualityWinding>();
                var repoFaultType = _unitOfWork.GetRepository<SerialFaultType>();

                var dbFaultType = repoFaultType.Get(d => d.Id == faultTypeId);
                if (dbFaultType == null)
                    throw new Exception("Bir hata tipi seçmelisiniz.");

                var dbWinding = repoWinding.Get(d => d.Id == windingId);
                if (dbWinding == null)
                    throw new Exception("Bir sarım bilgisi seçmelisiniz.");

                var dbModel = new SerialQualityWindingFault
                {
                    SerialQualityWindingId = windingId,
                    FaultId = faultTypeId,
                    CurrentMeter = meter,
                    EndMeter = isDotted ? meter : null,
                    CurrentQuantity = quantity,
                    EndQuantity = isDotted ? quantity : null,
                    OperatorId = userId,
                    FaultDate = DateTime.Now,
                    FaultStatus = 0, // unseen status
                };
                repo.Add(dbModel);

                _unitOfWork.SaveChanges();

                result.RecordId = dbModel.Id;
                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult EndFaultAtWinding(int windingId, int faultTypeId, decimal? meter, decimal? quantity)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repoWinding = _unitOfWork.GetRepository<SerialQualityWinding>();
                var repo = _unitOfWork.GetRepository<SerialQualityWindingFault>();

                var dbWinding = repoWinding.Get(d => d.Id == windingId);
                if (dbWinding == null)
                    throw new Exception("Seçilen sarım kaydı bulunamadı.");

                var ongoingFault = repo.Filter(d => d.SerialQualityWindingId == windingId
                    && d.FaultId == faultTypeId && d.EndMeter == null)
                    .OrderBy(d => d.Id)
                    .FirstOrDefault();

                if (ongoingFault == null)
                    throw new Exception("Seçtiğiniz hata türüne ait devam eden bir kayıt buluanamadı.");

                ongoingFault.EndMeter = meter;
                ongoingFault.EndQuantity = quantity;

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

        public BusinessResult SetFaultAsSeen(int windingFaultId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<SerialQualityWindingFault>();
                var dbFaultRecord = repo.Get(d => d.Id == windingFaultId);
                if (dbFaultRecord == null)
                    throw new Exception("Seçilen hata kaydı bulunamadı.");

                dbFaultRecord.FaultStatus = 1;

                _unitOfWork.SaveChanges();

                result.RecordId = dbFaultRecord.Id;
                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult SetFaultAsUnseen(int windingFaultId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<SerialQualityWindingFault>();
                var dbFaultRecord = repo.Get(d => d.Id == windingFaultId);
                if (dbFaultRecord == null)
                    throw new Exception("Seçilen hata kaydı bulunamadı.");

                dbFaultRecord.FaultStatus = 0;

                _unitOfWork.SaveChanges();

                result.RecordId = dbFaultRecord.Id;
                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult RemoveFault(int windingFaultId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<SerialQualityWindingFault>();
                var dbFaultRecord = repo.Get(d => d.Id == windingFaultId);
                if (dbFaultRecord == null)
                    throw new Exception("Seçilen hata kaydı bulunamadı.");

                repo.Delete(dbFaultRecord);

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

        public SerialQualityWindingFaultModel[] GetFaultsOfWinding(int windingId)
        {
            SerialQualityWindingFaultModel[] data = new SerialQualityWindingFaultModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<SerialQualityWindingFault>();
                data = repo.Filter(d => d.SerialQualityWindingId == windingId)
                    .ToList()
                    .Select(d => new SerialQualityWindingFaultModel
                    {
                        Id = d.Id,
                        CurrentMeter = d.CurrentMeter,
                        CurrentQuantity = d.CurrentQuantity,
                        EndMeter = d.EndMeter,
                        EndQuantity = d.EndQuantity,
                        Explanation = d.Explanation,
                        FaultDate = d.FaultDate,
                        FaultId = d.FaultId,
                        FaultStatus = d.FaultStatus,
                        OperatorId = d.OperatorId,
                        SerialQualityWindingId = d.SerialQualityWindingId,
                        FaultCode = d.SerialFaultType.FaultCode,
                        FaultName = d.SerialFaultType.FaultName,
                        FaultDateStr = string.Format("{0:dd.MM.yyyy HH:mm}", d.FaultDate),
                        OperatorName = d.User != null ? d.User.UserName : "",
                    }).ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }

        public SerialQualityWindingModel[] GetWindingList(int workOrderDetailId)
        {
            SerialQualityWindingModel[] data = new SerialQualityWindingModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<SerialQualityWinding>();
                data = repo.Filter(d => d.WorkOrderDetailId == workOrderDetailId)
                    .ToList()
                    .Select(d => new SerialQualityWindingModel
                    {
                        Id = d.Id,
                        EndDate = d.EndDate,
                        Explanation = d.Explanation,
                        FaultCount = d.FaultCount,
                        IsOk = d.IsOk,
                        OperatorId = d.OperatorId,
                        SerialNo = d.SerialNo,
                        StartDate = d.StartDate,
                        TotalMeters = d.TotalMeters,
                        TotalQuantity = d.TotalQuantity,
                        WorkOrderDetailId = d.WorkOrderDetailId,
                        WorkOrderSerialId = d.WorkOrderSerialId,
                        StartDateStr = string.Format("{0:dd.MM.yyyy HH:mm}", d.StartDate),
                        EndDateStr = d.EndDate != null ? string.Format("{0:dd.MM.yyyy HH:mm}", d.EndDate) : "",
                        OperatorName = d.User != null ? d.User.UserName : "",
                    }).ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }
        #endregion
    }
}
