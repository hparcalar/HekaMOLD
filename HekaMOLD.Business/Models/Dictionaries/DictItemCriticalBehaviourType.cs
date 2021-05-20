using HekaMOLD.Business.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Dictionaries
{
    public class DictItemCriticalBehaviourType
    {
        public static Dictionary<ItemCriticalBehaviourType, string> Values 
            = new Dictionary<ItemCriticalBehaviourType, string>()
        {
            { ItemCriticalBehaviourType.None, "Yok" },
            { ItemCriticalBehaviourType.Warn, "Uyar" },
            { ItemCriticalBehaviourType.Prevent, "İzin Verme" }
        };
    }
}
