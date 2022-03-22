using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public partial class VoyageCostDetail
    {
        public VoyageCostDetail()
        {
            this.DriverAccountDetail = new HashSet<DriverAccountDetail>();
        }
        public int Id { get; set; }

        [ForeignKey("VoyageCost")]
        public int? VoyageCostId { get; set; }
        [ForeignKey("Driver")]
        public int? DriverId { get; set; }
        [ForeignKey("Country")]
        public int? CountryId { get; set; }
        [ForeignKey("CostCategory")]
        public int? CostCategoryId { get; set; }
        public DateTime? OperationDate { get; set; }
        public int? Quantity { get; set; }
        [ForeignKey("UnitType")]
        public int? UnitTypeId { get; set; }
        public decimal? OverallTotal { get; set; }
        [ForeignKey("ForexType")]
        public int? ForexTypeId { get; set; }
        [ForeignKey("Vehicle")]
        public int? TowingVehicleId { get; set; }
        public int? KmHour { get; set; }
        public int? PayType { get; set; }
        public int ActionType { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        public virtual VoyageCost VoyageCost { get; set; }
        public virtual Driver Driver { get; set; }
        public virtual Country Country { get; set; }
        public virtual CostCategory CostCategory { get; set; }
        public virtual UnitType UnitType { get; set; }
        public virtual ForexType ForexType { get; set; }
        public virtual Vehicle Vehicle { get; set; }

        [InverseProperty("VoyageCostDetail")]
        public virtual ICollection<DriverAccountDetail> DriverAccountDetail { get; set; }
    }
}
