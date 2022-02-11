using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context.Models
{
    public partial class ItemLoad
    {
        public ItemLoad()
        {
            this.ItemLoadDetail = new HashSet<ItemLoadDetail>();
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
        public Nullable<System.DateTime> DateOfNeed { get; set; }
        public int? OveralQuantity { get; set; }

        [ForeignKey("UserAuthor")]
        public int? UserAuthorId { get; set; }
        public DateTime? LoadingDate { get; set; }
        
        [ForeignKey("Invoice")]
        public int? InvoiceId { get; set; }

        [ForeignKey("ForexType")]
        public Nullable<int> ForexTypeId { get; set; }

        public int? InvoiceStatus { get; set; }
        //Navlun bedeli
        public decimal? InvoiceFreightPrice { get; set; }
        public string CmrNo { get; set; }
        public int? CmrStatus { get; set; }

        public string Explanation { get; set; }
        public string ShipperFirmExplanation { get; set; }
        public string BuyerFirmExplanation { get; set; }
        //Hazır olma Tarihi
        public DateTime? ReadinessDate { get; set; }
        //Müşteriden teslim Alınış Tarihi
        public DateTime? DeliveryFromCustomerDate  { get; set; }
        //İstenen Varış Tarihi
        public DateTime? IntendedArrivalDate  { get; set; }
        [ForeignKey("FirmCustomsArrival")]
        public int? FirmCustomsArrivalId { get; set; }
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

        [ForeignKey("CityShipper")]
        public Nullable<int> ShipperCityId { get; set; }

        [ForeignKey("CityBuyer")]
        public int? BuyerCityId { get; set; }

        [ForeignKey("CountryShipper")]
        public Nullable<int> ShipperCountryId { get; set; }

        [ForeignKey("CountryBuyer")]
        public int? BuyerCountryId { get; set; }

        [ForeignKey("FirmCustomer")]
        public Nullable<int> CustomerFirmId { get; set; }

        [ForeignKey("FirmShipper")]
        public int? ShipperFirmId { get; set; }

        [ForeignKey("FirmBuyer")]
        public int? BuyerFirmId { get; set; }

        [ForeignKey("CustomsEntry")]
        public int? EntryCustomsId { get; set; }

        [ForeignKey("CustomsExit")]
        public int? ExitCustomsId { get; set; }

        [ForeignKey("ItemOrder")]
        public Nullable<int> ItemOrderId { get; set; }


        [ForeignKey("Plant")]
        public Nullable<int> PlantId { get; set; }

        [ForeignKey("User")]
        public Nullable<int> CreatedUserId { get; set; }


        public Nullable<System.DateTime> CreatDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        public virtual Plant Plant { get; set; }
        public virtual User User { get; set; }
        public virtual User UserAuthor { get; set; }
        public virtual ItemOrder ItemOrder { get; set; }
        public virtual Firm FirmShipper { get; set; }
        public virtual Firm FirmBuyer { get; set; }
        public virtual Firm FirmCustomer { get; set; }
        public virtual Customs CustomsEntry { get; set; }
        public virtual Customs CustomsExit { get; set; }
        public virtual Firm FirmCustomsArrival { get; set; }
        public virtual Invoice Invoice { get; set; }
        public virtual Country CountryShipper { get; set; }
        public virtual Country CountryBuyer { get; set; }
        public virtual City CityShipper { get; set; }
        public virtual City CityBuyer { get; set; }
        public virtual ForexType ForexType { get; set; }

        [InverseProperty("ItemLoad")]
        public virtual ICollection<ItemLoadDetail> ItemLoadDetail { get; set; }

    }
}
