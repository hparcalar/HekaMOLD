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
            { ItemReceiptType.EntryFromProduction, "Üretimden Giriş Fişi" },
            { ItemReceiptType.Consumption, "Sarf Fişi" },
            { ItemReceiptType.Wastage, "Fire Fişi" },
            { ItemReceiptType.WarehouseInput, "Depo Giriş Fişi" },
            { ItemReceiptType.WarehouseOutput, "Depo Çıkış Fişi" },
            { ItemReceiptType.ToContractor, "Fasona Çıkış" },
            { ItemReceiptType.FromContractor, "Fasondan Giriş" },
            { ItemReceiptType.DeliveryToProduction, "Üretime Çıkış Fişi" },
        };

        public static Dictionary<int, string> 
            GetReceiptTypes(ReceiptCategoryType receiptCategory)
        {
            Dictionary<int, string> data = new Dictionary<int, string>();

            int[] properTypes = null;

            if (receiptCategory == ReceiptCategoryType.All)
                properTypes = Values.Select(d => (int)d.Key).ToArray();
            else if (receiptCategory == ReceiptCategoryType.Purchasing)
                properTypes = PurchasingTypes;
            else if (receiptCategory == ReceiptCategoryType.ItemManagement)
                properTypes = ItemManagementTypes;
            else if (receiptCategory == ReceiptCategoryType.Sales)
                properTypes = SalesTypes;
            else if (receiptCategory == ReceiptCategoryType.AllInputs)
                properTypes = InputTypes;
            else if (receiptCategory == ReceiptCategoryType.AllOutputs)
                properTypes = OutputTypes;

            data = Values.Where(d => properTypes.Contains((int)d.Key))
                .Select(d => new { Id = (int)d.Key, Text = d.Value })
                .ToDictionary(k => k.Id, v => v.Text);

            return data;
        }

        public static int[] PurchasingTypes = new int[] { 
            (int)ItemReceiptType.ItemBuying,
            (int)ItemReceiptType.ItemBuyingReturn
        };

        public static int[] ItemManagementTypes = new int[] { 
            (int)ItemReceiptType.Consumption,
            (int)ItemReceiptType.Wastage,
            (int)ItemReceiptType.WarehouseInput,
            (int)ItemReceiptType.WarehouseOutput,
            (int)ItemReceiptType.EntryFromProduction,
            (int)ItemReceiptType.DeliveryToProduction,
        };

        public static int[] SalesTypes = new int[] { 
            (int)ItemReceiptType.ItemSelling,
            (int)ItemReceiptType.ItemSellingReturn
        };

        public static int[] InputTypes = new int[]
        {
            (int)ItemReceiptType.ItemBuying,
            (int)ItemReceiptType.ItemSellingReturn,
            (int)ItemReceiptType.WarehouseInput,
            (int)ItemReceiptType.EntryFromProduction
        };

        public static int[] OutputTypes = new int[]
        {
            (int)ItemReceiptType.ItemBuyingReturn,
            (int)ItemReceiptType.WarehouseOutput,
            (int)ItemReceiptType.Consumption,
            (int)ItemReceiptType.Wastage,
            (int)ItemReceiptType.DeliveryToProduction,
        };
    }
}
