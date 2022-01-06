using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context.Models
{
   public partial class VehicleInsuranceType
    {
        public VehicleInsuranceType()
        {
            this.VehicleInsurance = new HashSet<VehicleInsurance>();

        }
        public int Id { get; set; }
        public string VehicleInsuranceTypeCode { get; set; }
        public string VehicleInsuranceTypeName { get; set; }
        [ForeignKey("UnitType")]
        public int? PeriyodUnitid { get; set; }
        //Zorunlu
        public bool? Compulsory { get; set; }

        public Nullable<int> PlantId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        public virtual UnitType UnitType { get; set; }

        [InverseProperty("VehicleInsuranceType")]
        public virtual ICollection<VehicleInsurance> VehicleInsurance { get; set; }
    }
}
