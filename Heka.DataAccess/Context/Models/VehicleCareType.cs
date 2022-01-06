using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context.Models
{
   public partial class VehicleCareType
    {
        public VehicleCareType()
        {
            this.VehicleCare = new HashSet<VehicleCare>();

        }
        public int Id { get; set; }
        public string VehicleCareTypeCode { get; set; }
        public string VehicleCareTypeName { get; set; }
        public int? PeriyodUnitCode { get; set; }
        //Zorunlu
        public bool? Compulsory { get; set; }

        public Nullable<int> PlantId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }


        [InverseProperty("VehicleCareType")]
        public virtual ICollection<VehicleCare> VehicleCare { get; set; }
    }
}
