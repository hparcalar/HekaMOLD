using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heka.DataAccess.Context.Models
{
   public partial class ItemQualityType
    {
        public ItemQualityType()
        {
            this.Item = new HashSet<Item>();
        }
        public int Id { get; set; }
        public string ItemQualityTypeCode { get; set; }
        public string ItemQualityTypeName { get; set; }

        [InverseProperty("ItemQualityType")]
        public virtual ICollection<Item> Item { get; set; }

    }
}
