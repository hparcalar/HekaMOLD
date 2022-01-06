using Heka.DataAccess.Context.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
  public partial  class VehicleType
    {
        public VehicleType()
        {
            this.Vehicle = new HashSet<Vehicle>();
        }
        public int Id { get; set; }
        public string VehicleTypeCode { get; set; }
        public string VehicleTypeName { get; set; }


        public Nullable<int> PlantId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        [InverseProperty("VehicleType")]
        public virtual ICollection<Vehicle> Vehicle { get; set; }
    }
}
