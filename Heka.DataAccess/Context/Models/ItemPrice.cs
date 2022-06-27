using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public partial class ItemPrice
    {
        public int Id { get; set; }

        [ForeignKey("Item")]
        public Nullable<int> ItemId { get; set; }

        [ForeignKey("ForexType")]
        public Nullable<int> ForexTypeId { get; set; }

        public Nullable<decimal> Price { get; set; }
        public Nullable<int> PriceType { get; set; }
        public Nullable<bool> IsDefault { get; set; }

        public virtual Item Item { get; set; }
        public virtual ForexType ForexType { get; set; }
    }
}
