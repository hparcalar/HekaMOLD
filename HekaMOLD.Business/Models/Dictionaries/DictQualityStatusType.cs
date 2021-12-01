using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HekaMOLD.Business.Models.Constants;

namespace HekaMOLD.Business.Models.Dictionaries
{
    public class DictQualityStatusType
    {
        public static Dictionary<QualityStatusType, string> Values
            = new Dictionary<QualityStatusType, string>()
        {
            { QualityStatusType.Waiting, "Kalite Kontrol Bekliyor" },
            { QualityStatusType.Ok, "Kalite Onaylandı" },
            { QualityStatusType.Nok, "Kalite Reddedildi" },
            { QualityStatusType.QualityWaiting, "Kalite Beklemede" },
            { QualityStatusType.ConditionalApproved, "Şartlı Kabul" },
        };
    }
}
