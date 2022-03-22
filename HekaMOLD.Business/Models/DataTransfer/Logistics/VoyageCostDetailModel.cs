using HekaMOLD.Business.Base;
using System;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
    public class VoyageCostDetailModel : IDataObject
    {
        public int Id { get; set; }

        public int? VoyageCostId { get; set; }
        public int? DriverId { get; set; }
        public int? CountryId { get; set; }
        public int? CostCategoryId { get; set; }
        public DateTime? OperationDate { get; set; }
        public int? Quantity { get; set; }
        public int? UnitTypeId { get; set; }
        public decimal? OverallTotal { get; set; }
        public int? ForexTypeId { get; set; }
        public int? PayType { get; set; }
        public int? TowingVehicleId { get; set; }
        public int? KmHour { get; set; }
        public int ActionType { get; set; }

        #region VISUAL ELEMENTS
        public string DriverNameAndSurName { get; set; }
        public string CountryName { get; set; }
        public string CostCategoryName { get; set; }
        public string OperationDateStr { get; set; }
        public string UnitCode { get; set; }
        public string ForexTypeCode { get; set; }
        public string PayTypeStr { get; set; }
        public string TowingVehiclePlate { get; set; }
        public string TowingVehicleMarkAndVersiyon { get; set; }
        public string VoyageCode { get; set; }
        public bool NewDetail { get; set; }
        #endregion
    }
}
