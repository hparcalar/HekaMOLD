using HekaMOLD.Business.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Dictionaries
{
    public class DictMaintenancePlanStatusType
    {
        public static Dictionary<MaintenancePlanStatusType, string> Values
            = new Dictionary<MaintenancePlanStatusType, string>()
        {
            { MaintenancePlanStatusType.Planned, "Planlandı" },
            { MaintenancePlanStatusType.InProgress, "Çalışılıyor" },
            { MaintenancePlanStatusType.Complete, "Tamamlandı" },
            { MaintenancePlanStatusType.Cancelled, "İptal edildi" },
        };
    }
}
