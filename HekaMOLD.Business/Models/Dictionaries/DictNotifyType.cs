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
            { NotifyType.ItemOrderIsApproved, "Sipariş Sonucu" }
        };
    }
}
