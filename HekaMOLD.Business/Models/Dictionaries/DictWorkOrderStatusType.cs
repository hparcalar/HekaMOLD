using HekaMOLD.Business.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Dictionaries
{
    public class DictWorkOrderStatusType
    {
        public static Dictionary<WorkOrderStatusType, string> Values 
            = new Dictionary<WorkOrderStatusType, string>()
        {
            { WorkOrderStatusType.Created, "Planlanması Bekleniyor" },
            { WorkOrderStatusType.Planned, "Planlandı" },
            { WorkOrderStatusType.OnHold, "Beklemeye Alındı" },
            { WorkOrderStatusType.InProgress, "Devam Ediyor" },
            { WorkOrderStatusType.Completed, "Üretim Tamamlandı" },
            { WorkOrderStatusType.Delivered, "Sevk Edildi" },
            { WorkOrderStatusType.Cancelled, "İptal Edildi" }
        };
    }
}
