using Heka.DataAccess.Context;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.UseCases.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.UseCases
{
  public  class VoyageBO : CoreReceiptsBO
    {
        public string GetNextVoyageCode(int directionId = 0)
        {
            string defaultValue = "";
            try
            {
                var repo = _unitOfWork.GetRepository<CodeCounter>();
                var dbCodeCounter = repo.Filter(d => d.CounterType == 3)// CounterType type=1(Order) type=2(Load) type=3(Voyage)
                    .OrderByDescending(d => d.Id)
                    .Select(d => d)
                    .FirstOrDefault();
                defaultValue = dbCodeCounter.FirstValue + string.Format("{0:00000}", Convert.ToInt32(directionId == 1 ? (int)dbCodeCounter.Export : directionId == 2 ? (int)dbCodeCounter.Import : directionId == 3 ? (int)dbCodeCounter.Domestic : directionId == 4 ?
                    (int)dbCodeCounter.Transit : dbCodeCounter.Id) + 1) + ((OrderTransactionDirectionType)directionId).ToCaption();
                return defaultValue;
            }
            catch (Exception)
            {

            }

            return defaultValue;
        }
    }
}
