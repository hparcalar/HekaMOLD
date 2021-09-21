using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.Models.DataTransfer.Quality;
using Heka.DataAccess.Context;
using HekaMOLD.Business.Helpers;

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

        // ENTRY QUALITY DATA

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
        #endregion
    }
}
