using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public partial class YarnBreed
    {
        public YarnBreed()
        {
            this.YarnRecipe = new HashSet<YarnRecipe>();
            this.YarnRecipeMix = new HashSet<YarnRecipeMix>();
        }
        public int Id { get; set; }
        public string YarnBreedCode { get; set; }
        public string YarnBreedName { get; set; }


        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }


        [InverseProperty("YarnBreed")]
        public virtual ICollection<YarnRecipe> YarnRecipe { get; set; }

        [InverseProperty("YarnBreed")]
        public virtual ICollection<YarnRecipeMix> YarnRecipeMix { get; set; }
    }
}

