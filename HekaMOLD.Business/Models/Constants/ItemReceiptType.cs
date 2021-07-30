﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Constants
{
    public enum ItemReceiptType
    {
        ItemBuying = 1,
        ItemBuyingReturn = 101,
        ItemSelling = 102,
        ItemSellingReturn = 2,
        EntryFromProduction=4,
        Consumption = 103,
        Wastage = 104,
        WarehouseInput = 3,
        WarehouseOutput = 105
    }
}