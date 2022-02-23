using HekaMOLD.Business.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Dictionaries
{
    public class DictItemType
    {
        public static Dictionary<ItemType, string> Values 
            = new Dictionary<ItemType, string>()
        {
            { ItemType.RawMaterials, "Hammadde" },
            { ItemType.Commercial, "Ticari Mal" },
            { ItemType.SemiProduct, "Yarı Mamul" },
            { ItemType.Product, "Mamul" },
            { ItemType.Attempt, "Deneme Ürünü" }

        };
    }
}
