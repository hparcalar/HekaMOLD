using HekaMOLD.Business.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Dictionaries
{
    public class DictProcessStatusType
    {
        public static Dictionary<ProcessStatusType, string> Values
            = new Dictionary<ProcessStatusType, string>()
        {
            { ProcessStatusType.Waiting, "Bekleniyor" },
            { ProcessStatusType.Started, "Çalışıyor" },
            { ProcessStatusType.Finished, "Tamamlandı" },
            { ProcessStatusType.Cancelled, "İptal Edildi" },
        };
    }
}
