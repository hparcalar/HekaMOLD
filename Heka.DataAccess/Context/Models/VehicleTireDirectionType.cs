
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context.Models
{
   public partial class VehicleTireDirectionType
    {
        public VehicleTireDirectionType()
        {
            this.VehicleTire = new HashSet<VehicleTire>();
        }
        public int Id { get; set; }
        public string VehicleTireDirectionTypeCode { get; set; }
        public string VehicleTireDirectionTypeName { get; set; }

        public Nullable<int> PlantId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        [InverseProperty("VehicleTireDirectionType")]
        public virtual ICollection<VehicleTire> VehicleTire { get; set; }
    }
}
