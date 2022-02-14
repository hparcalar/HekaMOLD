using HekaMOLD.Business.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Dictionaries
{
    public class DictLoadStatusType
    {
        public static Dictionary<LoadStatusType, string> Values
    = new Dictionary<LoadStatusType, string>()
{
            { LoadStatusType.Created, "Hazır Bekliyor" },
            { LoadStatusType.Approved, "Yük Onaylandı" },
            { LoadStatusType.Cancelled, "Yük Reddedildi" },
            { LoadStatusType.Completed, "Yük Tamamlandı" },
            { LoadStatusType.Emptied, "Yük Boşaltıldı" },
            { LoadStatusType.OnTheRoad, "Yük Yolda"},
            { LoadStatusType.InWarehouse, "Yük Depoda"},

};

    }
}
