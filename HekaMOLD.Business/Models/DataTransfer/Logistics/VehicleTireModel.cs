using HekaMOLD.Business.Base;
using System;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
   public class VehicleTireModel:IDataObject
    {
        public int Id { get; set; }
        public int? VehicleTireDirectionTypeId { get; set; }
        public int? VehicleTireType { get; set; }
        public string SeriNo { get; set; }
        //Yon tip kod
        public int? DirectionType { get; set; }
        //Ebat bilgi
        public string DimensionsInfo { get; set; }
        //Montaj tarih
        public Nullable<DateTime> MontageDate { get; set; }
        public int? VehicleId { get; set; }
        public int? OperationFirmId { get; set; }
        public int? KmHour { get; set; }
        public int? KmHourLimit { get; set; }
        public int? ForexTypeId { get; set; }
        public string Explanation { get; set; }

        //iptal
        public bool? Invalidation { get; set; }
        public decimal? Amount { get; set; }
        public Nullable<int> PlantId { get; set; }

        #region VISUAL ELEMENTS
        public string VehicleTireTypeStr { get; set; }
        public string DirectionTypeStr { get; set; }
        public string MontageDateStr { get; set; }
        public string Plate { get; set; }
        public string Mark { get; set; }
        public string Model { get; set; }
        public string FirmCode { get; set; }
        public string FirmName { get; set; }
        public string ForexTypeCode { get; set; }
        public string VehicleTireDirectionTypeName { get; set; }
        #endregion
    }
}
