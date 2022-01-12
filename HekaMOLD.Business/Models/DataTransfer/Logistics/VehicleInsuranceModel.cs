using HekaMOLD.Business.Base;
using System;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
   public class VehicleInsuranceModel : IDataObject
    {
        public int Id { get; set; }
        public int? VehicleInsuranceTypeId { get; set; }
        public int? VehicleId { get; set; }
        public int? OperationFirmId { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public int? KmHour { get; set; }
        public int? PersonnelId { get; set; }
        public int? ForexTypeId { get; set; }
        public decimal? Amount { get; set; }
        public string Explanation { get; set; }


        #region VISUAL ELEMENTS
        public string VehicleInsuranceTypeCode { get; set; }
        public string VehicleInsuranceTypeName { get; set; }
        public string Plate { get; set; }
        public string Model { get; set; }
        public string Mark { get; set; }
        public string FirmCode { get; set; }
        public string FirmName { get; set; }
        public string ForexTypeCode { get; set; }
        #endregion
    }
}
