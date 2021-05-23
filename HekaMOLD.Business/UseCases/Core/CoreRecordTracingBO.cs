using HekaMOLD.Business.Base;
using HekaMOLD.Business.Models.DataTransfer.Core;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Heka.DataAccess.Context;

namespace HekaMOLD.Business.UseCases.Core
{
    public class CoreRecordTracingBO : IBusinessObject
    {
        public RecordInformationModel GetRecordInformation(int id, string dataType)
        {
            RecordInformationModel result = new RecordInformationModel();

            try
            {
                var objType = Type.GetType("Heka.DataAccess.Context." + dataType + ", Heka.DataAccess");

                var repo = _unitOfWork.GetType().GetMethod("GetRepository")
                    .MakeGenericMethod(objType)
                    .Invoke(_unitOfWork, new object[] { });
                var repoUser = _unitOfWork.GetRepository<User>();

                //var dbObj = repo.GetType().GetMethod("Get")
                //    .Invoke(repo, new object[] { 
                //        Expression.Lambda(
                //            Expression.Equal(
                //                Expression.Property(Expression.Parameter(objType, "d"), "Id"),
                //                Expression.Constant(id)    
                //            )
                //        )
                //    });

                var dbObj = repo.GetType().GetMethod("GetById")
                    .Invoke(repo, new object[] { id });

                if (dbObj == null)
                    throw new Exception("Kayıt bilgisine ulaşılamadı.");

                var pInfoCrUser = objType.GetProperty("CreatedUserId");
                if (pInfoCrUser != null)
                {
                    result.CreatedUserId = (int?)pInfoCrUser.GetValue(dbObj, null);
                    if (result.CreatedUserId != null)
                        result.CreatedUserName = repoUser.Get(d => d.Id == result.CreatedUserId.Value).UserName;
                    else
                        result.CreatedUserName = "";
                }

                var pInfoUpUser = objType.GetProperty("UpdatedUserId");
                if (pInfoUpUser != null)
                {
                    result.UpdatedUserId = (int?)pInfoUpUser.GetValue(dbObj, null);
                    if (result.UpdatedUserId != null)
                        result.UpdatedUserName = repoUser.Get(d => d.Id == result.UpdatedUserId.Value).UserName;
                    else
                        result.UpdatedUserName = "";
                }

                var pInfoCrDate = objType.GetProperty("CreatedDate");
                if (pInfoCrDate != null)
                {
                    result.CreatedDate = (DateTime?)pInfoCrDate.GetValue(dbObj, null);
                    if (result.CreatedDate != null)
                        result.CreatedDateStr = string.Format("{0:dd.MM.yyyy HH:mm}", result.CreatedDate.Value);
                    else
                        result.CreatedDateStr = "";
                }

                var pInfoUpDate = objType.GetProperty("UpdatedDate");
                if (pInfoUpDate != null)
                {
                    result.UpdatedDate = (DateTime?)pInfoUpDate.GetValue(dbObj, null);
                    if (result.UpdatedDate != null)
                        result.UpdatedDateStr = string.Format("{0:dd.MM.yyyy HH:mm}", result.UpdatedDate.Value);
                    else
                        result.UpdatedDateStr = "";
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
    }
}
