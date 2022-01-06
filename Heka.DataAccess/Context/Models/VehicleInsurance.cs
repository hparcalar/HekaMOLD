using Heka.DataAccess.Context.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
   public partial class VehicleInsurance
    {
        public VehicleInsurance()
        {

        }
        public int Id { get; set; }
        [ForeignKey("VehicleInsuranceType")]
        public int? VehicleInsuranceTypeId { get; set; }
        [ForeignKey("Vehicle")]
        public int? VehicleId { get; set; }
        [ForeignKey("Firm")]
        public int? OperationFirmId { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public int? KmHour { get; set; }
        public int? PersonnelId { get; set; }
        [ForeignKey("UnitType")]
        public int? UnitId { get; set; }
        public decimal? Amount { get; set; }

        public Nullable<int> PlantId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        public virtual VehicleInsuranceType VehicleInsuranceType { get; set; }
        public virtual Firm Firm { get; set; }
        public virtual UnitType UnitType { get; set; }
        public virtual Vehicle Vehicle { get; set; }



    }
}
