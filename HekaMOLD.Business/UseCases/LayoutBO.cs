using Heka.DataAccess.Context;
using HekaMOLD.Business.Base;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.DataTransfer.Layout;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.UseCases
{
    public class LayoutBO : IBusinessObject
    {
        #region LAYOUT OBJECT TYPE DEFINITIONS
        public LayoutObjectTypeModel[] GetLayoutObjectTypeList()
        {
            var repo = _unitOfWork.GetRepository<LayoutObjectType>();

            return repo.GetAll()
                .Select(d => new LayoutObjectTypeModel
                {
                    Id = d.Id,
                    DataTypeExtension = d.DataTypeExtension,
                    ObjectTypeCode = d.ObjectTypeCode,
                    ObjectTypeName = d.ObjectTypeName,
                }).ToArray();
        }

        public BusinessResult SaveOrUpdateLayoutObjectType(LayoutObjectTypeModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.ObjectTypeCode))
                    throw new Exception("Nesne tip kodu girilmelidir.");
                if (string.IsNullOrEmpty(model.ObjectTypeName))
                    throw new Exception("Nesne tip adı girilmelidir.");

                _unitOfWork.SetTimeout(60);
                var repo = _unitOfWork.GetRepository<LayoutObjectType>();

                if (repo.Any(d => (d.ObjectTypeCode == model.ObjectTypeCode) && d.Id != model.Id))
                    throw new Exception("Aynı koda sahip başka bir nesne tipi mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new LayoutObjectType();
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

        public BusinessResult DeleteLayoutObjectType(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<LayoutObjectType>();

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

        public LayoutObjectTypeModel GetLayoutObjectType(int id)
        {
            LayoutObjectTypeModel model = new LayoutObjectTypeModel { };

            var repo = _unitOfWork.GetRepository<LayoutObjectType>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = new LayoutObjectTypeModel
                {
                    Id = dbObj.Id,
                    ObjectTypeCode = dbObj.ObjectTypeCode,
                    ObjectTypeName = dbObj.ObjectTypeName,
                };
                //model = dbObj.MapTo(model); // slowness effect of mapping a byte array field
            }

            return model;
        }
        #endregion

        #region LAYOUT ITEM MANAGEMENT
        public MachineModel[] GetPlaceableMachines()
        {
            MachineModel[] data = new MachineModel[0];

            var repo = _unitOfWork.GetRepository<Machine>();

            data = repo.Filter(d => d.LayoutItem.Any() == false).Select(d => new MachineModel
            {
                Id = d.Id,
                MachineCode = d.MachineCode,
                MachineName = d.MachineName,
                MachineType = d.MachineType,
                PlantId = d.PlantId,
                MachineGroupCode = d.MachineGroup != null ? d.MachineGroup.MachineGroupCode : "",
                MachineGroupName = d.MachineGroup != null ? d.MachineGroup.MachineGroupName : "",
            }).ToArray();

            return data;
        }
        public LayoutItemModel[] GetLayoutItemList()
        {
            var repo = _unitOfWork.GetRepository<LayoutItem>();

            return repo.GetAll()
                .Select(d => new LayoutItemModel
                {
                    Id = d.Id,
                    MachineId = d.MachineId,
                    PositionData = d.PositionData,
                    RotationData = d.RotationData,
                    ScalingData = d.ScalingData,
                    Title = d.Title,
                    PlantId = d.PlantId,
                    MachineCode = d.Machine.MachineCode,
                    MachineName = d.Machine.MachineName,
                    MachineGroupCode = d.Machine.MachineGroup != null ? d.Machine.MachineGroup.MachineGroupCode : "",
                    MachineGroupName = d.Machine.MachineGroup != null ? d.Machine.MachineGroup.MachineGroupName : "",
                }).ToArray();
        }

        public BusinessResult SaveOrUpdateLayoutItem(LayoutItemModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (model.MachineId == null)
                    throw new Exception("Makine bilgisi verilmelidir.");
                if (model.PlantId == null)
                    throw new Exception("İşletme bilgisi verilmelidir.");

                var repo = _unitOfWork.GetRepository<LayoutItem>();

                var dbObj = repo.Get(d => d.MachineId == model.MachineId);
                if (dbObj == null)
                {
                    dbObj = new LayoutItem();
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

        public BusinessResult DeleteLayoutItem(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<LayoutItem>();

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

        public LayoutItemModel GetLayoutItem(int id)
        {
            LayoutItemModel model = new LayoutItemModel { };

            var repo = _unitOfWork.GetRepository<LayoutItem>();
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
