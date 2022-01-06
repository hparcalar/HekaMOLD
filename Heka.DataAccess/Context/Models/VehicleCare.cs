using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context.Models
{
   public partial class VehicleCare
    {
        public VehicleCare()
        {
        }
        public int Id { get; set; }

        public int? VehiceTireType { get; set; }
        public string SeriNo { get; set; }
        //Yon tip kod
        public int? DirectionType { get; set; }
        //Ebat bilgi
        public string DimensionsInfo { get; set; }
        //Montaj tarih
        public Nullable<DateTime> MontageDate { get; set; }

        [ForeignKey("VehicleCareType")]
        public int? VehicleCareTypeId { get; set; }

        [ForeignKey("Vehicle")]
        public int? VehicleId { get; set; }

        [ForeignKey("Firm")]
        public int? OperationFirmId { get; set; }

        public int? KmHour { get; set; }
        public int? KmHourLimit { get; set; }
        public bool? KmHourControl { get; set; }
        [ForeignKey("UnitType")]
        public int? UnitId { get; set; }

        //iptal
        public bool? Invalidation { get; set; }
        //Toplam
        public decimal? Amount { get; set; }

        public Nullable<int> PlantId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        public virtual Firm Firm { get; set; }
        public virtual UnitType UnitType { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public virtual VehicleCareType VehicleCareType { get; set; }

    }
}
