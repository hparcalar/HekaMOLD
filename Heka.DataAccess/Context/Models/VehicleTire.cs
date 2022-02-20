using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public partial class VehicleTire
    {
        public VehicleTire()
        {
        }
        public int Id { get; set; }
        public int? VehicleTireType { get; set; }
        public string SeriNo { get; set; }
        //Yon tip kod
        [ForeignKey("VehicleTireDirectionType")]
        public int? VehicleTireDirectionTypeId { get; set; }
        //Ebat bilgi
        public string DimensionsInfo { get; set; }
        //Montaj tarih
        public Nullable<DateTime> MontageDate { get; set; }
        [ForeignKey("Vehicle")]
        public int? VehicleId { get; set; }
        [ForeignKey("Firm")]
        public int? OperationFirmId { get; set; }
        public int? KmHour { get; set; }
        public int? KmHourLimit { get; set; }
        [ForeignKey("ForexType")]
        public int? ForexTypeId { get; set; }
        public string DocumentNo { get; set; }
        public string Explanation { get; set; }

        //iptal
        public bool? Invalidation { get; set; }
        public decimal? Amount { get; set; }
        public Nullable<int> PlantId { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        public virtual ForexType ForexType { get; set; }
        public virtual Firm Firm { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public virtual VehicleTireDirectionType VehicleTireDirectionType { get; set; }

    }
}
