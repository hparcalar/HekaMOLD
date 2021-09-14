using Heka.DataAccess.Context;
using Heka.DataAccess.UnitOfWork;
using HekaMOLD.Business.Base;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HekaMOLD.Business.UseCases
{
    public class DefinitionsBO : IBusinessObject
    {
        #region FIRM BUSINESS
        public FirmModel[] GetFirmList()
        {
            var repo = _unitOfWork.GetRepository<Firm>();

            return repo.GetAll()
                .Select(d => new FirmModel
                {
                    Id = d.Id,
                    FirmCode = d.FirmCode,
                    FirmName = d.FirmName,
                    IsApproved = true,
                    FirmType = d.FirmType,
                    FirmTypeStr = d.FirmType == 1 ? "Tedarikçi" : d.FirmType == 2 ? "Müşteri" : "Tedarikçi + Müşteri",
                }).ToArray();
        }

        public FirmModel[] GetApprovedSuppliers()
        {
            var repo = _unitOfWork.GetRepository<Firm>();

            return repo.Filter(d => d.FirmType == (int)FirmType.Supplier && d.IsApproved == true).ToList().Select(d => new FirmModel
            {
                Id = d.Id,
                FirmCode = d.FirmCode,
                FirmName = d.FirmName,
                IsApproved = true,
                FirmType = d.FirmType,
                FirmTypeStr = d.FirmType == 1 ? "Tedarikçi" : d.FirmType == 2 ? "Müşteri" : "Tedarikçi + Müşteri",
            }).ToArray();
        }

        public BusinessResult SaveOrUpdateFirm(FirmModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.FirmCode))
                    throw new Exception("Firma kodu girilmelidir.");
                if (string.IsNullOrEmpty(model.FirmName))
                    throw new Exception("Firma adı girilmelidir.");

                var repo = _unitOfWork.GetRepository<Firm>();
                var repoAuthors = _unitOfWork.GetRepository<FirmAuthor>();

                if (repo.Any(d => (d.FirmCode == model.FirmCode || d.FirmName == model.FirmName) && d.Id != model.Id))
                    throw new Exception("Aynı koda sahip başka bir firma mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new Firm();
                    dbObj.CreatedDate = DateTime.Now;
                    dbObj.CreatedUserId = model.CreatedUserId;
                    repo.Add(dbObj);
                }

                var crDate = dbObj.CreatedDate;

                model.MapTo(dbObj);

                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = crDate;

                dbObj.UpdatedDate = DateTime.Now;

                #region SAVE AUTHOR LIST
                if (model.Authors == null)
                    model.Authors = new FirmAuthorModel[0];

                var toBeRemovedAuthors = dbObj.FirmAuthor
                    .Where(d => !model.Authors.Where(m => m.NewDetail == false)
                        .Select(m => m.Id).ToArray().Contains(d.Id)
                    ).ToArray();
                foreach (var item in toBeRemovedAuthors)
                {
                    repoAuthors.Delete(item);
                }

                foreach (var item in model.Authors)
                {
                    if (item.NewDetail == true)
                    {
                        var dbItemAu = new FirmAuthor();
                        item.MapTo(dbItemAu);
                        dbItemAu.Firm = dbObj;
                        repoAuthors.Add(dbItemAu);
                    }
                    else if (!toBeRemovedAuthors.Any(d => d.Id == item.Id))
                    {
                        var dbItemAu = repoAuthors.GetById(item.Id);
                        item.MapTo(dbItemAu);
                        dbItemAu.Firm = dbObj;
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

        public BusinessResult DeleteFirm(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<Firm>();

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

        public FirmModel GetFirm(int id)
        {
            FirmModel model = new FirmModel { };

            var repo = _unitOfWork.GetRepository<Firm>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);

                #region GET AUTHOR LIST
                List<FirmAuthorModel> authorList = new List<FirmAuthorModel>();
                dbObj.FirmAuthor.ToList().ForEach(d =>
                {
                    FirmAuthorModel authorModel = new FirmAuthorModel();
                    d.MapTo(authorModel);
                    authorList.Add(authorModel);
                });
                model.Authors = authorList.ToArray();
                #endregion
            }

            return model;
        }

        public FirmModel GetFirm(string firmCode)
        {
            FirmModel model = new FirmModel { };

            var repo = _unitOfWork.GetRepository<Firm>();
            var dbObj = repo.Get(d => d.FirmCode == firmCode);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);

                #region GET AUTHOR LIST
                List<FirmAuthorModel> authorList = new List<FirmAuthorModel>();
                dbObj.FirmAuthor.ToList().ForEach(d =>
                {
                    FirmAuthorModel authorModel = new FirmAuthorModel();
                    d.MapTo(authorModel);
                    authorList.Add(authorModel);
                });
                model.Authors = authorList.ToArray();
                #endregion
            }

            return model;
        }
        public bool HasAnyFirm(string firmCode)
        {
            var repo = _unitOfWork.GetRepository<Firm>();
            return repo.Any(d => d.FirmCode == firmCode);
        }
        #endregion

        #region ITEM BUSINESS
        public ItemModel[] GetItemList()
        {
            var repo = _unitOfWork.GetRepository<Item>();

            return repo.GetAll().Select(d => new ItemModel
            {
                Id=d.Id,
                ItemNo = d.ItemNo,
                ItemName = d.ItemName,
                ItemTypeStr = d.ItemType == 1 ? "Hammadde" : d.ItemType == 2 ? "Ticari Mal" :
                        d.ItemType == 3 ? "Yarı Mamul" : d.ItemType == 4 ? "Mamul" : "",
                ItemType = d.ItemType,
                CategoryName = d.ItemCategory != null ? d.ItemCategory.ItemCategoryName : "",
                GroupName = d.ItemGroup != null ? d.ItemGroup.ItemGroupName : "",
                TotalInQuantity = d.ItemLiveStatus.Sum(m => m.InQuantity) ?? 0,
                TotalOutQuantity = d.ItemLiveStatus.Sum(m => m.OutQuantity) ?? 0,
                TotalOverallQuantity = d.ItemLiveStatus.Sum(m => m.LiveQuantity) ?? 0,
            }).ToArray();
        }

        public ItemModel[] GetProductList()
        {
            var repo = _unitOfWork.GetRepository<Item>();

            return repo.Filter(d => d.ItemType == (int)ItemType.Product
                || d.ItemType == (int)ItemType.SemiProduct).ToList().Select(d => new ItemModel
            {
                Id = d.Id,
                ItemNo = d.ItemNo,
                ItemName = d.ItemName,
                ItemTypeStr = d.ItemType == 1 ? "Hammadde" : d.ItemType == 2 ? "Ticari Mal" :
                        d.ItemType == 3 ? "Yarı Mamul" : d.ItemType == 4 ? "Mamul" : "",
                ItemType = d.ItemType,
                CategoryName = d.ItemCategory != null ? d.ItemCategory.ItemCategoryName : "",
                GroupName = d.ItemGroup != null ? d.ItemGroup.ItemGroupName : "",
                TotalInQuantity = d.ItemLiveStatus.Sum(m => m.InQuantity) ?? 0,
                TotalOutQuantity = d.ItemLiveStatus.Sum(m => m.OutQuantity) ?? 0,
                TotalOverallQuantity = d.ItemLiveStatus.Sum(m => m.LiveQuantity) ?? 0,
            }).ToArray();
        }

        public BusinessResult SaveOrUpdateItem(ItemModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.ItemNo))
                    throw new Exception("Stok numarası girilmelidir.");
                if (string.IsNullOrEmpty(model.ItemName))
                    throw new Exception("Stok adı girilmelidir.");

                var repo = _unitOfWork.GetRepository<Item>();
                var repoWarehouses = _unitOfWork.GetRepository<ItemWarehouse>();
                var repoUnits = _unitOfWork.GetRepository<ItemUnit>();

                if (repo.Any(d => (d.ItemNo == model.ItemNo || d.ItemName == model.ItemName) 
                    && d.Id != model.Id))
                    throw new Exception("Aynı numaraya sahip başka bir stok mevcuttur. Lütfen farklı bir numara giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new Item();
                    dbObj.CreatedDate = DateTime.Now;
                    dbObj.CreatedUserId = model.CreatedUserId;
                    repo.Add(dbObj);
                }

                var crDate = dbObj.CreatedDate;

                model.MapTo(dbObj);

                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = crDate;

                dbObj.UpdatedDate = DateTime.Now;

                #region SAVE WAREHOUSE PRM
                if (model.Warehouses == null)
                    model.Warehouses = new ItemWarehouseModel[0];

                var toBeRemovedWarehouses = dbObj.ItemWarehouse
                    .Where(d => !model.Warehouses.Select(m => m.Id).ToArray().Contains(d.Id))
                    .ToArray();
                foreach (var item in toBeRemovedWarehouses)
                {
                    repoWarehouses.Delete(item);
                }

                foreach (var item in model.Warehouses
                    .Where(d => !toBeRemovedWarehouses.Any(m => m.WarehouseId == d.WarehouseId)))
                {
                    var dbItemWr = repoWarehouses.GetById(item.Id);
                    if (dbItemWr == null || item.Id == 0)
                    {
                        dbItemWr = new ItemWarehouse();
                        item.MapTo(dbItemWr);
                        dbItemWr.Item = dbObj;
                        repoWarehouses.Add(dbItemWr);
                    }
                    else
                    {
                        item.MapTo(dbItemWr);
                        dbItemWr.Item = dbObj;
                    }
                }
                #endregion

                #region SAVE ITEM UNITS PRM
                if (model.Units == null)
                    model.Units = new ItemUnitModel[0];

                var toBeRemovedUnits = dbObj.ItemUnit
                    .Where(d => !model.Units.Where(m => m.NewDetail == false)
                        .Select(m => m.Id).ToArray().Contains(d.Id)
                    ).ToArray();
                foreach (var item in toBeRemovedUnits)
                {
                    repoUnits.Delete(item);
                }

                foreach (var item in model.Units)
                {
                    if (item.NewDetail == true)
                    {
                        var dbItemUn = new ItemUnit();
                        item.MapTo(dbItemUn);
                        dbItemUn.Item = dbObj;
                        repoUnits.Add(dbItemUn);
                    }
                    else if (!toBeRemovedUnits.Any(d => d.Id == item.Id))
                    {
                        var dbItemUn = repoUnits.GetById(item.Id);
                        item.MapTo(dbItemUn);
                        dbItemUn.Item = dbObj;
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

                if (ex.InnerException != null)
                    result.ErrorMessage = ex.InnerException.Message;
                else
                    result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult DeleteItem(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<Item>();

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

        public ItemModel GetItem(int id)
        {
            ItemModel model = new ItemModel { };

            var repo = _unitOfWork.GetRepository<Item>();
            var repoWarehouse = _unitOfWork.GetRepository<Warehouse>();

            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);

                #region WAREHOUSE RESTRICTIONS BY ITEM TYPE
                var warehouseList = repoWarehouse.Filter(d =>
                    ((model.ItemType == (int)ItemType.RawMaterials || model.ItemType == (int)ItemType.Commercial) 
                        && d.WarehouseType == (int)WarehouseType.ItemWarehouse)
                    ||
                    ((model.ItemType == (int)ItemType.SemiProduct) && 
                        (d.WarehouseType == (int)WarehouseType.ItemWarehouse || d.WarehouseType == (int)WarehouseType.ProductWarehouse))
                    ||
                    (model.ItemType == (int)ItemType.Product && d.WarehouseType == (int)WarehouseType.ProductWarehouse)
                ).ToArray();
                #endregion

                #region GET WAREHOUSE PARAMETERS
                List<ItemWarehouseModel> warehousePrmList = new List<ItemWarehouseModel>();

                foreach (var item in warehouseList)
                {
                    ItemWarehouseModel itemWarehousePrm = new ItemWarehouseModel
                    {
                        ItemId = model.Id,
                        WarehouseId = item.Id,
                        IsAllowed=true,
                        MaximumQuantity = null,
                        WarehouseCode = item.WarehouseCode,
                        WarehouseName = item.WarehouseName,
                        MinimumQuantity = null,
                        MinimumBehaviour = (int)ItemCriticalBehaviourType.None,
                        MaximumBehaviour = (int)ItemCriticalBehaviourType.None
                    };

                    var dbItemWarehouse = dbObj.ItemWarehouse.FirstOrDefault(d => d.WarehouseId == item.Id);
                    if (dbItemWarehouse != null)
                    {
                        dbItemWarehouse.MapTo(itemWarehousePrm);
                    }

                    warehousePrmList.Add(itemWarehousePrm);
                }
                #endregion

                #region GET ITEM UNITS
                List<ItemUnitModel> unitModels = new List<ItemUnitModel>();
                dbObj.ItemUnit.ToList().ForEach(d =>
                {
                    ItemUnitModel unitData = new ItemUnitModel();
                    d.MapTo(unitData);
                    unitData.UnitCode = d.UnitType != null ? d.UnitType.UnitCode : "";
                    unitData.UnitName = d.UnitType != null ? d.UnitType.UnitName : "";
                    unitData.NewDetail = false;
                    unitModels.Add(unitData);
                });
                model.Units = unitModels.ToArray();
                #endregion

                model.Warehouses = warehousePrmList.ToArray();
            }

            return model;
        }

        public ItemModel GetItem(string itemNo)
        {
            ItemModel model = new ItemModel { };

            var repo = _unitOfWork.GetRepository<Item>();
            var repoWarehouse = _unitOfWork.GetRepository<Warehouse>();

            var dbObj = repo.Get(d => d.ItemNo == itemNo);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);

                #region WAREHOUSE RESTRICTIONS BY ITEM TYPE
                var warehouseList = repoWarehouse.Filter(d =>
                    ((model.ItemType == (int)ItemType.RawMaterials || model.ItemType == (int)ItemType.Commercial)
                        && d.WarehouseType == (int)WarehouseType.ItemWarehouse)
                    ||
                    ((model.ItemType == (int)ItemType.SemiProduct) &&
                        (d.WarehouseType == (int)WarehouseType.ItemWarehouse || d.WarehouseType == (int)WarehouseType.ProductWarehouse))
                    ||
                    (model.ItemType == (int)ItemType.Product && d.WarehouseType == (int)WarehouseType.ProductWarehouse)
                ).ToArray();
                #endregion

                #region GET WAREHOUSE PARAMETERS
                List<ItemWarehouseModel> warehousePrmList = new List<ItemWarehouseModel>();

                foreach (var item in warehouseList)
                {
                    ItemWarehouseModel itemWarehousePrm = new ItemWarehouseModel
                    {
                        ItemId = model.Id,
                        WarehouseId = item.Id,
                        IsAllowed = true,
                        MaximumQuantity = null,
                        WarehouseCode = item.WarehouseCode,
                        WarehouseName = item.WarehouseName,
                        MinimumQuantity = null,
                        MinimumBehaviour = (int)ItemCriticalBehaviourType.None,
                        MaximumBehaviour = (int)ItemCriticalBehaviourType.None
                    };

                    var dbItemWarehouse = dbObj.ItemWarehouse.FirstOrDefault(d => d.WarehouseId == item.Id);
                    if (dbItemWarehouse != null)
                    {
                        dbItemWarehouse.MapTo(itemWarehousePrm);
                    }

                    warehousePrmList.Add(itemWarehousePrm);
                }
                #endregion

                #region GET ITEM UNITS
                List<ItemUnitModel> unitModels = new List<ItemUnitModel>();
                dbObj.ItemUnit.ToList().ForEach(d =>
                {
                    ItemUnitModel unitData = new ItemUnitModel();
                    d.MapTo(unitData);
                    unitData.UnitCode = d.UnitType != null ? d.UnitType.UnitCode : "";
                    unitData.UnitName = d.UnitType != null ? d.UnitType.UnitName : "";
                    unitData.NewDetail = false;
                    unitModels.Add(unitData);
                });
                model.Units = unitModels.ToArray();
                #endregion

                model.Warehouses = warehousePrmList.ToArray();
            }

            return model;
        }

        public ItemWarehouseModel[] GetProperWarehouses(ItemType itemType, int? itemId)
        {
            var repo = _unitOfWork.GetRepository<Item>();
            var repoWarehouse = _unitOfWork.GetRepository<Warehouse>();

            var dbObj = repo.Get(d => d.Id == itemId);

            var warehouseList = repoWarehouse.Filter(d =>
                ((itemType == ItemType.RawMaterials || itemType == ItemType.Commercial)
                    && d.WarehouseType == (int)WarehouseType.ItemWarehouse)
                ||
                ((itemType == ItemType.SemiProduct) &&
                    (d.WarehouseType == (int)WarehouseType.ItemWarehouse || d.WarehouseType == (int)WarehouseType.ProductWarehouse))
                ||
                (itemType == ItemType.Product && d.WarehouseType == (int)WarehouseType.ProductWarehouse)
            ).ToArray();

            List<ItemWarehouseModel> warehousePrmList = new List<ItemWarehouseModel>();

            foreach (var item in warehouseList)
            {
                ItemWarehouseModel itemWarehousePrm = new ItemWarehouseModel
                {
                    WarehouseId = item.Id,
                    IsAllowed = true,
                    MaximumQuantity = null,
                    MinimumQuantity = null,
                    WarehouseCode = item.WarehouseCode,
                    WarehouseName = item.WarehouseName,
                    MinimumBehaviour = (int)ItemCriticalBehaviourType.None,
                    MaximumBehaviour = (int)ItemCriticalBehaviourType.None
                };

                if (dbObj != null)
                {
                    var dbItemWarehouse = dbObj.ItemWarehouse.FirstOrDefault(d => d.WarehouseId == item.Id);
                    if (dbItemWarehouse != null)
                    {
                        dbItemWarehouse.MapTo(itemWarehousePrm);
                    }
                }

                warehousePrmList.Add(itemWarehousePrm);
            }

            return warehousePrmList.ToArray();
        }

        public bool HasAnyItem(string itemCode)
        {
            var repo = _unitOfWork.GetRepository<Item>();
            return repo.Any(d => d.ItemNo == itemCode);
        }
        #endregion

        #region ITEM CATEGORY BUSINESS
        public ItemCategoryModel[] GetItemCategoryList()
        {
            List<ItemCategoryModel> data = new List<ItemCategoryModel>();

            var repo = _unitOfWork.GetRepository<ItemCategory>();

            repo.GetAll().ToList().ForEach(d =>
            {
                ItemCategoryModel containerObj = new ItemCategoryModel();
                d.MapTo(containerObj);
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateItemCategory(ItemCategoryModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.ItemCategoryCode))
                    throw new Exception("Kategori kodu girilmelidir.");
                if (string.IsNullOrEmpty(model.ItemCategoryName))
                    throw new Exception("Kategori adı girilmelidir.");

                var repo = _unitOfWork.GetRepository<ItemCategory>();

                if (repo.Any(d => (d.ItemCategoryCode == model.ItemCategoryCode || d.ItemCategoryName == model.ItemCategoryName)
                    && d.Id != model.Id))
                    throw new Exception("Aynı koda sahip başka bir kategori mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new ItemCategory();
                    dbObj.CreatedDate = DateTime.Now;
                    dbObj.CreatedUserId = model.CreatedUserId;
                    repo.Add(dbObj);
                }

                var crDate = dbObj.CreatedDate;

                model.MapTo(dbObj);

                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = crDate;

                dbObj.UpdatedDate = DateTime.Now;

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

        public BusinessResult DeleteItemCategory(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemCategory>();

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

        public ItemCategoryModel GetItemCategory(int id)
        {
            ItemCategoryModel model = new ItemCategoryModel { };

            var repo = _unitOfWork.GetRepository<ItemCategory>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }

        public ItemCategoryModel GetItemCategory(string categoryCode)
        {
            ItemCategoryModel model = new ItemCategoryModel { };

            var repo = _unitOfWork.GetRepository<ItemCategory>();
            var dbObj = repo.Get(d => d.ItemCategoryCode == categoryCode);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }

        public bool HasAnyItemCategory(string itemCategoryCode)
        {
            var repo = _unitOfWork.GetRepository<ItemCategory>();
            return repo.Any(d => d.ItemCategoryCode == itemCategoryCode);
        }

        #endregion

        #region ITEM GROUP BUSINESS
        public ItemGroupModel[] GetItemGroupList()
        {
            List<ItemGroupModel> data = new List<ItemGroupModel>();

            var repo = _unitOfWork.GetRepository<ItemGroup>();

            repo.GetAll().ToList().ForEach(d =>
            {
                ItemGroupModel containerObj = new ItemGroupModel();
                d.MapTo(containerObj);
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateItemGroup(ItemGroupModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.ItemGroupCode))
                    throw new Exception("Grup kodu girilmelidir.");
                if (string.IsNullOrEmpty(model.ItemGroupName))
                    throw new Exception("Grup adı girilmelidir.");

                var repo = _unitOfWork.GetRepository<ItemGroup>();

                if (repo.Any(d => (d.ItemGroupCode == model.ItemGroupCode || d.ItemGroupName == model.ItemGroupName)
                    && d.Id != model.Id))
                    throw new Exception("Aynı koda sahip başka bir grup mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new ItemGroup();
                    dbObj.CreatedDate = DateTime.Now;
                    dbObj.CreatedUserId = model.CreatedUserId;
                    repo.Add(dbObj);
                }

                var crDate = dbObj.CreatedDate;

                model.MapTo(dbObj);

                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = crDate;

                dbObj.UpdatedDate = DateTime.Now;

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

        public BusinessResult DeleteItemGroup(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemGroup>();

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

        public ItemGroupModel GetItemGroup(int id)
        {
            ItemGroupModel model = new ItemGroupModel { };

            var repo = _unitOfWork.GetRepository<ItemGroup>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }

        public ItemGroupModel GetItemGroup(string groupCode)
        {
            ItemGroupModel model = new ItemGroupModel { };

            var repo = _unitOfWork.GetRepository<ItemGroup>();
            var dbObj = repo.Get(d => d.ItemGroupCode == groupCode);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }

        public bool HasAnyItemGroup(string itemGroupCode)
        {
            var repo = _unitOfWork.GetRepository<ItemGroup>();
            return repo.Any(d => d.ItemGroupCode == itemGroupCode);
        }
        #endregion

        #region WAREHOUSE BUSINESS
        public WarehouseModel[] GetWarehouseList()
        {
            List<WarehouseModel> data = new List<WarehouseModel>();

            var repo = _unitOfWork.GetRepository<Warehouse>();

            repo.GetAll().ToList().ForEach(d =>
            {
                WarehouseModel containerObj = new WarehouseModel();
                d.MapTo(containerObj);
                containerObj.WarehouseTypeStr = d.WarehouseType != null ? ((WarehouseType)d.WarehouseType.Value).ToCaption() : "";
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateWarehouse(WarehouseModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.WarehouseCode))
                    throw new Exception("Depo kodu girilmelidir.");
                if (string.IsNullOrEmpty(model.WarehouseName))
                    throw new Exception("Depo adı girilmelidir.");
                if ((model.WarehouseType ?? 0) == 0)
                    throw new Exception("Depo türü seçilmelidir.");

                var repo = _unitOfWork.GetRepository<Warehouse>();

                if (repo.Any(d => (d.WarehouseCode == model.WarehouseCode || d.WarehouseName == model.WarehouseName)
                    && d.Id != model.Id))
                    throw new Exception("Aynı koda sahip başka bir depo mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new Warehouse();
                    dbObj.CreatedDate = DateTime.Now;
                    dbObj.CreatedUserId = model.CreatedUserId;
                    repo.Add(dbObj);
                }

                var crDate = dbObj.CreatedDate;

                model.MapTo(dbObj);

                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = crDate;

                dbObj.UpdatedDate = DateTime.Now;

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

        public BusinessResult DeleteWarehouse(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<Warehouse>();

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

        public WarehouseModel GetWarehouse(int id)
        {
            WarehouseModel model = new WarehouseModel { };

            var repo = _unitOfWork.GetRepository<Warehouse>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }

        #endregion

        #region ITEM UNIT BUSINESS

        public BusinessResult SaveOrUpdateUnitType(UnitTypeModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.UnitCode))
                    throw new Exception("Birim kodu girilmelidir.");
                if (string.IsNullOrEmpty(model.UnitName))
                    throw new Exception("Birim adı girilmelidir.");

                var repo = _unitOfWork.GetRepository<UnitType>();

                if (repo.Any(d => (d.UnitCode == model.UnitCode)
                    && d.Id != model.Id))
                    throw new Exception("Aynı koda sahip başka bir birim mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new UnitType();
                    dbObj.CreatedDate = DateTime.Now;
                    dbObj.CreatedUserId = model.CreatedUserId;
                    repo.Add(dbObj);
                }

                var crDate = dbObj.CreatedDate;

                model.MapTo(dbObj);

                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = crDate;

                dbObj.UpdatedDate = DateTime.Now;

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

        public BusinessResult DeleteUnitType(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<UnitType>();

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

        public UnitTypeModel GetUnitType(int id)
        {
            UnitTypeModel model = new UnitTypeModel { };

            var repo = _unitOfWork.GetRepository<UnitType>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }

        public UnitTypeModel GetUnitType(string unitCode)
        {
            UnitTypeModel model = new UnitTypeModel { };

            var repo = _unitOfWork.GetRepository<UnitType>();
            var dbObj = repo.Get(d => d.UnitCode == unitCode);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }
        public UnitTypeModel[] GetUnitTypeList()
        {
            List<UnitTypeModel> data = new List<UnitTypeModel>();

            var repo = _unitOfWork.GetRepository<UnitType>();

            repo.GetAll().ToList().ForEach(d =>
            {
                UnitTypeModel containerObj = new UnitTypeModel();
                d.MapTo(containerObj);
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public bool HasAnyUnitType(string unitCode)
        {
            var repo = _unitOfWork.GetRepository<UnitType>();
            return repo.Any(d => d.UnitCode == unitCode);
        }
        #endregion

        #region FOREX BUSINESS
        public ForexTypeModel GetForexType(int id)
        {
            ForexTypeModel model = new ForexTypeModel();

            var repo = _unitOfWork.GetRepository<ForexType>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }

        public ForexTypeModel GetForexType(string forexCode)
        {
            ForexTypeModel model = new ForexTypeModel();

            var repo = _unitOfWork.GetRepository<ForexType>();
            var dbObj = repo.Get(d => d.ForexTypeCode == forexCode);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }

        public ForexTypeModel[] GetForexTypeList()
        {
            var repo = _unitOfWork.GetRepository<ForexType>();
            var dataList = repo.GetAll();

            List<ForexTypeModel> modelList = new List<ForexTypeModel>();
            dataList.ToList().ForEach(d => {
                var containerObj = new ForexTypeModel();
                modelList.Add(d.MapTo(containerObj));
            });

            return modelList.ToArray();
        }

        public BusinessResult SaveOrUpdateForexType(ForexTypeModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.ForexTypeCode))
                    throw new Exception("Döviz kuru cinsi girilmelidir.");

                var repo = _unitOfWork.GetRepository<ForexType>();
                if (repo.Any(d => d.ForexTypeCode == model.ForexTypeCode && d.Id != model.Id))
                    throw new Exception("Bu döviz cinsi zaten tanımlı.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new ForexType();
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

        public BusinessResult DeleteForexType(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ForexType>();

                var dbObj = repo.Get(d => d.Id == id);
                if (dbObj.ItemOrderDetail.Any())
                    throw new Exception("Bu döviz cinsi malzeme hareketlerinde kullanıldığı için silinemez.");

                var repoHist = _unitOfWork.GetRepository<ForexHistory>();
                var histArr = dbObj.ForexHistory.ToArray();
                foreach (var item in histArr)
                {
                    repoHist.Delete(item);
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

        public ForexHistoryModel GetForexValue(string forexCode, DateTime forexDate)
        {
            ForexHistoryModel model = new ForexHistoryModel();

            IUnitOfWork _outerWork = new EFUnitOfWork();

            var repoForex = _outerWork.GetRepository<ForexType>();
            var repo = _outerWork.GetRepository<ForexHistory>();

            var dbObj = repo.Get(d => d.ForexType.ForexTypeCode == forexCode && d.HistoryDate == forexDate.Date);
            if (dbObj == null)
            {
                XmlDocument xmlDoc = new XmlDocument();

                string tcmbString = "http://www.tcmb.gov.tr/kurlar/" +
                        string.Format("{0:yyyy}", forexDate) + string.Format("{0:MM}", forexDate) + "/" +
                        string.Format("{0:ddMMyyyy}", forexDate)
                    //201707/26072017 tcmb formatı
                    + ".xml";

                if (forexDate.Date == DateTime.Now.Date)
                    tcmbString = "http://www.tcmb.gov.tr/kurlar/today.xml";

                bool _kurBilgisiWebdenYuklendi = false;
                int _yuklemeSayaci = 0;

                while (!_kurBilgisiWebdenYuklendi && _yuklemeSayaci < 15) // kur bilgisi yüklenene kadar döngüde kal, fakat en fazla 15 kez sorgula (sonsuz döngüde kalmaması için)
                {
                    try
                    {
                        _yuklemeSayaci++; // her yükleme denemesinde sayacı arttır
                        xmlDoc.Load(tcmbString);

                        _kurBilgisiWebdenYuklendi = true; // Load metodu patlamıyorsa aşağı devam eder ve bayrak = true olur
                    }
                    catch (Exception)
                    {
                        // kur yoksa hataya düşer ve tarihi bir gün geri çekeriz
                        forexDate = forexDate.AddDays(-1);

                        // sorgu için http url'ini güncelliyoruz
                        tcmbString = "http://www.tcmb.gov.tr/kurlar/" +
                        string.Format("{0:yyyy}", forexDate) + string.Format("{0:MM}", forexDate) + "/" +
                        string.Format("{0:ddMMyyyy}", forexDate)
                            + ".xml";
                    }
                }

                string alisData = xmlDoc.SelectSingleNode("Tarih_Date/Currency[@Kod='" + forexCode + "']/ForexBuying").InnerXml;
                string satisData = xmlDoc.SelectSingleNode("Tarih_Date/Currency[@Kod='" + forexCode + "']/ForexSelling").InnerXml;

                if (!string.IsNullOrEmpty(satisData))
                {
                    decimal alisKuru = Decimal.Parse(alisData.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                    decimal satisKuru = Decimal.Parse(satisData.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);

                    // CHECK NEW DATE IF IT WAS DECREASED
                    if (repo.Any(d => d.ForexType.ForexTypeCode == forexCode && d.HistoryDate == forexDate.Date))
                    {
                        dbObj = repo.Get(d => d.ForexType.ForexTypeCode == forexCode && d.HistoryDate == forexDate.Date);
                        dbObj.SalesForexRate = satisKuru;
                        dbObj.BuyForexRate = alisKuru;

                        _outerWork.SaveChanges();
                    }
                    else
                    {
                        var dbForex = repoForex.Get(d => d.ForexTypeCode == forexCode);
                        if (dbForex != null)
                        {
                            dbObj = new ForexHistory
                            {
                                ForexId = dbForex.Id,
                                BuyForexRate = alisKuru,
                                SalesForexRate = satisKuru,
                                HistoryDate = forexDate.Date
                            };
                            repo.Add(dbObj);

                            _outerWork.SaveChanges();
                        }
                        else
                            throw new Exception(forexCode + " döviz cinsi sistemde tanımlı değil.");
                    }
                }
            }

            model.Id = dbObj.Id;
            model.ForexId = dbObj.ForexId;
            model.BuyForexRate = dbObj.BuyForexRate;
            model.SalesForexRate = dbObj.SalesForexRate;
            model.HistoryDate = dbObj.HistoryDate;

            return model;
        }
        #endregion

        #region MACHINE BUSINESS
        public MachineModel[] GetMachineList()
        {
            List<MachineModel> data = new List<MachineModel>();

            var repo = _unitOfWork.GetRepository<Machine>();

            repo.GetAll().ToList().ForEach(d =>
            {
                MachineModel containerObj = new MachineModel();
                d.MapTo(containerObj);
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public MachineModel[] GetMachineStats(string startDate, string endDate)
        {
            List<MachineModel> data = new List<MachineModel>();

            if (string.IsNullOrEmpty(startDate))
                startDate = string.Format("{0:dd.MM.yyyy}", DateTime.Now.AddMonths(-1));
            if (string.IsNullOrEmpty(endDate))
                endDate = string.Format("{0:dd.MM.yyyy}", DateTime.Now);

            DateTime dt1 = DateTime.ParseExact(startDate + " 00:00:00", "dd.MM.yyyy HH:mm:ss", System.Globalization.CultureInfo.GetCultureInfo("tr"));
            DateTime dt2 = DateTime.ParseExact(endDate + " 23:59:59", "dd.MM.yyyy HH:mm:ss", System.Globalization.CultureInfo.GetCultureInfo("tr"));

            var repo = _unitOfWork.GetRepository<Machine>();
            var repoSignal = _unitOfWork.GetRepository<MachineSignal>();

            repo.GetAll().ToList().ForEach(d =>
            {
                MachineModel containerObj = new MachineModel();
                d.MapTo(containerObj);

                var signalData = repoSignal.Filter(m => m.MachineId == d.Id &&
                    dt1 <= m.StartDate && dt2 >= m.StartDate);

                containerObj.MachineStats = new Models.DataTransfer.Summary.MachineStatsModel
                {
                    AvgInflationTime = Convert.ToDecimal(signalData.Average(m => m.Duration) ?? 0),
                    AvgProductionCount = signalData.Count(),
                };

                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateMachine(MachineModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.MachineCode))
                    throw new Exception("Makine kodu girilmelidir.");
                if (string.IsNullOrEmpty(model.MachineName))
                    throw new Exception("Makine adı girilmelidir.");

                var repo = _unitOfWork.GetRepository<Machine>();

                if (repo.Any(d => (d.MachineCode == model.MachineCode)
                    && d.Id != model.Id))
                    throw new Exception("Aynı koda sahip başka bir makine mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new Machine();
                    dbObj.CreatedDate = DateTime.Now;
                    dbObj.CreatedUserId = model.CreatedUserId;
                    repo.Add(dbObj);
                }

                var crDate = dbObj.CreatedDate;

                model.MapTo(dbObj);

                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = crDate;

                dbObj.UpdatedDate = DateTime.Now;

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

        public BusinessResult DeleteMachine(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<Machine>();

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

        public MachineModel GetMachine(int id)
        {
            MachineModel model = new MachineModel { };

            var repo = _unitOfWork.GetRepository<Machine>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }

        #endregion

        #region DYE BUSINESS
        public DyeModel[] GetDyeList()
        {
            List<DyeModel> data = new List<DyeModel>();

            var repo = _unitOfWork.GetRepository<Dye>();

            repo.GetAll().ToList().ForEach(d =>
            {
                DyeModel containerObj = new DyeModel();
                d.MapTo(containerObj);
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateDye(DyeModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.DyeCode))
                    throw new Exception("Renk kodu girilmelidir.");
                if (string.IsNullOrEmpty(model.DyeName))
                    throw new Exception("Renk adı girilmelidir.");

                var repo = _unitOfWork.GetRepository<Dye>();

                if (repo.Any(d => (d.DyeCode == model.DyeCode)
                    && d.Id != model.Id))
                    throw new Exception("Aynı koda sahip başka bir renk mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new Dye();
                    dbObj.CreatedDate = DateTime.Now;
                    dbObj.CreatedUserId = model.CreatedUserId;
                    repo.Add(dbObj);
                }

                var crDate = dbObj.CreatedDate;

                model.MapTo(dbObj);

                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = crDate;

                dbObj.UpdatedDate = DateTime.Now;

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

        public BusinessResult DeleteDye(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<Dye>();

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

        public DyeModel GetDye(int id)
        {
            DyeModel model = new DyeModel { };

            var repo = _unitOfWork.GetRepository<Dye>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }

        #endregion

        #region MOLD BUSINESS
        public MoldModel[] GetMoldList()
        {
            List<MoldModel> data = new List<MoldModel>();

            var repo = _unitOfWork.GetRepository<Mold>();

            repo.GetAll().ToList().ForEach(d =>
            {
                MoldModel containerObj = new MoldModel();
                d.MapTo(containerObj);
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateMold(MoldModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.MoldCode))
                    throw new Exception("Kalıp kodu girilmelidir.");
                if (string.IsNullOrEmpty(model.MoldName))
                    throw new Exception("Kalıp adı girilmelidir.");

                var repo = _unitOfWork.GetRepository<Mold>();

                if (repo.Any(d => (d.MoldCode == model.MoldCode)
                    && d.Id != model.Id))
                    throw new Exception("Aynı koda sahip başka bir kalıp mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new Mold();
                    dbObj.CreatedDate = DateTime.Now;
                    dbObj.CreatedUserId = model.CreatedUserId;
                    repo.Add(dbObj);
                }

                var crDate = dbObj.CreatedDate;

                model.MapTo(dbObj);

                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = crDate;

                dbObj.UpdatedDate = DateTime.Now;

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

        public BusinessResult DeleteMold(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<Mold>();

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

        public MoldModel GetMold(int id)
        {
            MoldModel model = new MoldModel { };

            var repo = _unitOfWork.GetRepository<Mold>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }

        #endregion

        #region SYNC POINTS
        public SyncPointModel[] GetSyncPointList()
        {
            List<SyncPointModel> data = new List<SyncPointModel>();

            var repo = _unitOfWork.GetRepository<SyncPoint>();

            repo.Filter(d => d.IsActive == true).ToList().ForEach(d =>
            {
                SyncPointModel containerObj = new SyncPointModel();
                d.MapTo(containerObj);
                data.Add(containerObj);
            });

            return data.ToArray();
        }
        #endregion
    }
}
