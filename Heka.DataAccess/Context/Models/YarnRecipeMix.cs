using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public partial class YarnRecipeMix
    {
        public YarnRecipeMix()
        {

        }
        public int Id { get; set; }
        [ForeignKey("YarnRecipe")]
        public int YarnRecipeId { get; set; }
        [ForeignKey("YarnBreed")]
        public int? YarnBreedId { get; set; }
        //Yuzde Oran
        public decimal? Percentage { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        public virtual YarnBreed YarnBreed { get; set; }
        public virtual YarnRecipe YarnRecipe { get; set; }
    }
}
