﻿using Heka.DataAccess.Context;
using Heka.DataAccess.Context.Models;
using Heka.DataAccess.UnitOfWork;
using HekaMOLD.Business.Base;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Logistics;
using HekaMOLD.Business.Models.DataTransfer.Maintenance;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.DataTransfer.Summary;
using HekaMOLD.Business.Models.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
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

                if (repo.Any(d => (d.FirmCode == model.FirmCode) && d.Id != model.Id))
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

        #region VEHICLETYPE BUSINESS
        public VehicleTypeModel[] GetVehicleTypeList()
        {
            List<VehicleTypeModel> data = new List<VehicleTypeModel>();

            var repo = _unitOfWork.GetRepository<VehicleType>();

            repo.GetAll().ToList().ForEach(d =>
            {
                VehicleTypeModel containerObj = new VehicleTypeModel();
                d.MapTo(containerObj);
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateVehicleType(VehicleTypeModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.VehicleTypeCode))
                    throw new Exception("Tip Kodu girilmelidir.");
                if (string.IsNullOrEmpty(model.VehicleTypeCode))
                    throw new Exception("Tip adı girilmelidir.");

                var repo = _unitOfWork.GetRepository<VehicleType>();

                if (repo.Any(d => (d.VehicleTypeCode == model.VehicleTypeCode)
                    && d.Id != model.Id))
                    throw new Exception("Aynı koda sahip başka bir tip mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new VehicleType();
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

        public BusinessResult DeleteVehicleType(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<VehicleType>();

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

        public VehicleTypeModel GetVehicleType(int id)
        {
            VehicleTypeModel model = new VehicleTypeModel { };

            var repo = _unitOfWork.GetRepository<VehicleType>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }
        #endregion

        #region VEHICLE BUSINESS
        public VehicleModel[] GetVehicleList()
        {
            var repo = _unitOfWork.GetRepository<Vehicle>();

            return repo.GetAll()
                .Select(d => new VehicleModel
                {
                    Id = d.Id,
                    Plate = d.Plate,
                    Mark = d.Mark,
                    Versiyon = d.Versiyon,
                    KmHour=d.KmHour,                
                    TrailerTypeStr = d.TrailerType == 1 ? "Çadırlı" : d.TrailerType == 2 ? "Frigo" : "Kapalı",
                }).ToArray();
        }
        public BusinessResult SaveOrUpdateVehicle(VehicleModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.Plate))


                    throw new Exception("Plaka girilmelidir.");

                var repo = _unitOfWork.GetRepository<Vehicle>();
                //var repoAuthors = _unitOfWork.GetRepository<FirmAuthor>();

                if (repo.Any(d => (d.Plate == model.Plate) && d.Id != model.Id))
                    throw new Exception("Aynı plakaya sahip başka bir araç mevcuttur. Lütfen farklı bir plaka giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new Vehicle();
                    dbObj.CreatedDate = DateTime.Now;
                    dbObj.CreatedUserId = model.CreatedUserId;
                    repo.Add(dbObj);
                }

                var crDate = dbObj.CreatedDate;
                model.MapTo(dbObj);

                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = crDate;
                dbObj.UpdatedDate = DateTime.Now;

                #region SAVE CARE LIST
                //if (model.Authors == null)
                //    model.Authors = new FirmAuthorModel[0];

                //var toBeRemovedAuthors = dbObj.FirmAuthor
                //    .Where(d => !model.Authors.Where(m => m.NewDetail == false)
                //        .Select(m => m.Id).ToArray().Contains(d.Id)
                //    ).ToArray();
                //foreach (var item in toBeRemovedAuthors)
                //{
                //    repoAuthors.Delete(item);
                //}

                //foreach (var item in model.Authors)
                //{
                //    if (item.NewDetail == true)
                //    {
                //        var dbItemAu = new FirmAuthor();
                //        item.MapTo(dbItemAu);
                //        dbItemAu.Firm = dbObj;
                //        repoAuthors.Add(dbItemAu);
                //    }
                //    else if (!toBeRemovedAuthors.Any(d => d.Id == item.Id))
                //    {
                //        var dbItemAu = repoAuthors.GetById(item.Id);
                //        item.MapTo(dbItemAu);
                //        dbItemAu.Firm = dbObj;
                //    }
                //}
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
        public BusinessResult DeleteVehicle(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<Vehicle>();

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
        public VehicleModel GetVehicle(int id)
        {
            VehicleModel model = new VehicleModel { };

            var repo = _unitOfWork.GetRepository<Vehicle>();

            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.ContractStartDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.ContractStartDate);
                model.ContractEndDateStr = string.Format("{0:dd.MM.yyyy}",dbObj.ContractEndDate);
                #region GET VEHICLECARE 
                List<VehicleCareModel> vehicleCareList = new List<VehicleCareModel>();
                dbObj.VehicleCare.ToList().ForEach(d =>
                {
                    VehicleCareModel vehicleCareModel = new VehicleCareModel();
                    vehicleCareModel.Plate = d.Vehicle != null ? d.Vehicle.Plate : "";
                    vehicleCareModel.Amount = d.Amount;
                    vehicleCareModel.FirmName = d.Firm != null ? d.Firm.FirmName : "";
                    vehicleCareModel.ForexTypeCode = d.ForexType != null ? d.ForexType.ForexTypeCode : "";
                    vehicleCareModel.KmHour = d.KmHour;
                    vehicleCareModel.VehicleCareTypeName = d.VehicleCareType != null ? d.VehicleCareType.VehicleCareTypeName : "";
                    vehicleCareModel.CareDateStr = string.Format("{0:dd.MM.yyyy}", d.CareDate);
                    d.MapTo(vehicleCareModel);
                    vehicleCareList.Add(vehicleCareModel);
                });
                model.VehicleCares = vehicleCareList.ToArray();
                #endregion
                #region GET VEHICLEINSURANCE 
                List<VehicleInsuranceModel> insuranceList = new List<VehicleInsuranceModel>();
                dbObj.VehicleInsurance.ToList().ForEach(d =>
                {
                    VehicleInsuranceModel insuranceModel = new VehicleInsuranceModel();
                    insuranceModel.Amount = d.Amount;
                    insuranceModel.StartDateStr = string.Format("{0:dd.MM.yyyy}", d.StartDate);
                    insuranceModel.EndDateStr = string.Format("{0:dd.MM.yyyy}", d.EndDate);
                    insuranceModel.FirmName = d.Firm != null ? d.Firm.FirmName : "";
                    insuranceModel.ForexTypeCode = d.ForexType != null ? d.ForexType.ForexTypeCode :"";
                    insuranceModel.KmHour = d.KmHour;
                    insuranceModel.Plate = d.Vehicle != null ? d.Vehicle.Plate : "";
                    insuranceModel.VehicleInsuranceTypeName = d.VehicleInsuranceType != null ? d.VehicleInsuranceType.VehicleInsuranceTypeName : "";
                    d.MapTo(insuranceModel);
                    insuranceList.Add(insuranceModel);
                });
                model.VehicleInsurances = insuranceList.ToArray();
                #endregion
                #region GET VEHICLETIRE
                List<VehicleTireModel> tireList = new List<VehicleTireModel>();
                dbObj.VehicleTire.ToList().ForEach(d =>
                {
                    VehicleTireModel tireModel = new VehicleTireModel();
                    tireModel.Amount = d.Amount;
                    tireModel.DimensionsInfo = d.DimensionsInfo;
                    tireModel.FirmName = d.Firm != null ? d.Firm.FirmName:"";
                    tireModel.ForexTypeCode = d.ForexType != null ? d.ForexType.ForexTypeCode : "";
                    tireModel.KmHour = d.KmHour;
                    tireModel.MontageDateStr = string.Format("{0:dd.MM.yyyy}",d.MontageDate);
                    tireModel.Plate = d.Vehicle != null ? d.Vehicle.Plate : "";
                    tireModel.SeriNo = d.SeriNo;
                    tireModel.VehicleTireDirectionTypeName = d.VehicleTireDirectionType != null ? d.VehicleTireDirectionType.VehicleTireDirectionTypeName : "";
                    tireModel.VehicleTireTypeStr = d.VehicleTireType == 1 ? "Değişim" : d.VehicleTireType == 2 ? "Onarım" :  d.VehicleTireType == 3 ? "Bakım" : "";
                    d.MapTo(tireModel);
                    tireList.Add(tireModel);
                });
                model.VehicleTires = tireList.ToArray();
                #endregion
            }

            return model;
        }
        #endregion

        #region VEHICLEINSURANCE BUSINESS

        public VehicleInsuranceModel[] GetVehicleInsuranceList()
        {
            List<VehicleInsuranceModel> data = new List<VehicleInsuranceModel>();

            var repo = _unitOfWork.GetRepository<VehicleInsurance>();

            return repo.GetAll()
               .ToList()
               .Select(d => new VehicleInsuranceModel
               {
                   Id = d.Id,
                   FirmCode = d.Firm != null ? d.Firm.FirmCode : "",
                   FirmName = d.Firm != null ? d.Firm.FirmName : "",
                   Plate = d.Vehicle.Plate,
                   Amount = d.Amount,
                   StartDateStr = string.Format("{0:dd.MM.yyyy}", d.StartDate),
                   EndDateStr = string.Format("{0:dd.MM.yyyy}", d.EndDate),
                   ForexTypeCode = d.ForexType != null ? d.ForexType.ForexTypeCode : "",
                   KmHour = d.KmHour,
                   VehicleInsuranceTypeName = d.VehicleInsuranceType != null ? d.VehicleInsuranceType.VehicleInsuranceTypeName : "",
               }).ToArray();
        }

        public BusinessResult SaveOrUpdateVehicleInsurance(VehicleInsuranceModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.Plate))
                    throw new Exception("Plaka girilmelidir.");
                if (string.IsNullOrEmpty(Convert.ToString(model.StartDate)))
                    throw new Exception("Başlama tarihi girilmelidir.");
                if (string.IsNullOrEmpty(Convert.ToString(model.EndDate)))
                    throw new Exception("Bitiş tarihi girilmelidir.");

                var repo = _unitOfWork.GetRepository<VehicleInsurance>();

                //if (repo.Any(d => (d.FirmCode == model.FirmCode) && d.Id != model.Id))
                //    throw new Exception("Aynı koda sahip başka bir firma mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new VehicleInsurance();
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

        public BusinessResult DeleteVehicleInsurance(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<VehicleInsurance>();

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

        public VehicleInsuranceModel GetVehicleInsurance(int id)
        {
            VehicleInsuranceModel model = new VehicleInsuranceModel { };

            var repo = _unitOfWork.GetRepository<VehicleInsurance>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.StartDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.StartDate);
                model.EndDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.EndDate);

            }

            return model;
        }

        #endregion

        #region VEHICLEINSURANCETYPE BUSINESS
        public VehicleInsuranceTypeModel[] GetVehicleInsuranceTypeList()
        {
            List<VehicleInsuranceTypeModel> data = new List<VehicleInsuranceTypeModel>();

            var repo = _unitOfWork.GetRepository<VehicleInsuranceType>();

            repo.GetAll().ToList().ForEach(d =>
            {
                VehicleInsuranceTypeModel containerObj = new VehicleInsuranceTypeModel();
                d.MapTo(containerObj);
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateVehicleInsuranceType(VehicleInsuranceTypeModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.VehicleInsuranceTypeCode))
                    throw new Exception("Plaka girilmelidir.");

                var repo = _unitOfWork.GetRepository<VehicleInsuranceType>();

                if (repo.Any(d => (d.VehicleInsuranceTypeCode == model.VehicleInsuranceTypeCode) && d.Id != model.Id))
                    throw new Exception("Aynı koda sahip başka bir sigorta mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new VehicleInsuranceType();
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

        public BusinessResult DeleteVehicleInsuranceType(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<VehicleInsuranceType>();

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

        public VehicleInsuranceTypeModel GetVehicleInsuranceType(int id)
        {
            VehicleInsuranceTypeModel model = new VehicleInsuranceTypeModel { };

            var repo = _unitOfWork.GetRepository<VehicleInsuranceType>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }
        #endregion

        #region VEHICLECARE BUSINESS
        public VehicleCareModel[] GetVehicleCareList()
        {

            var repo = _unitOfWork.GetRepository<VehicleCare>();

            return repo.GetAll().ToList()
                .Select(d => new VehicleCareModel
                {
                    Id = d.Id,
                    Plate = d.Vehicle != null ? d.Vehicle.Plate :"",
                    VehicleCareTypeName = d.VehicleCareType !=null ? d.VehicleCareType.VehicleCareTypeName : "",
                    FirmCode = d.Firm != null ? d.Firm.FirmCode :"",
                    FirmName = d.Firm != null ? d.Firm.FirmName : "",
                    KmHour = d.KmHour,
                    ForexTypeCode = d.ForexType != null ? d.ForexType.ForexTypeCode:"",
                    Amount = d.Amount,
                    Explanation = d.Explanation,
                    CareDateStr = string.Format("{0:dd.MM.yyyy}", d.CareDate),

                }).ToArray(); ;
        }

        public BusinessResult SaveOrUpdateVehicleCare(VehicleCareModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(Convert.ToString( model.VehicleId)))
                    throw new Exception("Plaka girilmelidir.");
                if (string.IsNullOrEmpty(Convert.ToString(model.VehicleCareTypeId)))
                    throw new Exception("Bakım tip kod girilmelidir.");
        
                var repo = _unitOfWork.GetRepository<VehicleCare>();

                //if (repo.Any(d => (d.FirmCode == model.FirmCode) && d.Id != model.Id))
                //    throw new Exception("Aynı koda sahip başka bir firma mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new VehicleCare();
                    dbObj.CreatedDate = DateTime.Now;
                    dbObj.CreatedUserId = model.CreatedUserId;
                    repo.Add(dbObj);
                }

                var crDate = dbObj.CreatedDate;
                if (!string.IsNullOrEmpty(model.CareDateStr))
                {
                    model.CareDate = DateTime.ParseExact(model.CareDateStr, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                }
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

        public BusinessResult DeleteVehicleCare(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<VehicleCare>();

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

        public VehicleCareModel GetVehicleCare(int id)
        {
            VehicleCareModel model = new VehicleCareModel { };

            var repo = _unitOfWork.GetRepository<VehicleCare>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.CareDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.CareDate);

            }

            return model;
        }


        #endregion

        #region VEHICLECARETYPE BUSINESS
        public VehicleCareTypeModel[] GetVehicleCareTypeList()
        {
            List<VehicleCareTypeModel> data = new List<VehicleCareTypeModel>();

            var repo = _unitOfWork.GetRepository<VehicleCareType>();

            repo.GetAll().ToList().ForEach(d =>
            {
                VehicleCareTypeModel containerObj = new VehicleCareTypeModel();
                d.MapTo(containerObj);
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateVehicleCareType(VehicleCareTypeModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.VehicleCareTypeCode))
                    throw new Exception("Bakım tip Kodu girilmelidir.");

                var repo = _unitOfWork.GetRepository<VehicleCareType>();

                if (repo.Any(d => (d.VehicleCareTypeCode == model.VehicleCareTypeCode)
                    && d.Id != model.Id))
                    throw new Exception("Aynı koda sahip başka bir sigorta tip mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new VehicleCareType();
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

        public BusinessResult DeleteVehicleCareType(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<VehicleCareType>();

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

        public VehicleCareTypeModel GetVehicleCareType(int id)
        {
            VehicleCareTypeModel model = new VehicleCareTypeModel { };

            var repo = _unitOfWork.GetRepository<VehicleCareType>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }
        #endregion

        #region VEHICLETIRE BUSINESS 
        public VehicleTireModel[] GetVehicleTireList()
        {
            var repo = _unitOfWork.GetRepository<VehicleTire>();
            return repo.GetAll().ToList()
                .Select(d => new VehicleTireModel
                {
                    Id = d.Id,
                    Plate = d.Vehicle != null ? d.Vehicle.Plate:"",
                    VehicleTireTypeStr = d.VehicleTireType == 1 ? "Değişim" : d.VehicleTireType == 2 ? "Onarım" :
                        d.VehicleTireType == 3 ? "Bakım" : "",
                    VehicleTireDirectionTypeName = d.VehicleTireDirectionType.VehicleTireDirectionTypeName !=null ? d.VehicleTireDirectionType.VehicleTireDirectionTypeName:"",
                    FirmCode = d.Firm != null ? d.Firm.FirmCode:"",
                    FirmName = d.Firm != null ? d.Firm.FirmName:"",
                    KmHour = d.KmHour,
                    ForexTypeCode =d.ForexType != null ? d.ForexType.ForexTypeCode:"",
                    Amount = d.Amount,
                    Explanation = d.Explanation,
                    MontageDateStr = string.Format("{0:dd.MM.yyyy}", d.MontageDate),

                }).ToArray();
        }

        public BusinessResult SaveOrUpdateVehicleTire(VehicleTireModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(model.VehicleId)))
                    throw new Exception("Plaka girilmelidir.");
                if (string.IsNullOrEmpty(Convert.ToString(model.MontageDate)))
                    throw new Exception("Montaj tarihi girilmelidir.");

                var repo = _unitOfWork.GetRepository<VehicleTire>();

                //if (repo.Any(d => (d.MontageDate == model.MontageDat) && d.Id != model.Id))
                //    throw new Exception("Aynı koda sahip başka bir firma mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new VehicleTire();
                    dbObj.CreatedDate = DateTime.Now;
                    dbObj.CreatedUserId = model.CreatedUserId;
                    repo.Add(dbObj);
                }

                var crDate = dbObj.CreatedDate;
                if (!string.IsNullOrEmpty(model.MontageDateStr))
                {
                    model.MontageDate = DateTime.ParseExact(model.MontageDateStr, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                }
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

        public BusinessResult DeleteVehicleTire(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<VehicleTire>();

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

        public VehicleTireModel GetVehicleTire(int id)
        {
            VehicleTireModel model = new VehicleTireModel { };

            var repo = _unitOfWork.GetRepository<VehicleTire>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.MontageDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.MontageDate);
            }

            return model;
        }
        #endregion

        #region VEHICLETIREDIRECTIONTYPE BUSINESS

        public VehicleTireDirectionTypeModel[] GetVehicleTireDirectionTypeList()
        {
            List<VehicleTireDirectionTypeModel> data = new List<VehicleTireDirectionTypeModel>();

            var repo = _unitOfWork.GetRepository<VehicleTireDirectionType>();

            repo.GetAll().ToList().ForEach(d =>
            {
                VehicleTireDirectionTypeModel containerObj = new VehicleTireDirectionTypeModel();
                d.MapTo(containerObj);
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateVehicleTireDirectionType(VehicleTireDirectionTypeModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.VehicleTireDirectionTypeCode))
                    throw new Exception("Kod girilmelidir.");

                var repo = _unitOfWork.GetRepository<VehicleTireDirectionType>();

                if (repo.Any(d => (d.VehicleTireDirectionTypeCode == model.VehicleTireDirectionTypeCode) && d.Id != model.Id))
                    throw new Exception("Aynı koda sahip başka bir tip mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new VehicleTireDirectionType();
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

        public BusinessResult DeleteVehicleTireDirectionType(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<VehicleTireDirectionType>();

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

        public VehicleTireDirectionTypeModel GetVehicleTireDirectionType(int id)
        {
            VehicleTireDirectionTypeModel model = new VehicleTireDirectionTypeModel { };

            var repo = _unitOfWork.GetRepository<VehicleTireDirectionType>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }

        #endregion

        #region CUSTOMS BUSINESS
        public CustomsModel[] GetCustomsList()
        {
            List<CustomsModel> data = new List<CustomsModel>();

            var repo = _unitOfWork.GetRepository<Customs>();

            repo.GetAll().ToList().ForEach(d =>
            {
                CustomsModel containerObj = new CustomsModel();
                d.MapTo(containerObj);
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateCustoms(CustomsModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.CustomsCode))
                    throw new Exception("Gümrük kodu girilmelidir.");
                if (string.IsNullOrEmpty(model.CustomsName))
                    throw new Exception("Gümrük adı girilmelidir.");

                var repo = _unitOfWork.GetRepository<Customs>();

                if (repo.Any(d => (d.CustomsCode == model.CustomsCode)
                    && d.Id != model.Id))
                    throw new Exception("Aynı koda sahip başka bir gümrük mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new Customs();
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

        public BusinessResult DeleteCustoms(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<Customs>();

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

        public CustomsModel GetCustoms(int id)
        {
            CustomsModel model = new CustomsModel { };

            var repo = _unitOfWork.GetRepository<Customs>();
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
            var repo = _unitOfWork.GetRepository<Item>();

            return repo.GetAll().Select(d => new ItemModel
            {
                Id = d.Id,
                ItemNo = d.ItemNo,
                ItemName = d.ItemName,
                ItemTypeStr = d.ItemType == 1 ? "Hammadde" : d.ItemType == 2 ? "Ticari Mal" :
                        d.ItemType == 3 ? "Yarı Mamul" : d.ItemType == 4 ? "Mamul" : "",
                ItemType = d.ItemType,
                CategoryName = d.ItemCategory != null ? d.ItemCategory.ItemCategoryName : "",
                GroupName = d.ItemGroup != null ? d.ItemGroup.ItemGroupName : "",
                //TotalInQuantity = d.ItemLiveStatus.Sum(m => m.InQuantity) ?? 0,
                //TotalOutQuantity = d.ItemLiveStatus.Sum(m => m.OutQuantity) ?? 0,
                //TotalOverallQuantity = d.ItemLiveStatus.Sum(m => m.LiveQuantity) ?? 0,
            }).ToArray();
        }

        public ItemModel[] GetItemListWithStates()
        {
            var repo = _unitOfWork.GetRepository<Item>();

            return repo.GetAll().Select(d => new ItemModel
            {
                Id = d.Id,
                ItemNo = d.ItemNo,
                ItemName = d.ItemName,
                ItemTypeStr = d.ItemType == 1 ? "Hammadde" : d.ItemType == 2 ? "Ticari Mal" :
                        d.ItemType == 3 ? "Yarı Mamul" : d.ItemType == 4 ? "Mamul" : "",
                ItemType = d.ItemType,
                CategoryName = d.ItemCategory != null ? d.ItemCategory.ItemCategoryName : "",
                GroupName = d.ItemGroup != null ? d.ItemGroup.ItemGroupName : "",
                //TotalInQuantity = d.ItemLiveStatus.Sum(m => m.InQuantity) ?? 0,
                //TotalOutQuantity = d.ItemLiveStatus.Sum(m => m.OutQuantity) ?? 0,
                //TotalOverallQuantity = d.ItemLiveStatus.Sum(m => m.LiveQuantity) ?? 0,
            }).ToArray();
        }

        public ItemModel[] GetItemListJustNames()
        {
            var repo = _unitOfWork.GetRepository<Item>();

            return repo.GetAll().Select(d => new ItemModel
            {
                Id = d.Id,
                ItemNo = d.ItemNo,
                ItemName = d.ItemName,
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

                if (repo.Any(d => (d.ItemNo == model.ItemNo)
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

                model.TotalInQuantity = dbObj.ItemLiveStatus.Sum(m => m.InQuantity) ?? 0;
                model.TotalOutQuantity = dbObj.ItemLiveStatus.Sum(m => m.OutQuantity) ?? 0;
                model.TotalOverallQuantity = dbObj.ItemLiveStatus.Sum(m => m.LiveQuantity) ?? 0;

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

        public WarehouseModel GetProductWarehouse()
        {
            WarehouseModel data = null;

            var repo = _unitOfWork.GetRepository<Warehouse>();
            var dbObj = repo.Get(d => d.WarehouseType == (int)WarehouseType.ProductWarehouse);
            if (dbObj != null)
            {
                data = new WarehouseModel();
                dbObj.MapTo(data);
            }

            return data;
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
            dataList.ToList().ForEach(d =>
            {
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
            MachineModel[] data = new MachineModel[0];

            var repo = _unitOfWork.GetRepository<Machine>();

            data = repo.GetAll().Select(d => new MachineModel
            {
                Id = d.Id,
                BackColor = d.BackColor,
                CreatedDate = d.CreatedDate,
                CreatedUserId = d.CreatedUserId,
                DeviceIp = d.DeviceIp,
                ForeColor = d.ForeColor,
                IsActive = d.IsActive,
                IsUpToPostureEntry = d.IsUpToPostureEntry,
                IsWatched = d.IsWatched,
                MachineCode = d.MachineCode,
                MachineName = d.MachineName,
                MachineStatus = d.MachineStatus,
                MachineType = d.MachineType,
                PlantId = d.PlantId,
                PostureExpirationCycleCount = d.PostureExpirationCycleCount,
                UpdatedDate = d.UpdatedDate,
                UpdatedUserId = d.UpdatedUserId,
                WatchCycleStartCondition = d.WatchCycleStartCondition,
                WorkingUserId = d.WorkingUserId,
                SignalEndDelay = d.SignalEndDelay,
            }).ToArray();

            return data;
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
            var repoShift = _unitOfWork.GetRepository<Shift>();
            var repoUser = _unitOfWork.GetRepository<User>();
            var repoMoldTest = _unitOfWork.GetRepository<MoldTest>();

            var repoWastage = _unitOfWork.GetRepository<ProductWastage>();
            var repoIncident = _unitOfWork.GetRepository<Incident>();
            var repoPosture = _unitOfWork.GetRepository<ProductionPosture>();

            var shiftList = repoShift.GetAll().ToArray();

            // PRODUCTION BO FOR ACTIVE WORK ORDERS ON MACHINES
            ProductionBO prodBO = new ProductionBO();

            repo.GetAll().ToList().ForEach(d =>
            {
                MachineModel containerObj = new MachineModel();
                d.MapTo(containerObj);

                if (d.WorkingUserId != null)
                {
                    var dbUser = repoUser.Get(m => m.Id == d.WorkingUserId);
                    if (dbUser != null)
                    {
                        containerObj.WorkingUserCode = dbUser.UserCode;
                        containerObj.WorkingUserName = dbUser.UserName;
                    }
                }

                containerObj.ActivePlan = prodBO.GetActiveWorkOrderOnMachine(d.Id);

                var signalData = repoSignal.Filter(m => m.MachineId == d.Id &&
                    dt1 <= m.ShiftBelongsToDate && dt2 >= m.ShiftBelongsToDate);
                var wastageData = repoWastage.Filter(m => m.MachineId == d.Id &&
                    dt1 <= m.ShiftBelongsToDate && dt2 >= m.ShiftBelongsToDate);
                var incidentData = repoIncident.Filter(m => m.MachineId == d.Id &&
                    dt1 <= m.ShiftBelongsToDate && dt2 >= m.ShiftBelongsToDate);
                var postureData = repoPosture.Filter(m => m.MachineId == d.Id &&
                    dt1 <= m.ShiftBelongsToDate && dt2 >= m.ShiftBelongsToDate);

                containerObj.IsInIncident = repoIncident.Any(m => m.MachineId == d.Id
                    && m.IncidentStatus != (int)PostureStatusType.Resolved);
                containerObj.ActiveIncidentText = repoIncident.Filter(m => m.MachineId == d.Id
                    && m.IncidentStatus != (int)PostureStatusType.Resolved)
                    .Select(m => m.IncidentCategory.IncidentCategoryName).FirstOrDefault();

                containerObj.IsInPosture = repoPosture.Any(m => m.MachineId == d.Id
                    && m.PostureStatus != (int)PostureStatusType.Resolved);
                containerObj.ActivePostureText = repoPosture.Filter(m => m.MachineId == d.Id
                    && m.PostureStatus != (int)PostureStatusType.Resolved)
                    .Select(m => m.PostureCategory.PostureCategoryName).FirstOrDefault();

                var machineWastageCount = wastageData.Sum(m => m.Quantity) ?? 0;
                containerObj.MachineStats = new Models.DataTransfer.Summary.MachineStatsModel
                {
                    AvgInflationTime = Convert.ToDecimal(signalData.Average(m => m.Duration) ?? 0),
                    AvgProductionCount = signalData.Count() - Convert.ToInt32(machineWastageCount),
                    WastageCount = machineWastageCount,
                    IncidentCount = incidentData.Count(),
                    PostureCount = postureData.Count(),
                };

                // RESOLVE SHIFT STATS OF THAT MACHINE
                List<ShiftStatsModel> shiftStats = new List<ShiftStatsModel>();
                shiftList = shiftList.OrderBy(m => m.StartTime).ToArray();
                foreach (var shift in shiftList)
                {
                    // CALCULATE TARGET COUNT
                    DateTime startTime = DateTime.Now.Date.Add(shift.StartTime.Value);
                    DateTime endTime = DateTime.Now.Date.Add(shift.EndTime.Value);

                    if (shift.StartTime > shift.EndTime)
                        endTime = DateTime.Now.Date.AddDays(1).Add(shift.EndTime.Value);

                    var totalBreakTimeSeconds = 75 * 60;
                    var totalShiftTime = (endTime - startTime).TotalSeconds;
                    var netShiftTime = totalShiftTime - totalBreakTimeSeconds;

                    // GET ACTIVE PLANS CYCLE TIME
                    string lastProductName = "";
                    decimal avgCycleTime = 0;
                    int targetCount = 0;

                    var lastSignal = repoSignal.Filter(m => m.MachineId == d.Id
                        && m.WorkOrderDetailId != null
                        && m.ShiftId == shift.Id)
                        .OrderByDescending(m => m.Id).FirstOrDefault();
                    if (lastSignal != null)
                    {
                        lastProductName = lastSignal.WorkOrderDetail.Item != null ?
                            lastSignal.WorkOrderDetail.Item.ItemName : lastSignal.WorkOrderDetail.TrialProductName;
                        if (lastSignal.WorkOrderDetail.Item != null)
                        {
                            var dbMoldTest = repoMoldTest.Get(m => m.ProductCode == lastSignal.WorkOrderDetail.Item.ItemNo);
                            if (dbMoldTest != null)
                                avgCycleTime = dbMoldTest.TotalTimeSeconds ?? 0;
                        }
                    }

                    // IF NO CYCLE TIME FOUND THEN CALCULATE OVER HISTORY
                    if (avgCycleTime <= 0)
                    {
                        avgCycleTime = Convert.ToDecimal(repoSignal.Filter(m => m.MachineId == d.Id && m.ShiftId == shift.Id)
                            .Average(m => m.Duration) ?? 0);
                    }

                    if (avgCycleTime > 0)
                    {
                        try
                        {
                            targetCount = Convert.ToInt32(Convert.ToDecimal(netShiftTime) / avgCycleTime);
                        }
                        catch (Exception)
                        {

                        }
                    }

                    var shiftWastageCount = wastageData.Where(m => m.ShiftId == shift.Id).Sum(m => m.Quantity) ?? 0;

                    shiftStats.Add(new ShiftStatsModel
                    {
                        ShiftId = shift.Id,
                        ChiefUserName = shift.User != null ? shift.User.UserName : "",
                        ShiftCode = shift.ShiftCode,
                        AvgInflationTime = Convert.ToDecimal(signalData.Where(m => m.ShiftId == shift.Id).Average(m => m.Duration)),
                        AvgProductionCount = signalData.Where(m => m.ShiftId == shift.Id).Count() -
                            Convert.ToInt32(shiftWastageCount),
                        WastageCount = shiftWastageCount,
                        LastProductName = lastProductName,
                        TargetCount = targetCount - Convert.ToInt32(shiftWastageCount),
                    });
                }

                containerObj.MachineStats.ShiftStats = shiftStats.ToArray();

                data.Add(containerObj);
            });

            prodBO.Dispose();

            return data.ToArray();
        }

        public MachineModel[] GetMachineStats(int machineId, string startDate, string endDate)
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
            var repoShift = _unitOfWork.GetRepository<Shift>();
            var repoUser = _unitOfWork.GetRepository<User>();

            var repoWastage = _unitOfWork.GetRepository<ProductWastage>();
            var repoIncident = _unitOfWork.GetRepository<Incident>();
            var repoPosture = _unitOfWork.GetRepository<ProductionPosture>();

            var shiftList = repoShift.GetAll().ToArray();

            // PRODUCTION BO FOR ACTIVE WORK ORDERS ON MACHINES
            ProductionBO prodBO = new ProductionBO();

            repo.Filter(d => d.Id == machineId).ToList().ForEach(d =>
            {
                MachineModel containerObj = new MachineModel();
                d.MapTo(containerObj);

                if (d.WorkingUserId != null)
                {
                    var dbUser = repoUser.Get(m => m.Id == d.WorkingUserId);
                    if (dbUser != null)
                    {
                        containerObj.WorkingUserCode = dbUser.UserCode;
                        containerObj.WorkingUserName = dbUser.UserName;
                    }
                }

                containerObj.ActivePlan = prodBO.GetActiveWorkOrderOnMachine(d.Id);

                var signalData = repoSignal.Filter(m => m.MachineId == d.Id &&
                    dt1 <= m.ShiftBelongsToDate && dt2 >= m.ShiftBelongsToDate);
                var wastageData = repoWastage.Filter(m => m.MachineId == d.Id &&
                    dt1 <= m.ShiftBelongsToDate && dt2 >= m.ShiftBelongsToDate);
                var incidentData = repoIncident.Filter(m => m.MachineId == d.Id &&
                    dt1 <= m.ShiftBelongsToDate && dt2 >= m.ShiftBelongsToDate);
                var postureData = repoPosture.Filter(m => m.MachineId == d.Id &&
                    dt1 <= m.ShiftBelongsToDate && dt2 >= m.ShiftBelongsToDate);

                containerObj.IsInIncident = repoIncident.Any(m => m.MachineId == d.Id
                    && m.IncidentStatus != (int)PostureStatusType.Resolved);
                containerObj.ActiveIncidentText = repoIncident.Filter(m => m.MachineId == d.Id
                    && m.IncidentStatus != (int)PostureStatusType.Resolved)
                    .Select(m => m.IncidentCategory.IncidentCategoryName).FirstOrDefault();

                containerObj.IsInPosture = repoPosture.Any(m => m.MachineId == d.Id
                    && m.PostureStatus != (int)PostureStatusType.Resolved);
                containerObj.ActivePostureText = repoPosture.Filter(m => m.MachineId == d.Id
                    && m.PostureStatus != (int)PostureStatusType.Resolved)
                    .Select(m => m.PostureCategory.PostureCategoryName).FirstOrDefault();

                var machineWastageCount = wastageData.Sum(m => m.Quantity) ?? 0;
                containerObj.MachineStats = new Models.DataTransfer.Summary.MachineStatsModel
                {
                    AvgInflationTime = Convert.ToDecimal(signalData.Average(m => m.Duration) ?? 0),
                    AvgProductionCount = signalData.Count() - Convert.ToInt32(machineWastageCount),
                    WastageCount = machineWastageCount,
                    IncidentCount = incidentData.Count(),
                    PostureCount = postureData.Count(),
                };

                // RESOLVE SHIFT STATS OF THAT MACHINE
                List<ShiftStatsModel> shiftStats = new List<ShiftStatsModel>();
                foreach (var shift in shiftList)
                {
                    var shiftWastageCount = wastageData.Where(m => m.ShiftId == shift.Id).Sum(m => m.Quantity) ?? 0;
                    shiftStats.Add(new ShiftStatsModel
                    {
                        ShiftId = shift.Id,
                        ShiftCode = shift.ShiftCode,
                        AvgInflationTime = Convert.ToDecimal(signalData.Where(m => m.ShiftId == shift.Id).Average(m => m.Duration)),
                        AvgProductionCount = signalData.Where(m => m.ShiftId == shift.Id).Count()
                            - Convert.ToInt32(shiftWastageCount),
                        WastageCount = shiftWastageCount,
                    });
                }

                containerObj.MachineStats.ShiftStats = shiftStats.ToArray();

                data.Add(containerObj);
            });

            prodBO.Dispose();

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
                var repoInstructions = _unitOfWork.GetRepository<MachineMaintenanceInstruction>();
                var repoEquipments = _unitOfWork.GetRepository<Equipment>();

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

                #region SAVE INSTRUCTIONS
                if (model.Instructions == null)
                    model.Instructions = new MachineMaintenanceInstructionModel[0];

                var toBeRemovedInstructions = dbObj.MachineMaintenanceInstruction
                    .Where(d => !model.Instructions.Where(m => m.NewDetail == false)
                        .Select(m => m.Id).ToArray().Contains(d.Id)
                    ).ToArray();
                foreach (var item in toBeRemovedInstructions)
                {
                    repoInstructions.Delete(item);
                }

                foreach (var item in model.Instructions)
                {
                    if (item.NewDetail == true)
                    {
                        var dbItemAu = new MachineMaintenanceInstruction();
                        item.MapTo(dbItemAu);
                        dbItemAu.Machine = dbObj;
                        repoInstructions.Add(dbItemAu);
                    }
                    else if (!toBeRemovedInstructions.Any(d => d.Id == item.Id))
                    {
                        var dbItemAu = repoInstructions.GetById(item.Id);
                        item.MapTo(dbItemAu);
                        dbItemAu.Machine = dbObj;
                    }
                }
                #endregion

                #region SAVE EQUIPMENTS
                if (model.Equipments == null)
                    model.Equipments = new EquipmentModel[0];

                var toBeRemovedEquipments = dbObj.Equipment
                    .Where(d => !model.Equipments.Where(m => m.NewDetail == false)
                        .Select(m => m.Id).ToArray().Contains(d.Id)
                    ).ToArray();
                foreach (var item in toBeRemovedEquipments)
                {
                    repoEquipments.Delete(item);
                }

                foreach (var item in model.Equipments)
                {
                    if (item.NewDetail == true)
                    {
                        var dbItemAu = new Equipment();
                        item.MapTo(dbItemAu);
                        dbItemAu.Machine = dbObj;
                        repoEquipments.Add(dbItemAu);
                    }
                    else if (!toBeRemovedEquipments.Any(d => d.Id == item.Id))
                    {
                        var dbItemAu = repoEquipments.GetById(item.Id);
                        item.MapTo(dbItemAu);
                        dbItemAu.Machine = dbObj;
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
            MachineModel model = new MachineModel { Instructions = new MachineMaintenanceInstructionModel[0] };

            var repo = _unitOfWork.GetRepository<Machine>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.Instructions = dbObj.MachineMaintenanceInstruction.
                    Select(d => new MachineMaintenanceInstructionModel
                    {
                        Id = d.Id,
                        LineNumber = d.LineNumber,
                        MachineCode = d.Machine != null ? d.Machine.MachineCode : "",
                        MachineName = d.Machine != null ? d.Machine.MachineName : "",
                        MachineId = d.MachineId,
                        PeriodType = d.PeriodType,
                        Responsible = d.Responsible,
                        ToDoList = d.ToDoList,
                        UnitName = d.UnitName,
                    }).OrderBy(d => d.Id).ToArray();
                model.Equipments = dbObj.Equipment
                    .Select(d => new EquipmentModel
                    {
                        Id = d.Id,
                        EquipmentCode = d.EquipmentCode,
                        EquipmentName = d.EquipmentName,
                        Location = d.Location,
                        MachineCode = d.Machine != null ? d.Machine.MachineCode : "",
                        MachineId = d.MachineId,
                        MachineName = d.Machine != null ? d.Machine.MachineName : "",
                        Manufacturer = d.Manufacturer,
                        ModelNo = d.ModelNo,
                        NewDetail = false,
                        PlantId = d.PlantId,
                        ResponsibleUserId = d.ResponsibleUserId,
                        SerialNo = d.SerialNo,
                        UserCode = d.User != null ? d.User.UserCode : "",
                        UserName = d.User != null ? d.User.UserName : "",
                    }).ToArray();
            }

            return model;
        }

        public MachineModel GetMachine(string machineCode)
        {
            MachineModel model = new MachineModel { Instructions = new MachineMaintenanceInstructionModel[0] };

            var repo = _unitOfWork.GetRepository<Machine>();
            var dbObj = repo.Get(d => d.MachineCode == machineCode);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.Instructions = dbObj.MachineMaintenanceInstruction.
                    Select(d => new MachineMaintenanceInstructionModel
                    {
                        Id = d.Id,
                        LineNumber = d.LineNumber,
                        MachineCode = d.Machine != null ? d.Machine.MachineCode : "",
                        MachineName = d.Machine != null ? d.Machine.MachineName : "",
                        MachineId = d.MachineId,
                        PeriodType = d.PeriodType,
                        Responsible = d.Responsible,
                        ToDoList = d.ToDoList,
                        UnitName = d.UnitName,
                    }).OrderBy(d => d.Id).ToArray();
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
            MoldModel[] data = new MoldModel[0];

            var repo = _unitOfWork.GetRepository<Mold>();

            data = repo.GetAll().ToList().Select(d => new MoldModel
            {
                Id = d.Id,
                MoldCode = d.MoldCode,
                MoldName = d.MoldName,
                FirmCode = d.Firm != null ? d.Firm.FirmCode : "",
                FirmName = d.Firm != null ? d.Firm.FirmName : "",
                OwnedDateStr = d.OwnedDate != null ?
                    string.Format("{0:dd.MM.yyyy}", d.OwnedDate) : "",
                LifeTimeTicks = d.LifeTimeTicks,
                CurrentTicks = d.CurrentTicks,
                IsActive = d.IsActive,
            }).ToArray();

            return data;
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

                if (!string.IsNullOrEmpty(model.CreatedDateStr))
                    model.CreatedDate = DateTime.ParseExact(model.CreatedDateStr, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));

                if (!string.IsNullOrEmpty(model.OwnedDateStr))
                    model.OwnedDate = DateTime.ParseExact(model.OwnedDateStr, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));

                var repo = _unitOfWork.GetRepository<Mold>();
                var repoMoldProducts = _unitOfWork.GetRepository<MoldProduct>();
                var repoItem = _unitOfWork.GetRepository<Item>();

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
                var owDate = dbObj.OwnedDate;

                model.MapTo(dbObj);

                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = crDate;
                if (dbObj.OwnedDate == null)
                    dbObj.OwnedDate = owDate;

                dbObj.UpdatedDate = DateTime.Now;

                #region SAVE PRODUCTS
                if (model.Products == null)
                    model.Products = new MoldProductModel[0];

                var toBeRemovedAuthors = dbObj.MoldProduct
                    .Where(d => !model.Products.Where(m => m.NewDetail == false)
                        .Select(m => m.Id).ToArray().Contains(d.Id)
                    ).ToArray();
                foreach (var item in toBeRemovedAuthors)
                {
                    repoMoldProducts.Delete(item);
                }

                foreach (var item in model.Products)
                {
                    if (item.NewDetail == true)
                    {
                        var dbItemAu = new MoldProduct();
                        item.MapTo(dbItemAu);
                        dbItemAu.Mold = dbObj;
                        repoMoldProducts.Add(dbItemAu);
                    }
                    else if (!toBeRemovedAuthors.Any(d => d.Id == item.Id))
                    {
                        var dbItemAu = repoMoldProducts.GetById(item.Id);
                        item.MapTo(dbItemAu);
                        dbItemAu.Mold = dbObj;
                    }
                }
                #endregion

                #region MOLD ITEM RECORD CHECK
                var dbMoldItem = repoItem.Get(d => d.ItemNo == dbObj.MoldCode && dbObj.PlantId == d.PlantId);
                if (dbMoldItem == null)
                {
                    dbMoldItem = new Item
                    {
                        PlantId = dbObj.PlantId,
                        ItemType = (int)ItemType.Commercial,
                        CreatedDate = DateTime.Now,
                    };
                    repoItem.Add(dbMoldItem);
                }

                dbMoldItem.ItemNo = dbObj.MoldCode;
                dbMoldItem.ItemName = dbObj.MoldName;
                dbMoldItem.SupplierFirmId = dbObj.FirmId;
                dbObj.ItemMold = dbMoldItem;
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

        public BusinessResult DeleteMold(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<Mold>();
                var repoProducts = _unitOfWork.GetRepository<MoldProduct>();

                var dbObj = repo.Get(d => d.Id == id);
                if (dbObj.MoldProduct.Any())
                {
                    var toBeRemoved = dbObj.MoldProduct.ToArray();
                    foreach (var item in toBeRemoved)
                    {
                        repoProducts.Delete(item);
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

        public MoldModel GetMold(int id)
        {
            MoldModel model = new MoldModel { Products = new MoldProductModel[0] };

            var repo = _unitOfWork.GetRepository<Mold>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.MoldStatusText = ((MoldStatus)(model.MoldStatus ?? 1)).ToCaption();
                model.CreatedDateStr = model.CreatedDate != null ?
                    string.Format("{0:dd.MM.yyyy}", model.CreatedDate) : "";
                model.OwnedDateStr = model.OwnedDate != null ?
                    string.Format("{0:dd.MM.yyyy}", model.OwnedDate) : "";
                model.FirmName = dbObj.Firm != null ? dbObj.Firm.FirmName : "";
                model.FirmCode = dbObj.Firm != null ? dbObj.Firm.FirmCode : "";
                model.Products = dbObj.MoldProduct.Select(m => new MoldProductModel
                {
                    Id = m.Id,
                    LineNumber = m.LineNumber,
                    MoldId = m.MoldId,
                    ProductId = m.ProductId,
                    ProductCode = m.Item != null ? m.Item.ItemNo : "",
                    ProductName = m.Item != null ? m.Item.ItemName : "",
                }).ToArray();
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

        #region SHIFT BUSINESS
        public ShiftModel[] GetShiftList()
        {
            List<ShiftModel> data = new List<ShiftModel>();

            var repo = _unitOfWork.GetRepository<Shift>();

            repo.GetAll().ToList().ForEach(d =>
            {
                ShiftModel containerObj = new ShiftModel();
                d.MapTo(containerObj);
                containerObj.StartTimeStr = d.StartTime != null ?
                    string.Format("{0:hh\\:mm}", d.StartTime) : "";
                containerObj.EndTimeStr = d.EndTime != null ?
                    string.Format("{0:hh\\:mm}", d.EndTime) : "";
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateShift(ShiftModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.ShiftCode))
                    throw new Exception("Vardiya kodu girilmelidir.");
                if (string.IsNullOrEmpty(model.ShiftName))
                    throw new Exception("Vardiya adı girilmelidir.");

                var repo = _unitOfWork.GetRepository<Shift>();

                if (repo.Any(d => (d.ShiftCode == model.ShiftCode)
                    && d.Id != model.Id))
                    throw new Exception("Aynı koda sahip başka bir vardiya mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new Shift();
                    dbObj.IsActive = true;
                    repo.Add(dbObj);
                }

                model.MapTo(dbObj);

                dbObj.IsActive = true;

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

        public BusinessResult DeleteShift(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<Shift>();

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

        public ShiftModel GetShift(int id)
        {
            ShiftModel model = new ShiftModel { };

            var repo = _unitOfWork.GetRepository<Shift>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.StartTimeStr = model.StartTime != null ?
                    string.Format("{0:hh\\:mm}", model.StartTime) : "";
                model.EndTimeStr = model.EndTime != null ?
                    string.Format("{0:hh\\:mm}", model.EndTime) : "";
                model.ChiefCode = dbObj.User != null ? dbObj.User.UserCode : "";
                model.ChiefName = dbObj.User != null ? dbObj.User.UserName : "";
            }

            return model;
        }

        #endregion

        #region SECTION SETTINGS BUSINESS
        public SectionSettingModel[] GetSectionSettingList()
        {
            List<SectionSettingModel> data = new List<SectionSettingModel>();

            var repo = _unitOfWork.GetRepository<SectionSetting>();

            repo.GetAll().ToList().ForEach(d =>
            {
                SectionSettingModel containerObj = new SectionSettingModel();
                d.MapTo(containerObj);
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateSectionSetting(SectionSettingModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.SectionGroupCode))
                    throw new Exception("Bölüm bilgisi girilmelidir.");

                var repo = _unitOfWork.GetRepository<SectionSetting>();

                if (repo.Any(d => (d.SectionGroupCode == model.SectionGroupCode)
                    && d.Id != model.Id))
                    throw new Exception("Aynı koda sahip başka bir bölüm ayarı mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new SectionSetting();
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

        public BusinessResult DeleteSectionSetting(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<SectionSetting>();

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

        public SectionSettingModel GetSectionSetting(int id)
        {
            SectionSettingModel model = new SectionSettingModel { };

            var repo = _unitOfWork.GetRepository<SectionSetting>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }

        public SectionSettingModel GetSectionSetting(string sectionCode)
        {
            SectionSettingModel model = new SectionSettingModel { };

            var repo = _unitOfWork.GetRepository<SectionSetting>();
            var dbObj = repo.Get(d => d.SectionGroupCode == sectionCode);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }
        #endregion

        #region REPORT TEMPLATE BUSINESS
        public ReportTemplateModel[] GetReportTemplateList()
        {
            List<ReportTemplateModel> data = new List<ReportTemplateModel>();

            var repo = _unitOfWork.GetRepository<ReportTemplate>();

            repo.GetAll().ToList().ForEach(d =>
            {
                ReportTemplateModel containerObj = new ReportTemplateModel();
                d.MapTo(containerObj);
                containerObj.ReportTypeStr = ((ReportType)d.ReportType.Value).ToCaption();
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateReportTemplate(ReportTemplateModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.ReportCode))
                    throw new Exception("Rapor şablon kodu girilmelidir.");
                if (string.IsNullOrEmpty(model.ReportName))
                    throw new Exception("Rapor şablon adı girilmelidir.");

                var repo = _unitOfWork.GetRepository<ReportTemplate>();

                if (repo.Any(d => (d.ReportCode == model.ReportCode)
                    && d.Id != model.Id))
                    throw new Exception("Aynı koda sahip başka bir rapor mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new ReportTemplate();
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

        public BusinessResult DeleteReportTemplate(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ReportTemplate>();

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

        public ReportTemplateModel GetReportTemplate(int id)
        {
            ReportTemplateModel model = new ReportTemplateModel { };

            var repo = _unitOfWork.GetRepository<ReportTemplate>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.ReportTypeStr = ((ReportType)dbObj.ReportType.Value).ToCaption();
            }

            return model;
        }
        #endregion

        #region EQUIPMENT BUSINESS
        public EquipmentModel[] GetEquipmentList()
        {
            List<EquipmentModel> data = new List<EquipmentModel>();

            var repo = _unitOfWork.GetRepository<Equipment>();

            repo.GetAll().ToList().ForEach(d =>
            {
                EquipmentModel containerObj = new EquipmentModel();
                d.MapTo(containerObj);
                containerObj.MachineCode = d.Machine != null ? d.Machine.MachineCode : "";
                containerObj.MachineName = d.Machine != null ? d.Machine.MachineName : "";
                containerObj.UserCode = d.User != null ? d.User.UserCode : "";
                containerObj.UserName = d.User != null ? d.User.UserName : "";

                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateEquipment(EquipmentModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.EquipmentCode))
                    throw new Exception("Ekipman kodu girilmelidir.");
                if (string.IsNullOrEmpty(model.EquipmentName))
                    throw new Exception("Ekipman adı girilmelidir.");

                var repo = _unitOfWork.GetRepository<Equipment>();

                if (repo.Any(d => (d.EquipmentCode == model.EquipmentCode)
                    && d.Id != model.Id))
                    throw new Exception("Aynı koda sahip başka bir ekipman mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new Equipment();
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

        public BusinessResult DeleteEquipment(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<Equipment>();

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

        public EquipmentModel GetEquipment(int id)
        {
            EquipmentModel model = new EquipmentModel { };

            var repo = _unitOfWork.GetRepository<Equipment>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.MachineCode = dbObj.Machine != null ? dbObj.Machine.MachineCode : "";
                model.MachineName = dbObj.Machine != null ? dbObj.Machine.MachineName : "";
                model.UserCode = dbObj.User != null ? dbObj.User.UserCode : "";
                model.UserName = dbObj.User != null ? dbObj.User.UserName : "";
            }

            return model;
        }

        #endregion

        #region EQUIPMENT CATEGORY BUSINESS
        public EquipmentCategoryModel[] GetEquipmentCategoryList(bool isCritical = false)
        {
            List<EquipmentCategoryModel> data = new List<EquipmentCategoryModel>();

            var repo = _unitOfWork.GetRepository<EquipmentCategory>();

            repo.Filter(d => isCritical == false || (isCritical == true && d.IsCritical == true)).ToList().ForEach(d =>
            {
                EquipmentCategoryModel containerObj = new EquipmentCategoryModel();
                d.MapTo(containerObj);
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateEquipmentCategory(EquipmentCategoryModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.EquipmentCategoryCode))
                    throw new Exception("Ekipman kategori kodu girilmelidir.");

                var repo = _unitOfWork.GetRepository<EquipmentCategory>();

                if (repo.Any(d => (d.EquipmentCategoryCode == model.EquipmentCategoryCode)
                    && d.Id != model.Id))
                    throw new Exception("Aynı koda sahip başka bir ekipman kategorisi mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new EquipmentCategory();
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

        public BusinessResult DeleteEquipmentCategory(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<EquipmentCategory>();

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

        public EquipmentCategoryModel GetEquipmentCategory(int id)
        {
            EquipmentCategoryModel model = new EquipmentCategoryModel { };

            var repo = _unitOfWork.GetRepository<EquipmentCategory>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }

        #endregion

        #region MACHINE GROUP BUSINESS
        public MachineGroupModel[] GetMachineGroupList()
        {
            var repo = _unitOfWork.GetRepository<MachineGroup>();

            return repo.GetAll()
                .Select(d => new MachineGroupModel
                {
                    Id = d.Id,
                    LayoutObjectTypeId = d.LayoutObjectTypeId,
                    MachineGroupCode = d.MachineGroupCode,
                    MachineGroupName = d.MachineGroupName,
                    PlantId = d.PlantId,
                    IsProduction = d.IsProduction,
                }).ToArray();
        }
        public BusinessResult SaveOrUpdateMachineGroup(MachineGroupModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.MachineGroupCode))
                    throw new Exception("Grup kodu girilmelidir.");
                if (string.IsNullOrEmpty(model.MachineGroupName))
                    throw new Exception("Grup adı girilmelidir.");

                var repo = _unitOfWork.GetRepository<MachineGroup>();

                if (repo.Any(d => (d.MachineGroupCode == model.MachineGroupCode) && d.Id != model.Id))
                    throw new Exception("Aynı koda sahip başka bir makine grubu mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new MachineGroup();
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

        public BusinessResult DeleteMachineGroup(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<MachineGroup>();

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

        public MachineGroupModel GetMachineGroup(int id)
        {
            MachineGroupModel model = new MachineGroupModel { };

            var repo = _unitOfWork.GetRepository<MachineGroup>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }
        #endregion


        #region PROCESS GROUP BUSINESS
        public ProcessGroupModel[] GetProcessGroupList()
        {
            List<ProcessGroupModel> data = new List<ProcessGroupModel>();

            var repo = _unitOfWork.GetRepository<ProcessGroup>();

            repo.GetAll().ToList().ForEach(d =>
            {
                ProcessGroupModel containerObj = new ProcessGroupModel();
                d.MapTo(containerObj);
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateProcessGroup(ProcessGroupModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.ProcessGroupCode))
                    throw new Exception("Grup kodu girilmelidir.");
                if (string.IsNullOrEmpty(model.ProcessGroupName))
                    throw new Exception("Grup adı girilmelidir.");

                var repo = _unitOfWork.GetRepository<ProcessGroup>();

                if (repo.Any(d => (d.ProcessGroupCode == model.ProcessGroupCode)
                    && d.Id != model.Id))
                    throw new Exception("Aynı koda sahip başka bir grup mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new ProcessGroup();
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

        public BusinessResult DeleteProcessGroup(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ProcessGroup>();

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

        public ProcessGroupModel GetProcessGroup(int id)
        {
            ProcessGroupModel model = new ProcessGroupModel { };

            var repo = _unitOfWork.GetRepository<ProcessGroup>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }
        #endregion

        #region COUNTRY BUSINESS
        public CountryModel[] GetCountryList()
        {
            List<CountryModel> data = new List<CountryModel>();

            var repo = _unitOfWork.GetRepository<Country>();

            repo.GetAll().ToList().ForEach(d =>
            {
                CountryModel containerObj = new CountryModel();
                d.MapTo(containerObj);
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateCountry(CountryModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.CountryName))
                    throw new Exception("Ülke adı girilmelidir.");


                var repo = _unitOfWork.GetRepository<Country>();

                if (repo.Any(d => (d.CountryName == model.CountryName)
                    && d.Id != model.Id))
                    throw new Exception("Aynı isme sahip başka bir ülke mevcuttur. Lütfen farklı bir isim giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new Country();
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

        public BusinessResult DeleteCountry(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<Country>();

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

        public CountryModel GetCountry(int id)
        {
            CountryModel model = new CountryModel { };

            var repo = _unitOfWork.GetRepository<Country>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }
        #endregion

        #region CITY BUSINESS
        public CityModel[] GetCityList()
        {
            List<CityModel> data = new List<CityModel>();

            var repo = _unitOfWork.GetRepository<City>();

            repo.GetAll().ToList().ForEach(d =>
            {
                CityModel containerObj = new CityModel();
                containerObj.CountryName = d.Country != null ? d.Country.CountryName : "";
                d.MapTo(containerObj);
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateCity(CityModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.CityName))
                    throw new Exception("Şehir adı girilmelidir.");

                if (string.IsNullOrEmpty(Convert.ToString( model.CountryId )))
                    throw new Exception("Ülke Seçilmelidir");

                var repo = _unitOfWork.GetRepository<City>();

                if (repo.Any(d => (d.CityName == model.CityName)
                    && d.Id != model.Id))
                    throw new Exception("Aynı isme sahip başka bir şehir mevcuttur. Lütfen farklı bir isim giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new City();
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

        public BusinessResult DeleteCity(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<City>();

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

        public CityModel GetCity(int id)
        {
            CityModel model = new CityModel { };

            var repo = _unitOfWork.GetRepository<City>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }
        #endregion

        #region DISTRICT BUSINESS
        public DistrictModel[] GetDistrictList()
        {
            List<DistrictModel> data = new List<DistrictModel>();

            var repo = _unitOfWork.GetRepository<District>();

            repo.GetAll().ToList().ForEach(d =>
            {
                DistrictModel containerObj = new DistrictModel();
                containerObj.CityName = d.City != null ? d.City.CityName : "";
                d.MapTo(containerObj);
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateDistrict(DistrictModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.DistrictName))
                    throw new Exception("İlçe adı girilmelidir.");

                if (string.IsNullOrEmpty(Convert.ToString(model.CityId) ))
                    throw new Exception("Şehir Seçilmelidir");

                var repo = _unitOfWork.GetRepository<District>();

                if (repo.Any(d => (d.DistrictName == model.DistrictName)
                    && d.Id != model.Id))
                    throw new Exception("Aynı isme sahip başka bir ilçe mevcuttur. Lütfen farklı bir isim giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new District();
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

        public BusinessResult DeleteDistrict(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<District>();

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

        public DistrictModel GetDistrict(int id)
        {
            DistrictModel model = new DistrictModel { };

            var repo = _unitOfWork.GetRepository<District>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }

        #endregion

        #region WORK ORDER CATEGORY BUSINESS
        public WorkOrderCategoryModel[] GetWorkOrderCategoryList()
        {
            List<WorkOrderCategoryModel> data = new List<WorkOrderCategoryModel>();

            var repo = _unitOfWork.GetRepository<WorkOrderCategory>();

            repo.GetAll().ToList().ForEach(d =>
            {
                WorkOrderCategoryModel containerObj = new WorkOrderCategoryModel();
                d.MapTo(containerObj);
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateWorkOrderCategory(WorkOrderCategoryModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.WorkOrderCategoryCode))
                    throw new Exception("Kategori kodu girilmelidir.");
                if (string.IsNullOrEmpty(model.WorkOrderCategoryName))
                    throw new Exception("Kategori adı girilmelidir.");

                var repo = _unitOfWork.GetRepository<WorkOrderCategory>();

                if (repo.Any(d => (d.WorkOrderCategoryCode == model.WorkOrderCategoryCode)
                    && d.Id != model.Id))
                    throw new Exception("Aynı koda sahip başka bir kategori mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new WorkOrderCategory();
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

        public BusinessResult DeleteWorkOrderCategory(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<WorkOrderCategory>();

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

        public WorkOrderCategoryModel GetWorkOrderCategory(int id)
        {
            WorkOrderCategoryModel model = new WorkOrderCategoryModel { };

            var repo = _unitOfWork.GetRepository<WorkOrderCategory>();
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
