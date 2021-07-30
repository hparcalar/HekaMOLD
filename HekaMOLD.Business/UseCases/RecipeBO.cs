using Heka.DataAccess.Context;
using HekaMOLD.Business.Base;
using HekaMOLD.Business.Helpers;
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
    public class RecipeBO : CoreProductionBO
    {
        #region PRODUCT RECIPE DEFINITON
        public ProductRecipeModel[] GetProductRecipeList()
        {
            var repo = _unitOfWork.GetRepository<ProductRecipe>();

            return repo.GetAll().Select(d => new ProductRecipeModel
            {
                Id = d.Id,
                ProductRecipeCode = d.ProductRecipeCode,
                Description = d.Description,
                CreatedDate = d.CreatedDate,
                CreatedDateStr = string.Format("{0:dd.MM.yyyy}", d.CreatedDate),
                ProductCode = d.Item != null ? d.Item.ItemNo : "",
                ProductName = d.Item != null ? d.Item.ItemName : "",

            }).ToArray();
        }

        public ProductRecipeModel[] GetActiveProductRecipeList()
        {
            var repo = _unitOfWork.GetRepository<ProductRecipe>();

            return repo.Filter(d => d.IsActive == true).ToList().Select(d => new ProductRecipeModel
            {
                Id = d.Id,
                ProductRecipeCode = d.ProductRecipeCode,
                Description = d.Description,
                CreatedDate = d.CreatedDate,
                CreatedDateStr = string.Format("{0:dd.MM.yyyy}", d.CreatedDate),
                ProductCode = d.Item != null ? d.Item.ItemNo : "",
                ProductName = d.Item != null ? d.Item.ItemName : "",

            }).ToArray();
        }

        public BusinessResult SaveOrUpdateProductRecipe(ProductRecipeModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.ProductRecipeCode))
                    throw new Exception("Reçete no girilmelidir.");
                //if (string.IsNullOrEmpty(model.Description))
                //    throw new Exception("Açıklama girilmelidir.");

                var repo = _unitOfWork.GetRepository<ProductRecipe>();
                var repoDetails = _unitOfWork.GetRepository<ProductRecipeDetail>();
                var repoItem = _unitOfWork.GetRepository<Item>();
                var repoWr = _unitOfWork.GetRepository<Warehouse>();
                var repoItemWr = _unitOfWork.GetRepository<ItemWarehouse>();

                if (repo.Any(d => (d.ProductRecipeCode == model.ProductRecipeCode) && d.Id != model.Id))
                    throw new Exception("Aynı koda sahip başka bir reçete mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new ProductRecipe();
                    dbObj.ProductRecipeCode = GetNextProductRecipeNo();
                    dbObj.CreatedDate = DateTime.Now;
                    dbObj.IsActive = true;
                    dbObj.CreatedUserId = model.CreatedUserId;
                    repo.Add(dbObj);
                }

                var crDate = dbObj.CreatedDate;

                model.MapTo(dbObj);

                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = crDate;

                dbObj.UpdatedDate = DateTime.Now;

                #region SAVE DETAILS
                if (model.Details == null)
                    model.Details = new ProductRecipeDetailModel[0];

                var toBeRemovedDetails = dbObj.ProductRecipeDetail
                    .Where(d => !model.Details.Where(m => m.NewDetail == false)
                        .Select(m => m.Id).ToArray().Contains(d.Id)
                    ).ToArray();
                foreach (var item in toBeRemovedDetails)
                {
                    repoDetails.Delete(item);
                }

                foreach (var item in model.Details)
                {
                    // CHECK WAREHOUSE MOVEMENT IS ALLOWED FOR THE ITEM
                    if (item.WarehouseId != null && !repoItemWr.Any(d => d.ItemId == item.ItemId &&
                        d.WarehouseId == item.WarehouseId && d.IsAllowed == true
                    ))
                    {
                        var dbItem = repoItem.Get(d => d.Id == item.ItemId);
                        var dbWr = repoWr.Get(d => d.Id == item.WarehouseId);
                        throw new Exception(dbItem.ItemNo + " nolu stok, " +
                            dbWr.WarehouseName + " deposunda hareket göremez.");
                    }

                    if (item.NewDetail == true)
                    {
                        var dbDetail = new ProductRecipeDetail();
                        item.MapTo(dbDetail);
                        dbDetail.ProductRecipe = dbObj;
                        repoDetails.Add(dbDetail);
                    }
                    else if (!toBeRemovedDetails.Any(d => d.Id == item.Id))
                    {
                        var dbDetail = repoDetails.GetById(item.Id);
                        item.MapTo(dbDetail);
                        dbDetail.ProductRecipe = dbObj;
                    }
                }
                #endregion

                if (dbObj.IsActive == true)
                {
                    #region CHECK ANOTHER ACTIVE RECIPES EXISTS, THEN MARK THEM AS REVISIONS
                    var revisionRecipes = repo
                        .Filter(d => d.ProductId != null && d.ProductId == dbObj.ProductId && d.IsActive == true && d.Id != dbObj.Id)
                        .ToArray();

                    foreach (var revRecipe in revisionRecipes)
                    {
                        revRecipe.IsActive = false;
                    }
                    #endregion
                }

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

        public BusinessResult AddRecipeDetail(int recipeId, ProductRecipeDetailModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repoRecipe = _unitOfWork.GetRepository<ProductRecipe>();
                var repoRecipeDetail = _unitOfWork.GetRepository<ProductRecipeDetail>();

                var dbRecipe = repoRecipe.Get(d => d.Id == recipeId);
                if (dbRecipe == null)
                    throw new Exception("Reçete bilgisi HEKA yazılımında bulunamadı.");

                var dbNewDetail = new ProductRecipeDetail();
                model.MapTo(dbNewDetail);
                dbNewDetail.ProductRecipe = dbRecipe;
                repoRecipeDetail.Add(dbNewDetail);

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
        public BusinessResult DeleteProductRecipe(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ProductRecipe>();
                var repoDetails = _unitOfWork.GetRepository<ProductRecipeDetail>();

                var dbObj = repo.Get(d => d.Id == id);

                var details = dbObj.ProductRecipeDetail.ToArray();
                foreach (var item in details)
                {
                    repoDetails.Delete(item);
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

        public ProductRecipeModel GetProductRecipe(int id)
        {
            ProductRecipeModel model = new ProductRecipeModel { };

            var repo = _unitOfWork.GetRepository<ProductRecipe>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.CreatedDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.CreatedDate);

                #region GET DETAILS
                List<ProductRecipeDetailModel> detailList = new List<ProductRecipeDetailModel>();
                dbObj.ProductRecipeDetail.ToList().ForEach(d =>
                {
                    ProductRecipeDetailModel containerObj = new ProductRecipeDetailModel();
                    d.MapTo(containerObj);
                    containerObj.NewDetail = false;
                    containerObj.ItemNo = d.Item != null ? d.Item.ItemNo : "";
                    containerObj.ItemName = d.Item != null ? d.Item.ItemName : "";
                    containerObj.UnitCode = d.UnitType != null ? d.UnitType.UnitCode : "";
                    containerObj.UnitName = d.UnitType != null ? d.UnitType.UnitName : "";
                    containerObj.WarehouseCode = d.Warehouse != null ? d.Warehouse.WarehouseCode : "";
                    containerObj.WarehouseName = d.Warehouse != null ? d.Warehouse.WarehouseName : "";
                    detailList.Add(containerObj);
                });
                model.Details = detailList.ToArray();
                #endregion
            }

            return model;
        }

        public ProductRecipeModel GetProductRecipe(string recipeCode)
        {
            ProductRecipeModel model = new ProductRecipeModel { };

            var repo = _unitOfWork.GetRepository<ProductRecipe>();
            var dbObj = repo.Get(d => d.ProductRecipeCode == recipeCode);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.CreatedDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.CreatedDate);

                #region GET DETAILS
                List<ProductRecipeDetailModel> detailList = new List<ProductRecipeDetailModel>();
                dbObj.ProductRecipeDetail.ToList().ForEach(d =>
                {
                    ProductRecipeDetailModel containerObj = new ProductRecipeDetailModel();
                    d.MapTo(containerObj);
                    containerObj.NewDetail = false;
                    containerObj.ItemNo = d.Item != null ? d.Item.ItemNo : "";
                    containerObj.ItemName = d.Item != null ? d.Item.ItemName : "";
                    containerObj.UnitCode = d.UnitType != null ? d.UnitType.UnitCode : "";
                    containerObj.UnitName = d.UnitType != null ? d.UnitType.UnitName : "";
                    containerObj.WarehouseCode = d.Warehouse != null ? d.Warehouse.WarehouseCode : "";
                    containerObj.WarehouseName = d.Warehouse != null ? d.Warehouse.WarehouseName : "";
                    detailList.Add(containerObj);
                });
                model.Details = detailList.ToArray();
                #endregion
            }

            return model;
        }

        public ProductRecipeModel FindActiveRecipeByProduct(int productId)
        {
            ProductRecipeModel model = new ProductRecipeModel();

            var repo = _unitOfWork.GetRepository<ProductRecipe>();
            var dbRecipe = repo.Get(d => d.ProductId == productId && d.IsActive == true);
            if (dbRecipe != null)
                model = GetProductRecipe(dbRecipe.Id);

            return model;
        }

        public ProductRecipeModel[] GetRevisionsOfProduct(int productId)
        {
            List<ProductRecipeModel> revisionsData = new List<ProductRecipeModel>();

            var repo = _unitOfWork.GetRepository<ProductRecipe>();
            var historyData = repo.Filter(d => d.ProductId == productId &&
                (d.IsActive ?? false) == false)
                .OrderByDescending(d => d.CreatedDate)
                .Select(d => d.Id)
                .ToArray();

            foreach (var item in historyData)
            {
                var revisionModel = GetProductRecipe(item);
                if (revisionModel != null)
                    revisionsData.Add(revisionModel);
            }

            return revisionsData.ToArray();
        }

        public bool HasAnyProductRecipe(string recipeCode)
        {
            var repo = _unitOfWork.GetRepository<ProductRecipe>();
            return repo.Any(d => d.ProductRecipeCode == recipeCode);
        }
        #endregion
    }
}
