using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context.Models
{
    public partial class YarnBreed
    {
        public YarnBreed()
        {

        }
        public int Id { get; set; }
        public string YarnBreedCode { get; set; }
        public string YarnBreedName { get; set; }


        [InverseProperty("YarnBreed")]
        public virtual ICollection<YarnRecipe> YarnRecipe { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
