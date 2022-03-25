using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public partial class YarnRecipe
    {
        public YarnRecipe()
        {
            this.KnitYarn = new HashSet<KnitYarn>();
            this.YarnRecipeMix = new HashSet<YarnRecipeMix>();
        }
        public int Id { get; set; }
        public string YarnRecipeCode { get; set; }
        public string YarnRecipeName { get; set; }
        [ForeignKey("YarnBreed")]
        public int? YarnBreedId { get; set; }
        public int? Denier { get; set; }
        public int? YarnDenier { get; set; }
        //Katsayi
        public int? Factor { get; set; }
        //Bukum
        public int? Twist { get; set; }
        public int? TwistDirection { get; set; }
        //Punta
        public int? CenterType { get; set; }
        //Karisim 
        public bool? Mix { get; set; }
        public int? YarnRecipeType { get; set; }
        public decimal? BuyingPrice { get; set; }
        public decimal? SalesPrice { get; set; }
        public string CustomerYarnColorExplanation { get; set; }
        public string Explanation { get; set; }

        public Nullable<int> PlantId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        [ForeignKey("YarnColour")]
        public int? YarnColourId { get; set; }

        [ForeignKey("Firm")]
        public int? FirmId { get; set; }
        public string YarnLot { get; set; }

        [ForeignKey("Item")]
        public Nullable<int> ItemId { get; set; }

        [ForeignKey("ForexType")]
        public Nullable<int> ForexTypeId { get; set; }

        public virtual Firm Firm { get; set; }
        public virtual YarnBreed YarnBreed { get; set; }
        public virtual YarnColour YarnColour { get; set; }
        public virtual YarnColour CustomerYarnColour { get; set; }
        public virtual ForexType ForexType { get; set; }
        public virtual Item Item { get; set; }

        [InverseProperty("YarnRecipe")]
        public virtual ICollection<KnitYarn> KnitYarn { get; set; }

        [InverseProperty("YarnRecipe")]
        public virtual ICollection<YarnRecipeMix> YarnRecipeMix { get; set; }
    }
}
