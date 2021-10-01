using HekaMOLD.Business.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Dictionaries
{
    public class DictMoldStatus
    {
        public static Dictionary<MoldStatus, string> Values
            = new Dictionary<MoldStatus, string>()
        {
            { MoldStatus.Active, "Aktif" },
            { MoldStatus.WaitingFromSupplier, "Tedarikçiden Bekleniyor" },
            { MoldStatus.OutOfUse, "Kullanım Dışı" },
        };
    }
}
