using Heka.DataAccess.Context;
using HekaMOLD.Business.Base;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Authentication;
using HekaMOLD.Business.Models.DataTransfer.Authentication;
using HekaMOLD.Business.Models.DataTransfer.Core;
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
                containerObj.RoleName = d.UserRole != null ? d.UserRole.RoleName : "";
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
    }
}
