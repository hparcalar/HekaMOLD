using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heka.DataAccess.Context
{
    public class ReturnalProductDetail
    {
        public int Id { get; set; }

        [ForeignKey("ReturnalProduct")]
        public Nullable<int> ReturnalProductId { get; set; }

        [ForeignKey("Item")]
        public Nullable<int> ItemId { get; set; }

        public Nullable<decimal> Quantity { get; set; }
        public Nullable<decimal> PackageQuantity { get; set; }
        public Nullable<decimal> PalletQuantity { get; set; }
        public string Explanation { get; set; }

        public virtual ReturnalProduct ReturnalProduct { get; set; }
        public virtual Item Item { get; set; }
    }
}
