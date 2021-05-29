using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class ItemLiveStatusModel
    {
        public int Id { get; set; }
        public int? ItemId { get; set; }
        public int? WarehouseId { get; set; }
        public decimal? InQuantity { get; set; }
        public decimal? OutQuantity { get; set; }
        public decimal? LiveQuantity { get; set; }
    }
}
