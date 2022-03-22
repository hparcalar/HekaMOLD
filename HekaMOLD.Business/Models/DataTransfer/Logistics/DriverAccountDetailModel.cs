using HekaMOLD.Business.Base;
using System;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
    public class DriverAccountDetailModel : IDataObject
    {
        public int Id { get; set; }
        public int DriverId { get; set; }
        public int DriverAccountId { get; set; }
        public int ForexTypeId { get; set; }
        public decimal OverallTotal { get; set; }
        public int ActionType { get; set; }
        public int? VoyageCostDetailId { get; set; }
        public int? VoyageId { get; set; }
        public int? CostCategoryId { get; set; }
        public int? TowingVehicleId { get; set; }
        public int? CountryId { get; set; }
        public int? UnitTypeId { get; set; }
        public int? KmHour { get; set; }
        public int? Quantity { get; set; }
        public DateTime? OperationDate { get; set; }
        public string DocumentNo { get; set; }
        public string Explanation { get; set; }

        #region VISUAL ELEMENTS

        public string DriverNameAndSurName { get; set; }
        public string ForexTypeCode { get; set; }
        public string ActionTypeStr { get; set; }
        public string VoyageCode { get; set; }
        public string CostCategoryName { get; set; }
        public string TowingVehiclePlate { get; set; }
        public string OperationDateStr { get; set; }
        public string CountryName { get; set; }
        public string UnitCode { get; set; }
        #endregion

    }
}
