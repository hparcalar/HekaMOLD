using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context.Models
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
        public int? UnitTypeId { get; set; }
        [ForeignKey("ForexType")]
        public int? ForexTypeId { get; set; }
        public decimal? Amount { get; set; }
        public string Explanation { get; set; }
        public string DocumentNo { get; set; }

        public Nullable<int> PlantId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        public virtual UnitType UnitType { get; set; }
        public virtual VehicleInsuranceType VehicleInsuranceType { get; set; }
        public virtual Firm Firm { get; set; }
        public virtual ForexType ForexType { get; set; }
        public virtual Vehicle Vehicle { get; set; }

    }
}
