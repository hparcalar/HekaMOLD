﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
   public class WeavingDraft
    {
        public WeavingDraft()
        {
            this.Item = new HashSet<Item>();
            this.Machine = new HashSet<Machine>();
            this.ItemVariant = new HashSet<ItemVariant>();

        }
        public int Id { get; set; }
        public string WeavingDraftCode { get; set; }

        [ForeignKey("MachineBreed")]
        public int? MachineBreedId { get; set; }
        public string NumberOfFramaes { get; set; }
        public string Report { get; set; }
        public int? PlatinumNumber { get; set; }
        public string JakarReport { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
        public Nullable<bool> IsActive { get; set; }

        public virtual MachineBreed MachineBreed { get; set; }

        [InverseProperty("WeavingDraft")]
        public virtual ICollection<Item> Item { get; set; }

        [InverseProperty("WeavingDraft")]
        public virtual ICollection<Machine> Machine { get; set; }

        [InverseProperty("WeavingDraft")]
        public virtual ICollection<ItemVariant> ItemVariant { get; set; }
    }
}
