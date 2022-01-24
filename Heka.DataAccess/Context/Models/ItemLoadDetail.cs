using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context.Models
{
   public partial class ItemLoadDetail
    {
        public ItemLoadDetail()
        {
            this.ItemOrderDetail = new HashSet<ItemOrderDetail>();
        }
        public int Id { get; set; }

        [ForeignKey("ItemLoad")]
        public Nullable<int> ItemLoadId { get; set; }
        public Nullable<int> LineNumber { get; set; }

        [ForeignKey("Item")]
        public Nullable<int> ItemId { get; set; }

        [ForeignKey("UnitType")]
        public Nullable<int> UnitId { get; set; }

        public Nullable<decimal> Quantity { get; set; }
        public Nullable<decimal> NetQuantity { get; set; }
        public Nullable<decimal> GrossQuantity { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public int? ShortWidth { get; set; }
        public int? LongWidth { get; set; }
        public decimal? Volume { get; set; }
        public int? Height { get; set; }
        public int? Weight { get; set; }
        public int? PackageInNumber { get; set; }
        //istiflenebilir
        public bool? Stackable { get; set; }
        public decimal? Ladametre { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        public virtual ItemLoad ItemLoad { get; set; }
        public virtual Item Item { get; set; }
        public virtual UnitType UnitType { get; set; }

        [InverseProperty("ItemLoadDetail")]
        public virtual ICollection<ItemOrderDetail> ItemOrderDetail { get; set; }
    }
}
