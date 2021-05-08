using HekaMOLD.Business.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Dictionaries
{
    public class DictWarehouseType
    {
        public static Dictionary<WarehouseType, string> Values 
            = new Dictionary<WarehouseType, string>()
        {
            { WarehouseType.ItemWarehouse, "Malzeme Deposu" },
            { WarehouseType.ProductWarehouse, "Ürün Deposu" }
        };
    }
}
