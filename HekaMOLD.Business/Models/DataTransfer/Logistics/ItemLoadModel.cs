using HekaMOLD.Business.Base;
using System;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
    public class ItemLoadModel : IDataObject
    {
        public int Id { get; set; }
        public string LoadCode { get; set; }
        public DateTime? LoadDate { get; set; }
        public DateTime? DischargeDate { get; set; }
        public int? LoadStatusType { get; set; }
        public decimal? OveralWeight { get; set; }
        public decimal? OveralVolume { get; set; }
        public decimal? OveralLadametre { get; set; }
        public Nullable<decimal> OverallTotal { get; set; }

        public decimal? CalculationTypePrice { get; set; }
        public string OrderNo { get; set; }
        public string DocumentNo { get; set; }
        public int? OrderUploadType { get; set; }
        public int? OrderUploadPointType { get; set; }
        public int? OrderTransactionDirectionType { get; set; }
        public int? OrderCalculationType { get; set; }
        public DateTime? LoadOutDate { get; set; }
        public string Explanation { get; set; }
        public string ShipperFirmExplanation { get; set; }
        public string BuyerFirmExplanation { get; set; }
        public Nullable<int> CustomerFirmId { get; set; }
        public int? ShipperFirmId { get; set; }
        public int? BuyerFirmId { get; set; }
        public int? EntryCustomsId { get; set; }
        public int? ExitCustomsId { get; set; }
        public DateTime? DateOfNeed { get; set; }
        public int? OveralQuantity { get; set; }

        public int? UserAuthorId { get; set; }
        public DateTime? LoadingDate { get; set; }
        public int? InvoiceId { get; set; }
        public int? InvoiceStatus { get; set; }
        //Navlun bedeli
        public decimal? InvoiceFreightPrice { get; set; }
        public string CrmNo { get; set; }
        public int? CrmStatus { get; set; }
        public Nullable<int> ShipperCityId { get; set; }
        public int? BuyerCityId { get; set; }
        public Nullable<int> ShipperCountryId { get; set; }
        public int? BuyerCountryId { get; set; }
        public Nullable<int> ItemOrderId { get; set; }
        public Nullable<int> PlantId { get; set; }
        public Nullable<int> ForexTypeId { get; set; }
        public ItemLoadDetailModel[] Details { get; set; }
        //public Nullable<int> CreatedUserId { get; set; }

        #region VISUAL ELEMENTS
        public string OrderDateStr { get; set; }
        public string LoadDateStr { get; set; }
        public string LoadOutDateStr { get; set; }
        public string DateOfNeedStr { get; set; }
        public string DischargeDateStr { get; set; }
        public string LoadStatusTypeStr { get; set; }
        public string CustomerFirmCode { get; set; }
        public string CustomerFirmName { get; set; }
        public string EntryCustomsCode { get; set; }
        public string EntryCustomsName { get; set; }
        public string ExitCustomsCode { get; set; }
        public string ExitCustomsName { get; set; }
        public string ShipperFirmCode { get; set; }
        public string ShipperFirmName { get; set; }
        public string BuyerFirmCode { get; set; }
        public string BuyerFirmName { get; set; }
        public string OrderTransactionDirectionTypeStr { get; set; }
        public string OrderUploadTypeStr { get; set; }
        public string OrderUploadPointTypeStr { get; set; }
        public string OrderCalculationTypeStr { get; set; }
        public string OrderCreatUser { get; set; }
        public string UserAuthorName { get; set; }
        public string LoadingDateStr { get; set; }
        public string ShipperCityName { get; set; }
        public string BuyerCityName { get; set; }
        public string ShipperCountryName { get; set; }
        public string BuyerCountryName { get; set; }
        public string ForexTypeCode { get; set; }
        #endregion
    }
}
