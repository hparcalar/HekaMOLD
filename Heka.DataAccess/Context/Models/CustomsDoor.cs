using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context.Models
{
    public partial class CustomsDoor
    {
        public CustomsDoor()
        {
            this.VoyageByCustomsDoorEntry = new HashSet<Voyage>();
            this.VoyageByCustomsDoorExit = new HashSet<Voyage>();

        }
        public int Id { get; set; }
        public string CustomsDoorCode { get; set; }
        public string CustomsDoorName { get; set; }

        public Nullable<int> PlantId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        [InverseProperty("CustomsDoorEntry")]
        public virtual ICollection<Voyage> VoyageByCustomsDoorEntry { get; set; }

        [InverseProperty("CustomsDoorExit")]
        public virtual ICollection<Voyage> VoyageByCustomsDoorExit { get; set; }
    }
}
