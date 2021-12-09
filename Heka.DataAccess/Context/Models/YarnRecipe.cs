using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heka.DataAccess.Context.Models
{
    public partial class YarnRecipe
    {
        public YarnRecipe()
        {

        }
        public int Id { get; set; }

        [ForeignKey("Item")]
        public int? ItemId { get; set; }

        public string YarnRecipeCode { get; set; }
        public string YarnRecipeName { get; set; }

        [ForeignKey("Firm")]
        public int? FirmId { get; set; }
        public int YarnRecipeTypeId { get; set; }
        public int? Denier { get; set; }
        public int? ReportWireCount { get; set; }
        public int? MeterWireCount { get; set; }
        public decimal? Gramaj { get; set; }

        public virtual Firm Firm { get; set; }
        public virtual Item Item { get; set; }

    }
}
