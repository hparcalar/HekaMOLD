using HekaMOLD.Business.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Dictionaries
{
    public class DictItemReceiptType
    {
        public static Dictionary<ItemReceiptType, string> Values
            = new Dictionary<ItemReceiptType, string>()
        {
            { ItemReceiptType.ItemBuying, "Mal Alım İrsaliyesi" },
            { ItemReceiptType.ItemBuyingReturn, "Mal Alım İadesi" },
            { ItemReceiptType.ItemSelling, "Satış İrsaliyesi" },
            { ItemReceiptType.ItemSellingReturn, "Satıştan İade" },
            { ItemReceiptType.Consumption, "Sarf Fişi" },
            { ItemReceiptType.Wastage, "Fire Fişi" },
            { ItemReceiptType.WarehouseInput, "Depo Giriş Fişi" },
            { ItemReceiptType.WarehouseOutput, "Depo Çıkış Fişi" }
        };
    }
}
