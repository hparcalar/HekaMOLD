using HekaMOLD.Business.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Dictionaries
{
    public class DictSubscriptionCategory
    {
        public static Dictionary<SubscriptionCategory, string> Values
            = new Dictionary<SubscriptionCategory, string>()
        {
            { SubscriptionCategory.ItemRequest, "Talep Bildirimleri" },
            { SubscriptionCategory.ItemOrder, "Sipariş Bildirimleri" },
            { SubscriptionCategory.Incident, "Arıza Bildirimleri" },
            { SubscriptionCategory.Posture, "Duruş Bildirimleri" },
            { SubscriptionCategory.ItemCriticals, "Malzeme Kritik Seviye Bildirimleri" },
        };
    }
}
