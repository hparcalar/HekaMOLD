using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Summary
{
    public class ItemStateModel
    {
        public int ItemId { get; set; }
        public int WarehouseId { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public int? ItemGroupId { get; set; }
        public string ItemGroupCode { get; set; }
        public string ItemGroupName { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public decimal? InQty { get; set; }
        public decimal? OutQty { get; set; }
        public decimal? TotalQty { get; set; }
    }
}
