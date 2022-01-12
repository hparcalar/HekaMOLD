using System;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
    public class VehicleCareModel
    {
        public int Id { get; set; }
        public int? VehicleTireType { get; set; }
        public string SeriNo { get; set; }
        //Yon tip kod
        public int? DirectionType { get; set; }
        //Ebat bilgi
        public string DimensionsInfo { get; set; }
        //Montaj tarih
        public Nullable<DateTime> MontageDate { get; set; }

        public int? VehicleCareTypeId { get; set; }

        public int? VehicleId { get; set; }

        public int? OperationFirmId { get; set; }

        public int? KmHour { get; set; }
        public int? KmHourLimit { get; set; }
        public bool? KmHourControl { get; set; }
        public int? UnitId { get; set; }
        //iptal
        public bool? Invalidation { get; set; }
        //Toplam
        public decimal? Amount { get; set; }

    }
}
