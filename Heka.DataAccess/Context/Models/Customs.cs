using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context.Models
{
    public partial class Customs
    {
        public Customs()
        {
            this.ItemOrderByEntries = new HashSet<ItemOrder>();
            this.ItemOrderByExits = new HashSet<ItemOrder>();
        }
        public int Id { get; set; }
        public string CustomsCode { get; set; }
        public string CustomsName { get; set; }

        public Nullable<int> PlantId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        [InverseProperty("CustomsEntry")]
        public virtual ICollection<ItemOrder> ItemOrderByEntries { get; set; }

        [InverseProperty("CustomsExit")]
        public virtual ICollection<ItemOrder> ItemOrderByExits { get; set; }

        //[InverseProperty("Customs")]
        //public virtual ICollection<ItemOrder> ItemOrder2 { get; set; }
    }
}
