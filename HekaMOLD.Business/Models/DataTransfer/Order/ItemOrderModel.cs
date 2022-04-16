using HekaMOLD.Business.Base;
using System;

namespace HekaMOLD.Business.Models.DataTransfer.Order
{
    public class ItemOrderModel : IDataObject
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public string DocumentNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? OrderType { get; set; }
        public DateTime? DateOfNeed { get; set; }
        public int? OrderUploadType { get; set; }
        public int? OrderUploadPointType { get; set; }
        public int? OrderTransactionDirectionType { get; set; }
        public Nullable<int> CustomerFirmId { get; set; }
        public int? ReelOwnerFirmId { get; set; }
        //Gönderici Firma
        public int? ShipperFirmId { get; set; }
        public int? BuyerFirmId { get; set; }
        public decimal? OveralWeight { get; set; }
        //Toplam Hacim
        public decimal? OveralVolume { get; set; }
        public decimal? OveralLadametre { get; set; }
        public int? OrderCalculationType { get; set; }
        public DateTime? LoadOutDate { get; set; }
        public DateTime? ScheduledUploadDate { get; set; }
        public decimal? CalculationTypePrice { get; set; }
        public Nullable<int> LoadCityId { get; set; }
        public Nullable<int> DischargeCityId { get; set; }
        public bool? Closed { get; set; }
        public string OrderWeek { get; set; }
        public int? ExitCustomsId { get; set; }
        public int? EntryCustomsId { get; set; }
        public int? InWarehouseId { get; set; }
        public int? OutWarehouseId { get; set; }
        public int? PlantId { get; set; }
        public int? OveralQuantity { get; set; }
        public string Explanation { get; set; }
        public int? OrderStatus { get; set; }
        public int? ItemRequestId { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? TaxPrice { get; set; }
        public decimal? OverallTotal { get; set; }
        public int? SyncStatus { get; set; }
        public int? SyncPointId { get; set; }
        public DateTime? SyncDate { get; set; }
        public int? SyncUserId { get; set; }
        public string SyncKey { get; set; }
        public Nullable<System.DateTime> ArrivalWarehouseDate { get; set; }
        //musteriden alinis Tarihi
        public Nullable<System.DateTime> ReceiptFromCustomerDate { get; set; }
        //Tahmini Yukleme Tarihi
        public Nullable<System.DateTime> EstimatedUplodDate { get; set; }
        //Istenilen Varis Tarihi
        public Nullable<System.DateTime> IntendedArrivalDate { get; set; }
        public Nullable<int> ForexTypeId { get; set; }
        public ItemOrderDetailModel[] Details { get; set; }

        #region VISUAL ELEMENTS
        public string CustomerFirmCode { get; set; }
        public string CustomerFirmName { get; set; }
        public string ReelOwnerFirmName { get; set; }
        public string ShipperFirmCode { get; set; }
        public string ShipperFirmName { get; set; }
        public string BuyerFirmCode { get; set; }
        public string BuyerFirmName { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public string OrderStatusStr { get; set; }
        public string CreatedDateStr { get; set; }
        public string DateOfNeedStr { get; set; }
        public string LoadOutDateStr { get; set; }
        public string OrderDateStr { get; set; }
        public string CreatedUserName { get; set; }
        public string OrderUploadTypeStr { get; set; }
        public string OrderTransactionDirectionTypeStr { get; set; }
        public string OrderUploadPointTypeStr { get; set; }
        public string OrderCalculationTypeStr { get; set; }
        public string EntryCustomsName { get; set; }
        public string ExitCustomsName { get; set; }
        public string LoadCityName { get; set; }
        public string LoadCountryName { get; set; }
        public string LoadPostCode { get; set; }
        public string DischangeCityName { get; set; }
        public string DischangeCountryName { get; set; }
        public string DischangePostCode { get; set; }
        public string ForexTypeCode { get; set; }
        public string OrderDateWeek { get; set; }
        public string ScheduledUploadDateStr { get; set; }
        public string ArrivalWarehouseDateStr { get; set; }
        public string ReceiptFromCustomerDateStr { get; set; }
        public string EstimatedUplodDateStr { get; set; }
        public string IntendedArrivalDateStr { get; set; }
        #endregion
    }
}
