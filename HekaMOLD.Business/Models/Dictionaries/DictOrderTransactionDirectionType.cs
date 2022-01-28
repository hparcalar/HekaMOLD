using HekaMOLD.Business.Models.Constants;
using System.Collections.Generic;

namespace HekaMOLD.Business.Models.Dictionaries
{
    class DictOrderTransactionDirectionType
    {
        public static Dictionary<OrderTransactionDirectionType, string> Values
    = new Dictionary<OrderTransactionDirectionType, string>()
{
            { OrderTransactionDirectionType.AbroadExport, "AEX" },
            { OrderTransactionDirectionType.AbroadImport, "AIM" },
            { OrderTransactionDirectionType.Domestic, "D" },
            { OrderTransactionDirectionType.DomesticTransfer, "DT" },
            { OrderTransactionDirectionType.AbroadTransfer, "AT" },

};
    }
}
