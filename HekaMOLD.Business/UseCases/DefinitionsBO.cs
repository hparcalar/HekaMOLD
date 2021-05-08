using Heka.DataAccess.Context;
using HekaMOLD.Business.Base;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.UseCases
{
    public class DefinitionsBO : IBusinessObject
    {
        #region FIRM BUSINESS
        public FirmModel[] GetFirmList()
        {
            List<FirmModel> data = new List<FirmModel>();

            var repo = _unitOfWork.GetRepository<Firm>();

            repo.GetAll().ToList().ForEach(d =>
            {
                FirmModel containerObj = new FirmModel();
                d.MapTo(containerObj);
                containerObj.FirmTypeStr = d.FirmType == 1 ? "Tedarikçi" : d.FirmType == 2 ? "Müşteri" : "";
                data.Add(containerObj);
            });

            return data.ToArray();
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
            }

            return model;
        }

        #endregion

        #region ITEM BUSINESS
        public ItemModel[] GetItemList()
        {
            List<ItemModel> data = new List<ItemModel>();

            var repo = _unitOfWork.GetRepository<Item>();

            repo.GetAll().ToList().ForEach(d =>
            {
                ItemModel containerObj = new ItemModel();
                d.MapTo(containerObj);
                containerObj.ItemTypeStr = 
                    d.ItemType == 1 ? "Hammadde" : d.ItemType == 2 ? "Ticari Mal" : 
                    d.ItemType == 3 ? "Yarı Mamul" : d.ItemType == 4 ? "Mamul" : "";
                containerObj.CategoryName = d.ItemCategory != null ? d.ItemCategory.ItemCategoryName : "";
                containerObj.GroupName = d.ItemGroup != null ? d.ItemGroup.ItemGroupName : "";
                data.Add(containerObj);
            });

            return data.ToArray();
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
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
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

        #endregion

        #region ITEM UNIT BUSINESS
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
        #endregion
    }
}
