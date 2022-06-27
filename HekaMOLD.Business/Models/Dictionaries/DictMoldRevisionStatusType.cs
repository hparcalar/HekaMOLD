using HekaMOLD.Business.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Dictionaries
{
    public class DictMoldRevisionStatusType
    {
        public static Dictionary<MoldRevisionStatusType, string> Values
           = new Dictionary<MoldRevisionStatusType, string>()
       {
            { MoldRevisionStatusType.Waiting, "Beklemede" },
            { MoldRevisionStatusType.OnHold, "İşlem Bekletiliyor" },
            { MoldRevisionStatusType.Completed, "Tamamlandı" },
            { MoldRevisionStatusType.Cancelled, "İptal Edildi" },
       };
    }
}
