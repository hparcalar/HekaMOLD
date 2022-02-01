using HekaMOLD.Business.Models.Constants;
using System.Collections.Generic;

namespace HekaMOLD.Business.Models.Dictionaries
{
    public class DictOrderCalculationType
    {
        public static Dictionary<OrderCalculationType, string> Values
    = new Dictionary<OrderCalculationType, string>()
{
            { OrderCalculationType.Weighted, "Ağırlık" },
            { OrderCalculationType.Volumetric, "Hacim" },
            { OrderCalculationType.Ladametre, "Ladametre" },
            { OrderCalculationType.Complet, "Komple" },
            { OrderCalculationType.Volumetric, "Minimum" },

};

    }
}
