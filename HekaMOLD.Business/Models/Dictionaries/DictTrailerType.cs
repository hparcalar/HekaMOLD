using HekaMOLD.Business.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Dictionaries
{
   public class DictTrailerType
    {
        public static Dictionary<TrailerType, string> Values
    = new Dictionary<TrailerType, string>()
{
            { TrailerType.Cadirli, "Çadirlı" },
            { TrailerType.Frigo, "Frigo" },
            { TrailerType.KapaliKasa, "Kapalı Kasa" },
            { TrailerType.Optima, "Optima" },
            { TrailerType.Mega, "Mega" },
            { TrailerType.Konteyner, "Konteyner" },
            { TrailerType.Swapboddy, "Swapboddy" },
            { TrailerType.Lowbed, "Lowbed" },
            { TrailerType.KamyonRomork, "Kamyon Romörk" },
            { TrailerType.Standart, "Standart" },
            { TrailerType.Minivan, "Minivan" },

};
    }
}
