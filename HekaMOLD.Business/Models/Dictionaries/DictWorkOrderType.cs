using HekaMOLD.Business.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Dictionaries
{
    public class DictWorkOrderType
    {
        public static Dictionary<WorkOrderType, string> Values
            = new Dictionary<WorkOrderType, string>()
        {
            { WorkOrderType.Casual, "Standart" },
            { WorkOrderType.TrialProduction, "Deneme Üretimi" },
        };
    }
}
