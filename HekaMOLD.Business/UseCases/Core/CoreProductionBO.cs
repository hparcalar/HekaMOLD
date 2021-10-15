using Heka.DataAccess.Context;
using HekaMOLD.Business.Base;
using HekaMOLD.Business.UseCases.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.UseCases.Core
{
    public class CoreProductionBO : CoreReceiptsBO
    {
        public string GetNextProductRecipeNo()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<ProductRecipe>();
                string lastReceiptNo = repo.GetAll()
                    .OrderByDescending(d => d.ProductRecipeCode)
                    .Select(d => d.ProductRecipeCode)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(lastReceiptNo))
                    lastReceiptNo = "0";

                return string.Format("{0:000000}", Convert.ToInt32(lastReceiptNo) + 1);
            }
            catch (Exception)
            {

            }

            return default;
        }

        public string GetNextWorkOrderNo()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<WorkOrder>();
                string lastWorkOrderNo = repo.GetAll()
                    .OrderByDescending(d => d.WorkOrderNo)
                    .Select(d => d.WorkOrderNo)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(lastWorkOrderNo))
                    lastWorkOrderNo = "0";

                return string.Format("{0:000000}", Convert.ToInt32(lastWorkOrderNo) + 1);
            }
            catch (Exception)
            {

            }

            return default;
        }

        public string GetNextSerialNo()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<WorkOrderSerial>();
                string lastSerialNo = repo.GetAll()
                    .OrderByDescending(d => d.SerialNo)
                    .Select(d => d.SerialNo)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(lastSerialNo))
                    lastSerialNo = "0";

                return string.Format("{0:00000000}", Convert.ToInt64(lastSerialNo) + 1);
            }
            catch (Exception)
            {

            }

            return default;
        }
    }
}
