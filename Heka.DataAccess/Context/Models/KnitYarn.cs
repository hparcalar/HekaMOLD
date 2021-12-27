using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context.Models
{
    public partial class KnitYarn
    {
        public KnitYarn()
        {

        }
        public int Id { get; set; }

        [ForeignKey("YarnRecipe")]
        public int YarnRecipeId { get; set; }

        [ForeignKey("Firm")]
        public int FirmId { get; set; }

        [ForeignKey("Item")]
        public int ItemId { get; set; }

        public int? YarnType { get; set; }

        //Rapor Tel Sayisi
        public int? ReportWireCount { get; set; }

        //Metre Tel Sayisi
        public int? MeterWireCount { get; set; }
        public decimal? Gramaj { get; set; }
        public int? Density { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        public virtual Firm Firm { get; set; }
        public virtual YarnRecipe YarnRecipe { get; set; }
        public virtual Item Item { get; set; }


    }
}
