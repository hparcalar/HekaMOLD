using HekaMOLD.Business.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Dictionaries
{
    public class DictOrderStatusType
    {
        public static Dictionary<OrderStatusType, string> Values 
            = new Dictionary<OrderStatusType, string>()
        {
            { OrderStatusType.Created, "Onay Bekliyor" },
            { OrderStatusType.Approved, "Sipariş Onaylandı" },
            { OrderStatusType.Cancelled, "Sipariş Reddedildi" },
            { OrderStatusType.Completed, "Sipariş Tamamlandı" }
        };
    }
}
