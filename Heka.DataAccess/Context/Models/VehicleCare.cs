using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public partial class VehicleCare
    {
        public VehicleCare()
        {
        }
        public int Id { get; set; }
        [ForeignKey("VehicleCareType")]
        public int? VehicleCareTypeId { get; set; }

        [ForeignKey("Vehicle")]
        public int? VehicleId { get; set; }

        [ForeignKey("Firm")]
        public int? OperationFirmId { get; set; }
        public int? PersonnelId { get; set; }
        public Nullable<DateTime> CareDate { get; set; }
        public int? KmHour { get; set; }
        [ForeignKey("ForexType")]
        public int? ForexTypeId { get; set; }
        //iptal
        public bool? Invalidation { get; set; }
        //Toplam
        public decimal? Amount { get; set; }
        public string DocumentNo { get; set; }
        public string Explanation { get; set; }

        public Nullable<int> PlantId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        public virtual Firm Firm { get; set; }
        public virtual ForexType ForexType { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public virtual VehicleCareType VehicleCareType { get; set; }

    }
}
