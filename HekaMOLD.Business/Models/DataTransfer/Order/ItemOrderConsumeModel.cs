using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Order
{
    public class ItemOrderConsumeModel
    {
        public int Id { get; set; }
        public int? ItemOrderDetailId { get; set; }
        public int? ConsumerReceiptDetailId { get; set; }
        public int? ConsumedReceiptDetailId { get; set; }
        public decimal? UsedQuantity { get; set; }
        public decimal? UsedGrossQuantity { get; set; }
        public int? UnitId { get; set; }
    }
}
