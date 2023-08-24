using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public class CountingReceiptSerial
    {
        public int Id { get; set; }
        public string Barcode { get; set; }

        [ForeignKey("Item")]
        public Nullable<int> ItemId { get; set; }
        public Nullable<decimal> Quantity { get; set; }

        [ForeignKey("CountingReceiptDetail")]
        public Nullable<int> CountingReceiptDetailId { get; set; }

        public virtual CountingReceiptDetail CountingReceiptDetail { get; set; }
        public virtual Item Item { get; set; }
    }
}
