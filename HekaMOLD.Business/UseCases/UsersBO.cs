﻿using Heka.DataAccess.Context;
using Heka.DataAccess.Context.Models;
using HekaMOLD.Business.Base;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Authentication;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Authentication;
using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Logistics;
using HekaMOLD.Business.Models.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.UseCases
{
    public class UsersBO : IBusinessObject
    {
        #region AUTHENTICATION
        public AuthenticationResult Authenticate(string userCode, string password)
        {
            AuthenticationResult result = new AuthenticationResult();

            try
            {
                var repo = _unitOfWork.GetRepository<User>();
                var dbUser = repo.Get(d => d.Login == userCode.Trim());
                if (dbUser == null)
                    throw new Exception("Bu koda sahip bir kullanıcı bulunamadı.");
                if (!string.Equals(dbUser.Password, password, StringComparison.Ordinal))
                    throw new Exception("Hatalı parola girildi.");

                result.UserData = GetUser(dbUser.Id);
                if (result.UserData.ProfileImage != null)
                {
                    result.UserData.ProfileImageBase64 = "data:image/png;base64, " + Convert.ToBase64String(result.UserData.ProfileImage);
                }
                result.UserData.ProfileImage = null;

                result.UserData.IsProdTerminal = dbUser.UserRole.UserAuth
                    .Any(d => d.UserAuthType.AuthTypeCode == "MobileProductionUser"
                    && d.IsGranted == true);
                result.UserData.IsProdChief = dbUser.UserRole.UserAuth
                    .Any(d => d.UserAuthType.AuthTypeCode == "IsProductionChief"
                    && d.IsGranted == true);
                result.UserData.IsMechanicTerminal = dbUser.UserRole.UserAuth
                    .Any(d => d.UserAuthType.AuthTypeCode == "MobileMechanicUser"
                    && d.IsGranted == true);
                result.UserData.IsWarehouseTerminal = dbUser.UserRole.UserAuth
                    .Any(d => d.UserAuthType.AuthTypeCode == "MobileWarehouseUser"
                    && d.IsGranted == true);
                result.UserData.Auths = dbUser.UserRole.UserAuth
                    .Select(d => new UserAuthModel
                    {
                        Id = d.Id,
                        AuthTypeId = d.AuthTypeId,
                        IsGranted = d.IsGranted,
                        UserId = d.UserId,
                        UserRoleId = d.UserRoleId,
                        AuthTypeCode = d.UserAuthType != null ? d.UserAuthType.AuthTypeCode : "",
                    }).ToArray();

                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public AuthenticationResult IsAuthenticated(string authType, int userId)
        {
            AuthenticationResult result = new AuthenticationResult { Result = false };

            try
            {
                var repoUser = _unitOfWork.GetRepository<User>();
                var repo = _unitOfWork.GetRepository<UserAuth>();

                var dbUser = repoUser.Get(d => d.Id == userId);
                if (dbUser == null)
                    throw new Exception("Oturumunuz kapanmış. Lütfen tekrar giriş yapınız.");

                result.Result = repo.Any(d => (d.UserRoleId == dbUser.UserRoleId || d.UserId == userId) && d.UserAuthType.AuthTypeCode == authType
                      && d.IsGranted == true);

                if (!result.Result)
                    result.ErrorMessage = "Bu bölümde yetkiniz bulunmamaktadır.";
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }
        #endregion

        #region PRODUCTION USERS MANAGEMENT
        public BusinessResult IsMachineAvailableForLoggedIn(int userId, int machineId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<UserWorkOrderHistory>();
                var repoUser = _unitOfWork.GetRepository<User>();

                var lastActive = repo.Filter(d => d.MachineId == machineId && d.EndDate == null)
                    .OrderByDescending(d => d.Id).FirstOrDefault();
                
                if (lastActive == null)
                    result.Result = true;
                else
                {
                    if (lastActive.UserId == userId)
                        result.Result = true;
                    else
                    {
                        if ((DateTime.Now - lastActive.StartDate.Value).TotalHours >= 23)
                        {
                            lastActive.EndDate = DateTime.Now;
                            _unitOfWork.SaveChanges();

                            result.Result = true;
                        }
                        else
                        {
                            var dbUser = repoUser.Get(d => d.Id == lastActive.UserId);
                            throw new Exception("Bu makinede şuan " + dbUser.UserName + " çalışmaktadır. "
                                + "Personel çıkış yapmadan siz bu makine için giriş yapamazsınız.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult LogUserIntoMachine(int userId, int machineId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<UserWorkOrderHistory>();
                var repoMachine = _unitOfWork.GetRepository<Machine>();

                var lastActive = repo.Filter(d => d.MachineId == machineId && d.EndDate == null)
                    .OrderByDescending(d => d.Id).FirstOrDefault();
                var dbMachine = repoMachine.Get(d => d.Id == machineId);
                var exActiveMachines = repoMachine.Filter(d => d.Id != machineId && d.WorkingUserId == userId).ToArray();

                var exActives = repo.Filter(d => d.MachineId == machineId && d.EndDate == null).ToArray();
                foreach (var item in exActives.Where(m => lastActive == null || m.Id != lastActive.Id))
                {
                    item.EndDate = DateTime.Now;
                }

                foreach (var item in exActiveMachines)
                {
                    item.WorkingUserId = null;
                }

                _unitOfWork.SaveChanges();

                using (ProductionBO bObj = new ProductionBO())
                {
                    bObj.UpdateUserHistory(machineId, userId);
                }

                result.Result = true;
                result.Code = dbMachine.MachineName;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult LogOffUserFromMachine(int userId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<UserWorkOrderHistory>();
                var repoMachines = _unitOfWork.GetRepository<Machine>();

                var userActives = repo.Filter(d => d.UserId == userId && d.EndDate == null).ToArray();
                foreach (var item in userActives)
                {
                    item.EndDate = DateTime.Now;
                }

                var machineActives = repoMachines.Filter(d => d.WorkingUserId == userId).ToArray();
                foreach (var item in machineActives)
                {
                    item.WorkingUserId = null;
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

        #region NOTIFICATIONS
        public NotificationModel[] GetAwaitingNotifications(int userId, int topRecords=0)
        {
            List<NotificationModel> data = new List<NotificationModel>();

            var repo = _unitOfWork.GetRepository<Notification>();

            IQueryable<Notification> repoQuery = repo.Filter(d => d.UserId == userId && (d.IsProcessed ?? false) == false)
                .OrderByDescending(d => d.CreatedDate);

            if (topRecords > 0)
                repoQuery = repoQuery.Take(topRecords);

            repoQuery.ToList().ForEach(d =>
            {
                NotificationModel containerObj = new NotificationModel();
                d.MapTo(containerObj);
                containerObj.CreatedDateStr = string.Format("{0:dd.MM.yyyy}", d.CreatedDate);
                containerObj.UserName = d.User != null ? d.User.UserName : "";
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SetNotifyAsSeen(int userId, int notificationId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<Notification>();
                var dbObj = repo.GetById(notificationId);
                if (dbObj == null)
                    throw new Exception("Bildirim kaydı bulunamadı.");

                if (dbObj.UserId != userId)
                    throw new Exception("Bildirimin hedefi olan kullanıcı ile gören uyuşmuyor.");

                dbObj.SeenStatus = 1;
                dbObj.SeenDate = DateTime.Now;

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

        public BusinessResult SetNotifyAsPushed(int userId, int notificationId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<Notification>();
                var dbObj = repo.GetById(notificationId);
                if (dbObj == null)
                    throw new Exception("Bildirim kaydı bulunamadı.");

                if (dbObj.UserId != userId)
                    throw new Exception("Bildirimin hedefi olan kullanıcı ile gören uyuşmuyor.");

                dbObj.PushStatus = 1;

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

        #region USERS MANAGEMENT
        public UserModel GetUser(int id)
        {
            UserModel model = new UserModel();

            var repo = _unitOfWork.GetRepository<User>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                if (dbObj.ProfileImage != null)
                {
                    model.ProfileImageBase64 = "data:image/png;base64, " + Convert.ToBase64String(dbObj.ProfileImage);
                }
            }

            return model;
        }

        public UserModel[] GetUserList()
        {
            List<UserModel> data = new List<UserModel>();

            var repo = _unitOfWork.GetRepository<User>();

            repo.GetAll().ToList().ForEach(d =>
            {
                UserModel containerObj = new UserModel();
                d.MapTo(containerObj);
                containerObj.ProfileImage = null;
                containerObj.ProfileImageBase64 = "";
                containerObj.RoleName = d.UserRole != null ? d.UserRole.RoleName : "";
                containerObj.IsProdTerminal = d.UserRole.UserAuth
                    .Any(m => m.UserAuthType.AuthTypeCode == "MobileProductionUser" && m.IsGranted == true);
                containerObj.IsProdChief = d.UserRole.UserAuth
                    .Any(m => m.UserAuthType.AuthTypeCode == "IsProductionChief" && m.IsGranted == true);
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public UserModel[] GetProductionChiefList()
        {
            List<UserModel> data = new List<UserModel>();

            var repo = _unitOfWork.GetRepository<User>();
            var repoRole = _unitOfWork.GetRepository<UserRole>();

            var chiefRole = repoRole.Filter(d => 
                d.UserAuth.Any(m => m.UserAuthType.AuthTypeCode == "IsProductionChief" && m.IsGranted == true))
                .FirstOrDefault();

            if (chiefRole == null)
                return default;

            repo.Filter(d => d.UserRoleId == chiefRole.Id).ToList().ForEach(d =>
            {
                UserModel containerObj = new UserModel();
                d.MapTo(containerObj);
                containerObj.ProfileImage = null;
                containerObj.ProfileImageBase64 = "";
                containerObj.RoleName = d.UserRole != null ? d.UserRole.RoleName : "";
                containerObj.IsProdTerminal = d.UserRole.UserAuth
                    .Any(m => m.UserAuthType.AuthTypeCode == "MobileProductionUser" && m.IsGranted == true);
                containerObj.IsProdChief = d.UserRole.UserAuth
                    .Any(m => m.UserAuthType.AuthTypeCode == "IsProductionChief" && m.IsGranted == true);
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateUser(UserModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.UserCode))
                    throw new Exception("Kullanıcı kodu girilmelidir.");
                if (string.IsNullOrEmpty(model.Login))
                    throw new Exception("Giriş bilgisi girilmelidir.");

                var repo = _unitOfWork.GetRepository<User>();

                if (repo.Any(d => (d.Login == model.Login)
                    && d.Id != model.Id))
                    throw new Exception("Aynı giriş bilgisine sahip başka bir kullanıcı mevcuttur. Lütfen farklı bir giriş bilgisi giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new User();
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

        public BusinessResult DeleteUser(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<User>();

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

        #endregion

        #region USER ROLE MANAGEMENT
        public UserRoleModel[] GetUserRoleList()
        {
            List<UserRoleModel> data = new List<UserRoleModel>();

            var repo = _unitOfWork.GetRepository<UserRole>();

            repo.GetAll().ToList().ForEach(d =>
            {
                UserRoleModel containerObj = new UserRoleModel();
                d.MapTo(containerObj);
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateUserRole(UserRoleModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.RoleName))
                    throw new Exception("Rol tanımı girilmelidir.");

                var repo = _unitOfWork.GetRepository<UserRole>();
                var repoAuth = _unitOfWork.GetRepository<UserAuth>();
                var repoSbsc = _unitOfWork.GetRepository<UserRoleSubscription>();

                if (repo.Any(d => (d.RoleName == model.RoleName)
                    && d.Id != model.Id))
                    throw new Exception("Aynı tanıma sahip başka bir rol mevcuttur. Lütfen farklı bir rol tanımı giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new UserRole();
                    dbObj.CreatedDate = DateTime.Now;
                    dbObj.CreatedUserId = model.CreatedUserId;
                    repo.Add(dbObj);
                }

                var crDate = dbObj.CreatedDate;

                model.MapTo(dbObj);

                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = crDate;

                dbObj.UpdatedDate = DateTime.Now;

                #region SAVE AUTH TYPES
                if (model.AuthTypes != null)
                {
                    foreach (var item in model.AuthTypes)
                    {
                        var dbAuthObj = dbObj.UserAuth.FirstOrDefault(m => m.AuthTypeId == item.AuthTypeId);
                        if (dbAuthObj == null)
                        {
                            dbAuthObj = new UserAuth
                            {
                                AuthTypeId = item.AuthTypeId,
                                IsGranted = item.IsGranted,
                                UserRole = dbObj
                            };
                            repoAuth.Add(dbAuthObj);
                        }
                        else
                            dbAuthObj.IsGranted = item.IsGranted;
                    }
                }
                #endregion

                #region SAVE SUBSCRIPTION CATEGORIES
                if (model.Subscriptions != null)
                {
                    var toBeRemovedSubscriptions = dbObj.UserRoleSubscription
                    .Where(d => !model.Subscriptions
                        .Select(m => m.SubscriptionCategory).ToArray().Contains(d.SubscriptionCategory)
                    ).ToArray();
                    foreach (var item in toBeRemovedSubscriptions)
                    {
                        repoSbsc.Delete(item);
                    }

                    foreach (var item in model.Subscriptions)
                    {
                        var dbSbscObj = dbObj.UserRoleSubscription.FirstOrDefault(m => m.SubscriptionCategory == item.SubscriptionCategory);
                        if (dbSbscObj == null)
                        {
                            dbSbscObj = new UserRoleSubscription
                            {
                                SubscriptionCategory = item.SubscriptionCategory,
                                UserRole = dbObj,
                            };
                            repoSbsc.Add(dbSbscObj);
                        }
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

        public BusinessResult DeleteUserRole(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<UserRole>();

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

        public UserRoleModel GetUserRole(int id)
        {
            UserRoleModel model = new UserRoleModel { };

            var repo = _unitOfWork.GetRepository<UserRole>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.AuthTypes = dbObj.UserAuth.Select(d => new UserAuthModel
                {
                    Id = d.Id,
                    AuthTypeId = d.AuthTypeId,
                    IsGranted = d.IsGranted,
                    UserId = d.UserId,
                    UserRoleId = d.UserRoleId
                }).ToArray();
                model.Subscriptions = dbObj.UserRoleSubscription.Select(d => new UserRoleSubscriptionModel
                {
                    Id = d.Id,
                    UserRoleId = d.UserRoleId,
                    SubscriptionCategory = d.SubscriptionCategory ?? 0,
                    SubscriptionText = ((SubscriptionCategory)d.SubscriptionCategory).ToCaption(),
                }).ToArray();
            }

            return model;
        }

        public UserAuthTypeModel[] GetAuthTypeList()
        {
            UserAuthTypeModel[] data = new UserAuthTypeModel[0];

            var repo = _unitOfWork.GetRepository<UserAuthType>();
            data = repo.GetAll().Select(d => new UserAuthTypeModel
            {
                Id=d.Id,
                AuthTypeCode = d.AuthTypeCode,
                AuthTypeName = d.AuthTypeName
            }).ToArray();

            return data;
        }
        #endregion

        #region DRIVER BUSINESS
        public DriverModel GetDriver(int id)
        {
            DriverModel model = new DriverModel();

            var repo = _unitOfWork.GetRepository<Driver>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.BirthDateStr = String.Format("{0:dd.MM.yyyy}", model.BirthDate);
                model.VisaStartDateStr = string.Format("{0:dd.MM.yyyy}",model.VisaStartDate);
                model.VisaEndDateStr = string.Format("{0:dd.MM.yyyy}", model.VisaEndDate);

                if (dbObj.ProfileImage != null)
                {
                    model.ProfileImageBase64 = "data:image/png;base64, " + Convert.ToBase64String(dbObj.ProfileImage);
                }
            }

            return model;
        }

        public DriverModel[] GetDriverList()
        {
            List<DriverModel> data = new List<DriverModel>();

            var repo = _unitOfWork.GetRepository<Driver>();

            repo.GetAll().ToList().ForEach(d =>
            {
                DriverModel containerObj = new DriverModel();
                d.MapTo(containerObj);
                containerObj.BirthDateStr = String.Format("{0:dd.MM.yyyy}", d.BirthDate);
                containerObj.VisaStartDateStr = String.Format("{0:dd.MM.yyyy}", d.VisaStartDate);
                containerObj.VisaEndDateStr = String.Format("{0:dd.MM.yyyy}", d.VisaEndDate);
                containerObj.VisaTypeStr = d.VisaType != null ? d.VisaType == 1 ? "SCHENGEN" : "" :"";
                containerObj.CountryName = d.Country != null ? d.Country.CountryName : "";
                containerObj.ProfileImage = null;
                containerObj.ProfileImageBase64 = "";
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateDriver(DriverModel model)
        {
            BusinessResult result = new BusinessResult();
            bool newRecord = false;
            try
            {
                if (string.IsNullOrEmpty(model.DriverName))
                    throw new Exception("Şoför adı bilgisi girilmelidir.");
                if (string.IsNullOrEmpty(model.DriverSurName))
                    throw new Exception("Şoför soyadı bilgisi girilmelidir.");
                if (!String.IsNullOrEmpty(model.BirthDateStr))
                    model.BirthDate = DateTime.ParseExact(model.BirthDateStr, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
                if (!String.IsNullOrEmpty(model.VisaStartDateStr))
                    model.VisaStartDate = DateTime.ParseExact(model.VisaStartDateStr, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
                if (!String.IsNullOrEmpty(model.VisaEndDateStr))
                    model.VisaEndDate = DateTime.ParseExact(model.VisaEndDateStr, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));

                var repo = _unitOfWork.GetRepository<Driver>();
                var repoAccount = _unitOfWork.GetRepository<DriverAccount>();

                if (model.Tc != null && repo.Any(d => (d.Tc == model.Tc)
                    && d.Id != model.Id))
                    throw new Exception("Aynı Tc bilgisine sahip başka bir şoför mevcuttur. Lütfen farklı bir giriş bilgisi giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new Driver();
                    dbObj.CreatedDate = DateTime.Now;
                    dbObj.CreatedUserId = model.CreatedUserId;
                    newRecord = true;
                    repo.Add(dbObj);
                }

                var crDate = dbObj.CreatedDate;

                model.MapTo(dbObj);

                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = crDate;

                dbObj.UpdatedDate = DateTime.Now;
                var driverAccountList = repoAccount.Filter(d => d.DriverId == model.Id).ToArray();
                if (newRecord)
                {
                    var dbTlAccount = repoAccount.Filter(d => d.DriverId == model.Id && d.ForexTypeId == LSabit.GET_FOREXTYPE_TL).ToArray();
                    if(dbTlAccount.Length == 0)
                    {
                        var dbTlAcc = new DriverAccount();
                        dbTlAcc.ForexTypeId = LSabit.GET_FOREXTYPE_TL;
                        dbTlAcc.DriverId = model.Id;
                        dbTlAcc.Balance = 0;
                        repoAccount.Add(dbTlAcc);
                    }
                    var dbUsdAccount = repoAccount.Filter(d => d.DriverId == model.Id && d.ForexTypeId == LSabit.GET_FOREXTYPE_USD).ToArray();
                    if (dbUsdAccount.Length == 0)
                    {
                        var dbUsdAcc = new DriverAccount();
                        dbUsdAcc.ForexTypeId = LSabit.GET_FOREXTYPE_USD;
                        dbUsdAcc.DriverId = model.Id;
                        dbUsdAcc.Balance = 0;
                        repoAccount.Add(dbUsdAcc);
                    }
                    var dbEuroAccount = repoAccount.Filter(d => d.DriverId == model.Id && d.ForexTypeId == LSabit.GET_FOREXTYPE_EURO).ToArray();
                    if (dbUsdAccount.Length == 0)
                    {
                        var dbEuroAcc = new DriverAccount();
                        dbEuroAcc.ForexTypeId = LSabit.GET_FOREXTYPE_EURO;
                        dbEuroAcc.DriverId = model.Id;
                        dbEuroAcc.Balance = 0;
                        repoAccount.Add(dbEuroAcc);
                    }

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

        public BusinessResult DeleteDriver(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<Driver>();

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

        #endregion

        #region ROTA BUSINESS
        public RotaModel GetRota(int id)
        {
            RotaModel model = new RotaModel();

            var repo = _unitOfWork.GetRepository<Rota>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.CityStartName = dbObj.CityStart != null ? dbObj.CityStart.CityName : "";
                model.CityEndName = dbObj.CityEnd != null ? dbObj.CityEnd.CityName : "";

                if (dbObj.ProfileImage != null)
                {
                    model.ProfileImageBase64 = "data:image/png;base64, " + Convert.ToBase64String(dbObj.ProfileImage);
                }
            }

            return model;
        }

        public RotaModel[] GetRotaList()
        {
            List<RotaModel> data = new List<RotaModel>();

            var repo = _unitOfWork.GetRepository<Rota>();

            repo.GetAll().ToList().ForEach(d =>
            {
                RotaModel containerObj = new RotaModel();
                d.MapTo(containerObj);
                containerObj.CityStartName = d.CityStart != null ? d.CityStart.CityName : "";
                containerObj.CityEndName = d.CityEnd != null ? d.CityEnd.CityName : "";
                containerObj.ProfileImage = null;
                containerObj.ProfileImageBase64 = "";
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdateRota(RotaModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {

                var repo = _unitOfWork.GetRepository<Rota>();

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new Rota();
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

        public BusinessResult DeleteRota(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<Rota>();

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
        #endregion
    }
}
