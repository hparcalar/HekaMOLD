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
            this.VoyageByExitCustoms = new HashSet<Voyage>();
            this.VoyageByEntryCustoms = new HashSet<Voyage>();
            this.VoyageDetailByEntries = new HashSet<VoyageDetail>();
            this.VoyageDetailByExits = new HashSet<VoyageDetail>();
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

        [InverseProperty("ExitCustoms")]
        public virtual ICollection<Voyage> VoyageByExitCustoms { get; set; }

        [InverseProperty("EntryCustoms")]
        public virtual ICollection<Voyage> VoyageByEntryCustoms { get; set; }

        [InverseProperty("CustomsEntry")]
        public virtual ICollection<VoyageDetail> VoyageDetailByEntries { get; set; }

        [InverseProperty("CustomsExit")]
        public virtual ICollection<VoyageDetail> VoyageDetailByExits { get; set; }
    }
}
