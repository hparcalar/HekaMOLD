using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heka.DataAccess.Context
{
   public partial class DriverAccountDetail
    {
        public int Id { get; set; }
        [ForeignKey("Driver")]
        public int DriverId { get; set; }
        [ForeignKey("DriverAccount")]
        public int DriverAccountId { get; set; }
        [ForeignKey("ForexType")]
        public int ForexTypeId { get; set; }
        public decimal OverallTotal { get; set; }
        public int ActionType { get; set; }
        [ForeignKey("VoyageCostDetail")]
        public int? VoyageCostDetailId { get; set; }
        [ForeignKey("Voyage")]
        public int? VoyageId { get; set; }
        [ForeignKey("CostCategory")]
        public int? CostCategoryId { get; set; }
        [ForeignKey("TowingVehicle")]
        public int? TowingVehicleId { get; set; }
        [ForeignKey("Country")]
        public int? CountryId { get; set; }
        [ForeignKey("UnitType")]
        public int? UnitTypeId { get; set; }
        public int? KmHour { get; set; }
        public int? Quantity { get; set; }
        public DateTime? OperationDate { get; set; }
        public string DocumentNo { get; set; }
        public string Explanation { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        public virtual Driver Driver { get; set; }
        public virtual DriverAccount DriverAccount { get; set; }
        public virtual ForexType ForexType { get; set; }
        public virtual VoyageCostDetail VoyageCostDetail { get; set; }
        public virtual Voyage Voyage { get; set; }
        public virtual CostCategory CostCategory { get; set; }
        public virtual Vehicle TowingVehicle { get; set; }
        public virtual Country Country { get; set; }
        public virtual UnitType UnitType { get; set; }

    }
}
