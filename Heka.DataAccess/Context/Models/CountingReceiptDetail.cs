using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public class CountingReceiptDetail
    {
        public CountingReceiptDetail()
        {
            this.CountingReceiptSerial = new HashSet<CountingReceiptSerial>();
        }
        public int Id { get; set; }

        [ForeignKey("CountingReceipt")]
        public Nullable<int> CountingReceiptId { get; set; }

        [ForeignKey("Item")]
        public Nullable<int> ItemId { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<int> PackageQuantity { get; set; }

        [ForeignKey("Warehouse")]
        public Nullable<int> WarehouseId { get; set; }

        public virtual Item Item { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual CountingReceipt CountingReceipt { get; set; }

        [InverseProperty("CountingReceiptDetail")]
        public ICollection<CountingReceiptSerial> CountingReceiptSerial { get; set; }
    }
}
