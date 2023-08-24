using Heka.DataAccess.Context;
using HekaMOLD.Business.Base;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.DataTransfer.Receipt;
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

        #region PRODUCT RECIPE CONSUMPTIONS
        public BusinessResult CreateRecipeConsumption(int workOrderDetailId, int? warehouseId = null)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repoWorkDetail = _unitOfWork.GetRepository<WorkOrderDetail>();
                var repoRecipeDetail = _unitOfWork.GetRepository<ProductRecipeDetail>();
                var repoWr = _unitOfWork.GetRepository<Warehouse>();

                var dbWorkDetail = repoWorkDetail.Get(d => d.Id == workOrderDetailId);
                if (dbWorkDetail == null)
                    throw new Exception("İş emri bilgisine ulaşılamadı.");

                var recipeDetails = repoRecipeDetail.Filter(d => d.ProductRecipe.ProductId == dbWorkDetail.ItemId);
                List<ItemReceiptDetailModel> consDetails = new List<ItemReceiptDetailModel>();

                int lineNumber = 1;
                foreach (var item in recipeDetails)
                {
                    consDetails.Add(new ItemReceiptDetailModel
                    {
                        ItemId = item.ItemId,
                        Quantity = item.Quantity * dbWorkDetail.Quantity,
                        UnitId = item.UnitId,
                        LineNumber = lineNumber,
                        CreatedDate = DateTime.Now,
                        NewDetail = true,
                        ReceiptStatus = (int)ReceiptStatusType.Created,
                        SyncStatus = 1,
                        TaxIncluded = false,
                        TaxRate = 0,
                        TaxAmount = 0,
                    });

                    lineNumber++;
                }

                BusinessResult recipeCreationResult = new BusinessResult { Result = false };

                var wrRecord = repoWr.Get(d => (warehouseId == null || d.Id == warehouseId) && d.WarehouseType ==
                    (int)WarehouseType.ItemWarehouse);
                if (wrRecord == null)
                    throw new Exception("Malzeme sarfiyatları için uygun depo tanımı bulunamadı.");

                using (ReceiptBO bObj = new ReceiptBO())
                {
                    var currentConsReceipt = bObj.GetConsumptionReceipt(workOrderDetailId);
                    if (currentConsReceipt == null)
                    {
                        recipeCreationResult = bObj.SaveOrUpdateItemReceipt(new ItemReceiptModel
                        {
                            CreatedDate = DateTime.Now,
                            ReceiptDate = DateTime.Now,
                            ReceiptNo = bObj.GetNextReceiptNo(dbWorkDetail.WorkOrder.PlantId.Value, ItemReceiptType.Consumption),
                            ReceiptType = (int)ItemReceiptType.Consumption,
                            InWarehouseId = wrRecord.Id,
                            WorkOrderDetailId = workOrderDetailId,
                            PlantId = dbWorkDetail.WorkOrder.PlantId,
                            SyncStatus = 1,
                            SyncDate = null,
                            Details = consDetails.ToArray(),
                            ReceiptStatus = (int)ReceiptStatusType.Created,
                        });
                    }
                    else
                    {
                        currentConsReceipt.Details = consDetails.ToArray();
                        recipeCreationResult = bObj.SaveOrUpdateItemReceipt(currentConsReceipt);
                    }

                    if (!recipeCreationResult.Result)
                        throw new Exception("Tüketim oluşturulurken bir hata meydana geldi.");
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

        public BusinessResult CreateRecipeConsumption(int productionReceiptId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repoReceipt = _unitOfWork.GetRepository<ItemReceipt>();
                var repoRecipeDetail = _unitOfWork.GetRepository<ProductRecipeDetail>();
                var repoWr = _unitOfWork.GetRepository<Warehouse>();
                var repoMoldTest = _unitOfWork.GetRepository<MoldTest>();
                var repoItem = _unitOfWork.GetRepository<Item>();

                var dbProdReceipt = repoReceipt.Get(d => d.Id == productionReceiptId);
                if (dbProdReceipt == null)
                    throw new Exception("Üretim fişi bilgisine ulaşılamadı.");

                List<ItemReceiptDetailModel> consDetails = new List<ItemReceiptDetailModel>();
                int lineNumber = 1;

                var prodReceiptDetails = dbProdReceipt.ItemReceiptDetail.ToArray();

                foreach (var prodItem in prodReceiptDetails)
                {
                    var productionForm = repoMoldTest.Get(d => d.ProductCode == prodItem.Item.ItemNo);
                    //var productionForm = repoRecipeDetail.Get(d => d.ProductCode == prodItem.Item.ItemNo);
                    if (productionForm != null)
                    {
                        // raw material consumption
                        var dbRawItem = repoItem.Get(d => d.ItemNo == productionForm.RawMaterialName || d.ItemName == productionForm.RawMaterialName);
                        if (dbRawItem != null)
                        {
                            consDetails.Add(new ItemReceiptDetailModel
                            {
                                ItemId = dbRawItem.Id,
                                Quantity = productionForm.RawMaterialGr / 1000.0m * prodItem.Quantity,
                                UnitId = prodItem.UnitId,
                                LineNumber = lineNumber,
                                CreatedDate = DateTime.Now,
                                NewDetail = true,
                                ReceiptStatus = (int)ReceiptStatusType.Created,
                                SyncStatus = 1,
                                TaxIncluded = false,
                                TaxRate = 0,
                                TaxAmount = 0,
                            });

                            lineNumber++;
                        }

                        // dye consumption
                        var dbDye = repoItem.Get(d => d.ItemNo == productionForm.DyeCode || d.ItemName == productionForm.DyeCode
                                || d.ItemNo == productionForm.RalCode || d.ItemName == productionForm.RalCode);
                        if (dbDye != null)
                        {
                            consDetails.Add(new ItemReceiptDetailModel
                            {
                                ItemId = dbDye.Id,
                                Quantity = 0.02m * prodItem.Quantity,
                                UnitId = prodItem.UnitId,
                                LineNumber = lineNumber,
                                CreatedDate = DateTime.Now,
                                NewDetail = true,
                                ReceiptStatus = (int)ReceiptStatusType.Created,
                                SyncStatus = 1,
                                TaxIncluded = false,
                                TaxRate = 0,
                                TaxAmount = 0,
                            });

                            lineNumber++;
                        }

                        // package consumption
                        var dbPackage = repoItem.Get(d => d.ItemNo == productionForm.PackageDimension || d.ItemName == productionForm.PackageDimension);
                        if (dbPackage != null)
                        {
                            consDetails.Add(new ItemReceiptDetailModel
                            {
                                ItemId = dbPackage.Id,
                                Quantity = prodItem.Quantity / (productionForm.InPackageQuantity ?? 1),
                                UnitId = prodItem.UnitId,
                                LineNumber = lineNumber,
                                CreatedDate = DateTime.Now,
                                NewDetail = true,
                                ReceiptStatus = (int)ReceiptStatusType.Created,
                                SyncStatus = 1,
                                TaxIncluded = false,
                                TaxRate = 0,
                                TaxAmount = 0,
                            });

                            lineNumber++;
                        }

                        // nut consumption
                        var dbNut = repoItem.Get(d => d.ItemNo == productionForm.NutCaliber || d.ItemName == productionForm.NutCaliber);
                        if (dbNut != null)
                        {
                            consDetails.Add(new ItemReceiptDetailModel
                            {
                                ItemId = dbNut.Id,
                                Quantity = prodItem.Quantity * (productionForm.NutQuantity ?? 0),
                                UnitId = prodItem.UnitId,
                                LineNumber = lineNumber,
                                CreatedDate = DateTime.Now,
                                NewDetail = true,
                                ReceiptStatus = (int)ReceiptStatusType.Created,
                                SyncStatus = 1,
                                TaxIncluded = false,
                                TaxRate = 0,
                                TaxAmount = 0,
                            });

                            lineNumber++;
                        }
                    }
                }

                BusinessResult recipeCreationResult = new BusinessResult { Result = false };

                var wrRecord = repoWr.Get(d => d.WarehouseType ==
                    (int)WarehouseType.ItemWarehouse);
                if (wrRecord == null)
                    throw new Exception("Malzeme sarfiyatları için uygun depo tanımı bulunamadı.");

                using (ReceiptBO bObj = new ReceiptBO())
                {
                    var currentConsReceipt = bObj.GetItemReceipt(dbProdReceipt.ConsumptionReceiptId ?? 0); //bObj.GetConsumptionReceipt(workOrderDetailId);
                    if (currentConsReceipt == null || currentConsReceipt.Id <= 0)
                    {
                        recipeCreationResult = bObj.SaveOrUpdateItemReceipt(new ItemReceiptModel
                        {
                            CreatedDate = DateTime.Now,
                            ReceiptDate = DateTime.Now,
                            ReceiptNo = bObj.GetNextReceiptNo(dbProdReceipt.PlantId.Value, ItemReceiptType.Consumption),
                            ReceiptType = (int)ItemReceiptType.Consumption,
                            InWarehouseId = wrRecord.Id,
                            PlantId = dbProdReceipt.PlantId,
                            SyncStatus = 1,
                            SyncDate = null,
                            Details = consDetails.ToArray(),
                            ReceiptStatus = (int)ReceiptStatusType.Created,
                        });
                    }
                    else
                    {
                        currentConsReceipt.Details = consDetails.ToArray();
                        recipeCreationResult = bObj.SaveOrUpdateItemReceipt(currentConsReceipt);
                    }

                    if (!recipeCreationResult.Result)
                        throw new Exception("Tüketim oluşturulurken bir hata meydana geldi.");
                    else
                    {
                        if (dbProdReceipt.ConsumptionReceiptId == null)
                        {
                            dbProdReceipt.ConsumptionReceiptId = recipeCreationResult.RecordId;
                            _unitOfWork.SaveChanges();
                        }
                    }
                }

                result.RecordId = dbProdReceipt.Id;
                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public ItemReceiptModel GetConsumptionReceipt(int productionReceiptId)
        {
            ItemReceiptModel model = null;

            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceipt>();
                var dbProdReceipt = repo.Get(d => d.Id == productionReceiptId);
                if (dbProdReceipt != null)
                {
                    using (ReceiptBO bObj = new ReceiptBO())
                    {
                        model = bObj.GetItemReceipt(dbProdReceipt.ConsumptionReceiptId ?? 0);
                    }
                }
            }
            catch (Exception)
            {

            }

            return model;
        }
        #endregion
    }
}
