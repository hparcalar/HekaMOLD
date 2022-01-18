using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context.Models
{
    public class Vehicle
    {
        public Vehicle()
        {
            this.VehicleInsurance = new HashSet<VehicleInsurance>();
            this.VehicleCare = new HashSet<VehicleCare>();
            this.VehicleTire = new HashSet<VehicleTire>();

        }
        public int Id { get; set; }
        public string Plate { get; set; }
        public string Mark { get; set; }
        public string Versiyon { get; set; }
        public string ChassisNumber { get; set; }
        public int? VehicleAllocationType { get; set; }

        [ForeignKey("VehicleType")]
        public int? VehicleTypeId { get; set; }

        [ForeignKey("Firm")]
        public int? OwnerFirmId { get; set; }
        public Nullable<DateTime> ContractStartDate { get; set; }
        public Nullable<DateTime> ContractEndDate { get; set; }
        public int? KmHour { get; set; }
        public decimal? Price { get; set; }
        [ForeignKey("UnitType")]
        public int? UnitId { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public decimal? TrailerHeadWeight { get; set; }
        public decimal? LoadCapacity { get; set; }
        public int? TrailerType { get; set; }
        public bool? StatusCode { get; set; }
        public int? CarePeriyot { get; set; }
        // Oransal sınır
        public int? ProportionalLimit { get; set; }
        // Bakım bildirim
        public bool? CareNotification { get; set; }
        //Lastik bildirim
        public bool? TireNotification { get; set; }
        public bool? Approval { get; set; }
        public bool? Invalidation { get; set; }
        public bool? KmHourControl { get; set; }

        public Nullable<int> PlantId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        public virtual VehicleType VehicleType { get; set; }
        public virtual Firm Firm { get; set; }
        public virtual UnitType UnitType { get; set; }

        [InverseProperty("Vehicle")]
        public virtual ICollection<VehicleCare> VehicleCare { get; set; }

        [InverseProperty("Vehicle")]
        public virtual ICollection<VehicleInsurance> VehicleInsurance { get; set; }

        [InverseProperty("Vehicle")]
        public virtual ICollection<VehicleTire> VehicleTire { get; set; }

    }
}
