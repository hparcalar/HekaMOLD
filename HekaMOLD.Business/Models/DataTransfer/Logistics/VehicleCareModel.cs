using HekaMOLD.Business.Base;
using System;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
    public class VehicleCareModel:IDataObject
    {
        public int Id { get; set; }
        public int? VehicleCareTypeId { get; set; }
        public int? VehicleId { get; set; }
        public int? OperationFirmId { get; set; }
        public int? PersonnelId { get; set; }
        public Nullable<DateTime> CareDate { get; set; }
        public int? KmHour { get; set; }
        public int? ForexTypeId { get; set; }
        //iptal
        public bool? Invalidation { get; set; }
        //Toplam
        public decimal? Amount { get; set; }
        public string Explanation { get; set; }

        #region VISUAL ELEMENTS
        public string VehicleCareTypeCode { get; set; }
        public string VehicleCareTypeName { get; set; }
        public string Plate { get; set; }
        public string FirmCode { get; set; }
        public string FirmName { get; set; }
        public string ForexTypeCode { get; set; }
        #endregion
    }
}
