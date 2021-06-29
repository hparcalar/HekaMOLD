using HekaMOLD.Business.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Dictionaries
{
    public class DictItemOrderType
    {
        public static Dictionary<ItemOrderType, string> Values 
            = new Dictionary<ItemOrderType, string>()
        {
            { ItemOrderType.Purchase, "Satınalma Siparişi" },
            { ItemOrderType.Sale, "Satış Siparişi" },
        };
    }
}
