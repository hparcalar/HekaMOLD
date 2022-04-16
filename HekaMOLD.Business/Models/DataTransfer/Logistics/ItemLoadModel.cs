﻿using HekaMOLD.Business.Base;
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
        public DateTime? ScheduledUploadDate { get; set; }
        public string Explanation { get; set; }
        public string ShipperFirmExplanation { get; set; }
        public string BuyerFirmExplanation { get; set; }
        public string BuyerFirmAddress { get; set; }
        public string ShipperFirmAddress { get; set; }
        public string CmrBuyerFirmAddress { get; set; }
        public string CmrShipperFirmAddress { get; set; }
        public Nullable<int> CustomerFirmId { get; set; }
        public int? ShipperFirmId { get; set; }
        public int? BuyerFirmId { get; set; }
        public int? CmrShipperFirmId { get; set; }
        public int? CmrBuyerFirmId { get; set; }
        public Nullable<int> VehicleTraillerId { get; set; }
        public int? TowinfVehicleId { get; set; }
        public int? VoyageId { get; set; }
        public bool? VoyageConverted { get; set; }
        //Hazır olma Tarihi
        public DateTime? ReadinessDate { get; set; }
        //Müşteriden teslim Alınış Tarihi
        public DateTime? DeliveryFromCustomerDate { get; set; }
        //İstenen Varış Tarihi
        public DateTime? IntendedArrivalDate { get; set; }
        public int? FirmCustomsArrivalId { get; set; }
        public int? FirmCustomsExitId { get; set; }
        public int? VoyageStartCityId { get; set; }
        public int? VoyageEndCityId { get; set; }
        public int? VoyageStartCountryId { get; set; }
        public int? VoyageEndCountryId { get; set; }
        public string CustomsExplanation { get; set; }
        public string T1T2No { get; set; }
        //T Kapanış Tarihi
        public DateTime? TClosingDate { get; set; }
        public bool? HasCmrDeliveryed { get; set; }
        public decimal? ItemPrice { get; set; }
        public int? TrailerType { get; set; }
        public bool? HasItemInsurance { get; set; }
        public string ItemInsuranceDraftNo { get; set; }
        //Tehlikeli Madde var
        public bool? HasItemDangerous { get; set; }
        //Cmr Müşteri Teslim Tarihi
        public DateTime? CmrCustomerDeliveryDate { get; set; }
        //Depoya Getiren Araç
        public string BringingToWarehousePlate { get; set; }
        public DateTime? BringingToWarehouseDate { get; set; }
        public DateTime? LoadExitDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string DeclarationX1No { get; set; }
        public DateTime? VoyageExitDate { get; set; }
        public DateTime? VoyageEndDate { get; set; }
        public string VoyageStartAddress { get; set; }
        public string VoyageEndAddress { get; set; }
        public int? LoadingLineNo { get; set; }
        public int? DischargeLineNo { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public Nullable<System.DateTime> KapikuleEntryDate { get; set; }

        public Nullable<System.DateTime> KapikuleExitDate { get; set; }
        public int? EntryCustomsId { get; set; }
        public int? ExitCustomsId { get; set; }
        public DateTime? DateOfNeed { get; set; }
        public int? OveralQuantity { get; set; }
        public string LoadWeek { get; set; }
        public string VoyageCode { get; set; }

        public Nullable<int> DriverId { get; set; }
        public int? UserAuthorId { get; set; }
        public DateTime? LoadingDate { get; set; }
        public int? InvoiceId { get; set; }
        public int? InvoiceStatus { get; set; }
        //Navlun bedeli
        public decimal? InvoiceFreightPrice { get; set; }
        public string CmrNo { get; set; }
        public int? CmrStatus { get; set; }
        public Nullable<int> ShipperCityId { get; set; }
        public int? BuyerCityId { get; set; }
        public Nullable<int> CmrShipperCityId { get; set; }
        public int? CmrBuyerCityId { get; set; }
        public Nullable<int> ShipperCountryId { get; set; }
        public int? BuyerCountryId { get; set; }
        public Nullable<int> CmrShipperCountryId { get; set; }
        public int? CmrBuyerCountryId { get; set; }
        public int? ManufacturerFirmId { get; set; }
        public int? ReelOwnerFirmId { get; set; }
        public Nullable<int> ItemOrderId { get; set; }
        public Nullable<int> PlantId { get; set; }
        public Nullable<int> ForexTypeId { get; set; }
        public Nullable<int> VoyageCreatedUserId { get; set; }
        public ItemLoadDetailModel[] Details { get; set; }
        public LoadInvoiceModel[] Invoices { get; set; }


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
        public string CmrShipperFirmCode { get; set; }
        public string CmrShipperFirmName { get; set; }
        public string CmrBuyerFirmCode { get; set; }
        public string CmrBuyerFirmName { get; set; }
        public string OrderTransactionDirectionTypeStr { get; set; }
        public string OrderUploadTypeStr { get; set; }
        public string OrderUploadPointTypeStr { get; set; }
        public string OrderCalculationTypeStr { get; set; }
        public string OrderCreatUser { get; set; }
        public string UserAuthorName { get; set; }
        public string LoadingDateStr { get; set; }
        public string ShipperCityName { get; set; }
        public string BuyerCityName { get; set; }
        public string CmrShipperCityName { get; set; }
        public string CmrBuyerCityName { get; set; }
        public string ShipperCountryName { get; set; }
        public string BuyerCountryName { get; set; }
        public string CmrShipperCountryName { get; set; }
        public string CmrBuyerCountryName { get; set; }
        public string CreatedUserName { get; set; }

        public string ForexTypeCode { get; set; }
        public string ReadinessDateStr { get; set; }
        public string DeliveryFromCustomerDateStr { get; set; }
        public string BringingToWarehouseDateStr { get; set; }
        public string LoadExitDateStr { get; set; }
        public string DeliveryDateStr { get; set; }
        public string IntendedArrivalDateStr { get; set; }
        public string TClosingDateStr { get; set; }
        public string CrmDeliveryDateStr { get; set; }
        public string CmrCustomerDeliveryDateStr { get; set; }
        public string FirmArrivalCustomsCode { get; set; }
        public string FirmArrivalCustomsName { get; set; }
        public string FirmCustomsExitCode { get; set; }
        public string FirmCustomsExitName { get; set; }
        public string ScheduledUploadDateStr { get; set; }
        public string ScheduledUploadWeek { get; set; }
        public string LoadAddress { get; set; }
        public string DischargeAddress { get; set; }
        public string Plate { get; set; }
        public string Mark { get; set; }
        public string Versiyon { get; set; }
        public string DriverNameAndSurname { get; set; }
        public string VoyageCreatedUserName { get; set; }
        public string ManufacturerFirmCode { get; set; }
        public string ManufacturerFirmName { get; set; }
        public string ReelOwnerFirmCode { get; set; }
        public string ReelOwnerFirmName { get; set; }
        public string TowinfVehiclePlate { get; set; }
        public string TowinfVehicleMarkAndVersiyon { get; set; }
        public string VoyageExitDateStr { get; set; }
        public string VoyageEndDateStr { get; set; }
        public string KapikuleExitDateStr { get; set; }
        public string KapikuleEntryDateStr { get; set; }
        public string VoyageStartCityPostCode { get; set; }
        public string VoyageStartCityName { get; set; }
        public string VoyageEndCityPostCode { get; set; }
        public string VoyageEndtCityName { get; set; }
        public string VoyageStartCountryPostCode { get; set; }
        public string VoyageStartCountryName { get; set; }
        public string VoyageEndCountryPostCode { get; set; }
        public string VoyageEndtCountryName { get; set; }
        public string VehicleTraillerPlate { get; set; }
        public string VehicleTraillerMarkAndModel { get; set; }
        public string VoyageConvertedStr { get; set; }
        #endregion
    }
}