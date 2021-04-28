using Heka.DataAccess.Context;
using HekaMOLD.Business.Base;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Authentication;
using HekaMOLD.Business.Models.DataTransfer.Core;
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
        #endregion
    }
}
