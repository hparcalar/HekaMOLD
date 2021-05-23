using Heka.DataAccess.Context;
using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.UseCases.Core
{
    public class CoreReceiptsBO : IBusinessObject
    {
        public string GetNextOrderNo(int plantId)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<ItemOrder>();
                string lastReceiptNo = repo.Filter(d => d.PlantId == plantId)
                    .OrderByDescending(d => d.OrderNo)
                    .Select(d => d.OrderNo)
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

        public string GetNextRequestNo(int plantId)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<ItemRequest>();
                string lastReceiptNo = repo.Filter(d => d.PlantId == plantId)
                    .OrderByDescending(d => d.RequestNo)
                    .Select(d => d.RequestNo)
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
    }
}
