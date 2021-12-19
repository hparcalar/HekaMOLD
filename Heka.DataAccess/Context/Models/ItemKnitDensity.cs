using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context.Models
{
  public partial  class ItemKnitDensity
    {
        public ItemKnitDensity()
        {
        }
        public int Id { get; set; }

        [ForeignKey("Item")]
        public int ItemId { get; set; }

        public string YarnRecipeType { get; set; }
        public int Density { get; set; }

        public virtual Item Item { get; set; }


    }
}
