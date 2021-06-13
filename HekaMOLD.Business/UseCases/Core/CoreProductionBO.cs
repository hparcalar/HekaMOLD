using Heka.DataAccess.Context;
using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.UseCases.Core
{
    public class CoreProductionBO : IBusinessObject
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
    }
}
