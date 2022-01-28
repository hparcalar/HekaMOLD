using HekaMOLD.Business.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Dictionaries
{
    public class DictNotifyType
    {
        public static Dictionary<NotifyType, string> Values 
            = new Dictionary<NotifyType, string>()
        {
            { NotifyType.ItemRequestWaitForApproval, "Satınalma Talebi" },
            { NotifyType.ItemRequestIsApproved, "Talep Sonucu" },
            { NotifyType.ItemOrderWaitForApproval, "Satınalma Siparişi" },
            { NotifyType.ItemOrderIsApproved, "Sipariş Sonucu" },
            { NotifyType.IncidentCreated, "Arıza Bildirimi" },
            { NotifyType.PostureCreated, "Duruş Bildirimi" },
            { NotifyType.IncidentTouched, "Arızaya Müdahale Başlandı" },
            { NotifyType.IncidentResolved, "Arıza Tamamlandı" },
            { NotifyType.PostureTouched, "Duruşa Müdahale Başlandı" },
            { NotifyType.PostureResolved, "Duruş Tamamlandı" },
            { NotifyType.ItemAtMinimumInWarehouse, "Malzeme Azalıyor" },
            { NotifyType.ItemAtMaximumInWarehouse, "Malzeme Gereğinden Fazla Birikti" },
            { NotifyType.ItemLoadWaitForApproval, "Yük Talebi" },
            { NotifyType.ItemLoadIsApproved, "Yük Sonucu" },

        };
    }
}

