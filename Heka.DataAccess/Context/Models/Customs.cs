using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public partial class Customs
    {
        public Customs()
        {
            this.ItemOrderByEntries = new HashSet<ItemOrder>();
            this.ItemOrderByExits = new HashSet<ItemOrder>();
            this.ItemLoadByEntries = new HashSet<ItemLoad>();
            this.ItemLoadByExits = new HashSet<ItemLoad>();
            this.VoyageByLoadCustoms = new HashSet<Voyage>();
            this.VoyageByDischargeCustoms = new HashSet<Voyage>();
        }
        public int Id { get; set; }
        public string CustomsCode { get; set; }
        public string CustomsName { get; set; }
        [ForeignKey("City")]
        public int? CityId { get; set; }


        public Nullable<int> PlantId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        public virtual City City { get; set; }

        [InverseProperty("CustomsEntry")]
        public virtual ICollection<ItemOrder> ItemOrderByEntries { get; set; }

        [InverseProperty("CustomsExit")]
        public virtual ICollection<ItemOrder> ItemOrderByExits { get; set; }

        [InverseProperty("CustomsEntry")]
        public virtual ICollection<ItemLoad> ItemLoadByEntries { get; set; }

        [InverseProperty("CustomsExit")]
        public virtual ICollection<ItemLoad> ItemLoadByExits { get; set; }

        [InverseProperty("LoadCustoms")]
        public virtual ICollection<Voyage> VoyageByLoadCustoms { get; set; }

        [InverseProperty("DischargeCustoms")]
        public virtual ICollection<Voyage> VoyageByDischargeCustoms { get; set; }


    }
}
