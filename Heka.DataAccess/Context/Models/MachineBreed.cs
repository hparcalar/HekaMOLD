using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context.Models
{
   public partial class MachineBreed
    {
        public MachineBreed()
        {
            this.Machine =new  HashSet<Machine>();
        }
        public int Id { get; set; }
        public string MachineBreedCode { get; set; }
        public string MachineBreedName { get; set; }
        public Nullable<int> PlantId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        [InverseProperty("MachineBreed")]
        public virtual ICollection<Machine> Machine { get; set; }
    }
}
