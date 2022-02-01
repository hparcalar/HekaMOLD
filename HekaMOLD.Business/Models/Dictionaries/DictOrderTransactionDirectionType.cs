using HekaMOLD.Business.Models.Constants;
using System.Collections.Generic;

namespace HekaMOLD.Business.Models.Dictionaries
{
    class DictOrderTransactionDirectionType
    {
        public static Dictionary<OrderTransactionDirectionType, string> Values
    = new Dictionary<OrderTransactionDirectionType, string>()
{
            { OrderTransactionDirectionType.Export, "EX" },
            { OrderTransactionDirectionType.Import, "IM" },
            { OrderTransactionDirectionType.Domestic, "YI" },
            { OrderTransactionDirectionType.Transit, "TR" },

};
    }
}
