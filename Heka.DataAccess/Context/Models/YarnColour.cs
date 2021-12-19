using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heka.DataAccess.Context.Models
{
   public partial class YarnColour
    {
        public YarnColour()
        {
            this.YarnRecipe = new HashSet<YarnRecipe>();   
        }
        public int Id { get; set; }
        public string YarnColourCode { get; set; }
        public string YarnColourName { get; set; }

        [ForeignKey("YarnColourGroup")]
        public int YarnColourGroupId { get; set; }

        public Nullable<int> PlantId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
        public virtual YarnColourGroup YarnColourGroup { get; set; }

        [InverseProperty("YarnColour")]
        public virtual ICollection<YarnRecipe> YarnRecipe { get; set; }


    }
}
