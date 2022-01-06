using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heka.DataAccess.Context.Models
{
   public partial class YarnRecipeType
    {
        public YarnRecipeType()
        {
            this.YarnRecipe = new HashSet<YarnRecipe>();

        }
        public int Id { get; set; }
        public string YarnTypeCode { get; set; }
        public string YarnTypeName { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        [InverseProperty("YarnRecipeType")]
        public virtual ICollection<YarnRecipe> YarnRecipe { get; set; }
    }
}
