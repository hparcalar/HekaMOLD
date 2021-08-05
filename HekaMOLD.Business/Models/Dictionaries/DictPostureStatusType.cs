using HekaMOLD.Business.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Dictionaries
{
    public class DictPostureStatusType
    {
        public static Dictionary<PostureStatusType, string> Values
            = new Dictionary<PostureStatusType, string>()
        {
            { PostureStatusType.Started, "Devam Ediyor" },
            { PostureStatusType.WorkingOn, "Müdahale Ediliyor" },
            { PostureStatusType.Resolved, "Çözüldü" },
            { PostureStatusType.Cancelled, "İptal Edildi" },
        };
    }
}
