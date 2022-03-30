using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public partial class ItemQualityType
    {
        public ItemQualityType()
        {
            this.Item = new HashSet<Item>();
            this.ItemVariant = new HashSet<ItemVariant>();

        }
        public int Id { get; set; }
        public string ItemQualityTypeCode { get; set; }
        public string ItemQualityTypeName { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        [InverseProperty("ItemQualityType")]
        public virtual ICollection<Item> Item { get; set; }

        [InverseProperty("ItemQualityType")]
        public virtual ICollection<ItemVariant> ItemVariant { get; set; }

    }
}
