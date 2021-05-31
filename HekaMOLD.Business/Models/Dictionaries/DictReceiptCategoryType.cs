using HekaMOLD.Business.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Dictionaries
{
    public class DictReceiptCategoryType
    {
        public static Dictionary<ReceiptCategoryType, string> Values 
            = new Dictionary<ReceiptCategoryType, string>()
        {
            { ReceiptCategoryType.All, "İrsaliyeler" },
            { ReceiptCategoryType.Purchasing, "Satınalma İrsaliyeleri" },
            { ReceiptCategoryType.ItemManagement, "Malzeme Yönetim Fişleri" },
            { ReceiptCategoryType.Sales, "Satış İrsaliyeleri" },
            { ReceiptCategoryType.AllInputs, "Giriş İrsaliyeleri" },
            { ReceiptCategoryType.AllOutputs, "Çıkış İrsaliyeleri" }
        };
    }
}
