using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public partial class ItemLoad
    {
        public ItemLoad()
        {
            this.ItemLoadDetail = new HashSet<ItemLoadDetail>();
            this.ItemLoadCost = new HashSet<ItemLoadCost>();
            this.VoyageDetail = new HashSet<VoyageDetail>();
            this.LoadInvoice = new HashSet<LoadInvoice>();
        }
        public int Id { get; set; }
        public string LoadCode { get; set; }
        public string OrderNo { get; set; }
        public Nullable<System.DateTime> LoadDate { get; set; }
        public Nullable<System.DateTime> DischargeDate { get; set; }
        public int? LoadStatusType { get; set; }
        public decimal? OveralWeight { get; set; }
        public decimal? OveralVolume { get; set; }
        public decimal? OveralLadametre { get; set; }
        public Nullable<decimal> OverallTotal { get; set; }
        public decimal? CalculationTypePrice { get; set; }

        public string DocumentNo { get; set; }
        public int? OrderUploadType { get; set; }
        public int? OrderUploadPointType { get; set; }
        public int? OrderTransactionDirectionType { get; set; }
        public int? OrderCalculationType { get; set; }
        public DateTime? LoadOutDate { get; set; }
        public DateTime? ScheduledUploadDate { get; set; }
        public Nullable<System.DateTime> DateOfNeed { get; set; }
        public int? OveralQuantity { get; set; }
        public string LoadWeek { get; set; }

        public int? InvoiceStatus { get; set; }
        //Navlun bedeli
        public decimal? InvoiceFreightPrice { get; set; }
        public string CmrNo { get; set; }
        public int? CmrStatus { get; set; }

        public string Explanation { get; set; }
        public string ShipperFirmExplanation { get; set; }
        public string BuyerFirmExplanation { get; set; }
        public string BuyerFirmAddress { get; set; }
        public string ShipperFirmAddress { get; set; }
        public string CmrBuyerFirmAddress { get; set; }
        public string CmrShipperFirmAddress { get; set; }
        //Hazır olma Tarihi
        public DateTime? ReadinessDate { get; set; }
        //Müşteriden teslim Alınış Tarihi
        public DateTime? DeliveryFromCustomerDate { get; set; }
        //İstenen Varış Tarihi
        public DateTime? IntendedArrivalDate { get; set; }
        public string CustomsExplanation { get; set; }
        public string T1T2No { get; set; }
        //T Kapanış Tarihi
        public DateTime? TClosingDate { get; set; }
        public bool? HasCmrDeliveryed { get; set; }
        public decimal? ItemPrice { get; set; }
        public int? TrailerType { get; set; }
        public bool? HasItemInsurance { get; set; }
        public string ItemInsuranceDraftNo { get; set; }
        public string VoyageCode { get; set; }
        public int? VoyageId { get; set; }
        public bool? VoyageConverted { get; set; }
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
        public Nullable<System.DateTime> T1T2StartDate { get; set; }
        public Nullable<System.DateTime> KapikuleExitDate { get; set; }
        //Faturalastirildi
        public bool? Billed { get; set; }
        public string InvoiceDocumentNo { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }

        [ForeignKey("CityShipper")]
        public Nullable<int> ShipperCityId { get; set; }

        [ForeignKey("CityBuyer")]
        public int? BuyerCityId { get; set; }

        [ForeignKey("CityCmrShipper")]
        public Nullable<int> CmrShipperCityId { get; set; }

        [ForeignKey("CityCmrBuyer")]
        public int? CmrBuyerCityId { get; set; }

        [ForeignKey("CountryShipper")]
        public Nullable<int> ShipperCountryId { get; set; }

        [ForeignKey("CountryBuyer")]
        public int? BuyerCountryId { get; set; }

        [ForeignKey("CountryCmrShipper")]
        public Nullable<int> CmrShipperCountryId { get; set; }

        [ForeignKey("CountryCmrBuyer")]
        public int? CmrBuyerCountryId { get; set; }

        [ForeignKey("FirmCustomer")]
        public Nullable<int> CustomerFirmId { get; set; }

        [ForeignKey("FirmShipper")]
        public int? ShipperFirmId { get; set; }

        [ForeignKey("FirmBuyer")]
        public int? BuyerFirmId { get; set; }

        [ForeignKey("FirmCmrShipper")]
        public int? CmrShipperFirmId { get; set; }

        [ForeignKey("FirmCmrBuyer")]
        public int? CmrBuyerFirmId { get; set; }

        [ForeignKey("FirmManufacturer")]
        public int? ManufacturerFirmId { get; set; }

        [ForeignKey("FirmAgent")]
        public int? AgentFirmId { get; set; }

        [ForeignKey("FirmReelOwner")]
        public int? ReelOwnerFirmId { get; set; }

        [ForeignKey("CustomsEntry")]
        public int? EntryCustomsId { get; set; }

        [ForeignKey("CustomsExit")]
        public int? ExitCustomsId { get; set; }

        [ForeignKey("ItemOrder")]
        public Nullable<int> ItemOrderId { get; set; }

        [ForeignKey("Plant")]
        public Nullable<int> PlantId { get; set; }

        [ForeignKey("Driver")]
        public Nullable<int> DriverId { get; set; }

        [ForeignKey("UserVoyage")]
        public Nullable<int> VoyageCreatedUserId { get; set; }

        [ForeignKey("UserAuthor")]
        public int? UserAuthorId { get; set; }
        public DateTime? LoadingDate { get; set; }

        [ForeignKey("Invoice")]
        public int? InvoiceId { get; set; }

        [ForeignKey("ForexType")]
        public Nullable<int> ForexTypeId { get; set; }

        [ForeignKey("Vehicle")]
        public Nullable<int> VehicleTraillerId { get; set; }

        [ForeignKey("TowinfVehicle")]
        public int? TowinfVehicleId { get; set; }

        [ForeignKey("FirmCustomsArrival")]
        public int? FirmCustomsArrivalId { get; set; }

        [ForeignKey("FirmCustomsExit")]
        public int? FirmCustomsExitId { get; set; }

        [ForeignKey("VoyageStartCity")]
        public int? VoyageStartCityId { get; set; }

        [ForeignKey("VoyageEndCity")]
        public int? VoyageEndCityId { get; set; }

        [ForeignKey("VoyageStartCountry")]
        public int? VoyageStartCountryId { get; set; }

        [ForeignKey("VoyageEndCountry")]
        public int? VoyageEndCountryId { get; set; }

        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        public virtual Plant Plant { get; set; }
        public virtual User User { get; set; }
        public virtual User UserAuthor { get; set; }
        public virtual ItemOrder ItemOrder { get; set; }
        public virtual Firm FirmShipper { get; set; }
        public virtual Firm FirmBuyer { get; set; }
        public virtual Firm FirmCustomer { get; set; }
        public virtual Firm FirmCmrBuyer { get; set; }
        public virtual Firm FirmCmrShipper { get; set; }
        public virtual Firm FirmAgent { get; set; }
        public virtual Customs CustomsEntry { get; set; }
        public virtual Customs CustomsExit { get; set; }
        public virtual Firm FirmCustomsArrival { get; set; }
        public virtual Firm FirmCustomsExit { get; set; }
        public virtual Invoice Invoice { get; set; }
        public virtual Country CountryShipper { get; set; }
        public virtual Country CountryBuyer { get; set; }
        public virtual Country CountryCmrShipper { get; set; }
        public virtual Country CountryCmrBuyer { get; set; }
        public virtual City CityShipper { get; set; }
        public virtual City CityBuyer { get; set; }
        public virtual City CityCmrShipper { get; set; }
        public virtual City CityCmrBuyer { get; set; }
        public virtual ForexType ForexType { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public virtual Driver Driver { get; set; }
        public virtual User UserVoyage { get; set; }
        public virtual Firm FirmManufacturer { get; set; }
        public virtual Firm FirmReelOwner { get; set; }
        public virtual Vehicle TowinfVehicle { get; set; }
        public virtual City VoyageStartCity { get; set; }
        public virtual City VoyageEndCity { get; set; }
        public virtual Country VoyageStartCountry { get; set; }
        public virtual Country VoyageEndCountry { get; set; }

        [InverseProperty("ItemLoad")]
        public virtual ICollection<ItemLoadDetail> ItemLoadDetail { get; set; }

        [InverseProperty("ItemLoad")]
        public virtual ICollection<ItemLoadCost> ItemLoadCost { get; set; }

        [InverseProperty("ItemLoad")]
        public virtual ICollection<VoyageDetail> VoyageDetail { get; set; }

        [InverseProperty("ItemLoad")]
        public virtual ICollection<LoadInvoice> LoadInvoice { get; set; }
    }
}