using HekaMOLD.Business.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Dictionaries
{
    public class DictReceiptStatusType
    {
        public static Dictionary<ReceiptStatusType, string> Values 
            = new Dictionary<ReceiptStatusType, string>()
        {
            { ReceiptStatusType.Created, "" },
            { ReceiptStatusType.InUse, "Kullanımda" },
            { ReceiptStatusType.WaitingQualityApprove, "Kalite Onayı Bekliyor" },
            { ReceiptStatusType.QualityIsApproved, "Kalite Onayladı" },
            { ReceiptStatusType.Blocked, "Blokeli" },
            { ReceiptStatusType.Closed, "Kapalı" }
        };
    }
}
