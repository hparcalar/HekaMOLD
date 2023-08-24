using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Receipt
{
    public class CountingReceiptSerialModel
    {
        public int Id { get; set; }
        public string Barcode { get; set; }
        public Nullable<int> ItemId { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<int> CountingReceiptDetailId { get; set; }

        #region VISUAL ELEMENTS
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        #endregion
    }
}
