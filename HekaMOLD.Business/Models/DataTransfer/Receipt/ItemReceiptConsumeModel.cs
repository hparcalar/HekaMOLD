using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Receipt
{
    public class ItemReceiptConsumeModel
    {
        public int Id { get; set; }
        public int? ConsumedReceiptDetailId { get; set; }
        public int? ConsumerReceiptDetailId { get; set; }
        public decimal? UsedQuantity { get; set; }
        public decimal? UsedGrossQuantity { get; set; }
        public int? UnitId { get; set; }

        #region VISUAL ELEMENTS

        #endregion
    }
}
