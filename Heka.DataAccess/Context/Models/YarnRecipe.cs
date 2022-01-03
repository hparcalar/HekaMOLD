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
            this.KnitYarn = new HashSet<KnitYarn>();
            this.YarnRecipeMix = new HashSet<YarnRecipeMix>();
        }
        public int Id { get; set; }
        public string YarnRecipeCode { get; set; }
        public string YarnRecipeName { get; set; }
        [ForeignKey("YarnBreed")]
        public int? YarnBreedId { get; set; }
        public int? Denier { get; set; }
        //Katsayi
        public int? Factor { get; set; }
        //Bukum
        public int? Twist { get; set; }
        //Punta
        public int? CenterType { get; set; }
        //Karisim 
        public bool? Mix { get; set; }
        public int? YarnRecipeType { get; set; }
        public Nullable<int> PlantId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        [ForeignKey("YarnColour")]
        public int? YarnColourId { get; set; }

        [ForeignKey("Firm")]
        public int? FirmId { get; set; }
        public int? YarnLot { get; set; }

        [ForeignKey("Item")]
        public Nullable<int> ItemId { get; set; }
        public virtual Firm Firm { get; set; }
        public virtual YarnBreed YarnBreed { get; set; }
        public virtual YarnColour YarnColour { get; set; }

        [InverseProperty("YarnRecipe")]
        public virtual ICollection<KnitYarn> KnitYarn { get; set; }
        [InverseProperty("YarnRecipe")]
        public virtual ICollection<YarnRecipeMix> YarnRecipeMix { get; set; }
        public virtual Item Item { get; set; }
    }
}
