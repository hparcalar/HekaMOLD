using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
  public partial  class ItemKnitDensity
    {
        public ItemKnitDensity()
        {
        }
        public int Id { get; set; }

        [ForeignKey("Item")]
        public Nullable<int> ItemId { get; set; }

        [ForeignKey("ItemVariant")]
        public Nullable<int> ItemVariantId { get; set; }

        public string YarnRecipeType { get; set; }
        public int Density { get; set; }

        public virtual Item Item { get; set; }
        public virtual ItemVariant ItemVariant { get; set; }



    }
}
