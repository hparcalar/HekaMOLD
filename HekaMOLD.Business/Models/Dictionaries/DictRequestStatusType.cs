using HekaMOLD.Business.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Dictionaries
{
    public class DictRequestStatusType
    {
        public static Dictionary<RequestStatusType, string> Values 
            = new Dictionary<RequestStatusType, string>()
        {
            { RequestStatusType.Created, "Onay Bekliyor" },
            { RequestStatusType.Approved, "Talep Onaylandı" },
            { RequestStatusType.Cancelled, "Talep Reddedildi" },
            { RequestStatusType.Completed, "Talep Tamamlandı" }
        };
    }
}
