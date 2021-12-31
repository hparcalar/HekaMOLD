using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public class CountingReceipt
    {
        public CountingReceipt()
        {
            this.CountingReceiptDetail = new HashSet<CountingReceiptDetail>();
        }
        public int Id { get; set; }
        public Nullable<DateTime> CountingDate { get; set; }
        public string ReceiptNo { get; set; }

        [ForeignKey("Plant")]
        public Nullable<int> PlantId { get; set; }
        public Nullable<int> CountingStatus { get; set; }

        public virtual Plant Plant { get; set; }

        [InverseProperty("CountingReceipt")]
        public ICollection<CountingReceiptDetail> CountingReceiptDetail { get; set; }
    }
}
