using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context.Models
{
    public partial class FabricRecipe
    {
        public FabricRecipe()
        {

        }
        public int Id { get; set; }

        [ForeignKey("Item")]
        public int? ItemId { get; set; }

        public string FabricRecipeCode { get; set; }
        public string FabricRecipeName { get; set; }
        public int? Denier { get; set; }

        [ForeignKey("Firm")]
        public int? FirmId { get; set; }

        public string YarnRecipeType { get; set; }

        public int? ReportWireCount { get; set; }
        public int? MeterWireCount { get; set; }
        public decimal? Gramaj { get; set; }

        public virtual Firm Firm { get; set; }
        public virtual Item Item { get; set; }
    }
}
