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
        public bool? Center { get; set; }
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
        public virtual Firm Firm { get; set; }
        public virtual YarnBreed YarnBreed { get; set; }
        public virtual YarnColour YarnColour { get; set; }

        [InverseProperty("YarnRecipe")]
        public virtual ICollection<KnitYarn> KnitYarn { get; set; }

    }
}
