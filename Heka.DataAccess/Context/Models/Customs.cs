using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heka.DataAccess.Context.Models
{
   public partial class Customs
    {
        public Customs()
        {
            this.ItemOrder = new HashSet<ItemOrder>();
        }
        public int Id { get; set; }
        public string CustomsCode { get; set; }
        public string CustomsName { get; set; }

        [ForeignKey("Plant")]
        public Nullable<int> PlantId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        [InverseProperty("ItemOrder")]
        public virtual ICollection<ItemOrder> ItemOrder { get; set; }
    }
}
