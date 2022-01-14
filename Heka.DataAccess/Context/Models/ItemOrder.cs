//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Heka.DataAccess.Context
{
    using Heka.DataAccess.Context.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class ItemOrder
    {
        
        public ItemOrder()
        {
            this.ItemOrderDetail = new HashSet<ItemOrderDetail>();
            this.ItemOrderItemNeeds = new HashSet<ItemOrderItemNeeds>();
            this.ItemReceipt = new HashSet<ItemReceipt>();
        }
    
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public string DocumentNo { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public Nullable<int> OrderType { get; set; }
        public int? OrderUploadType { get; set; }
        public int? OrderTransactionDirectionType { get; set; }
        public Nullable<System.DateTime> DateOfNeed { get; set; }
        [ForeignKey("CustomerFirm")]
        public Nullable<int> CustomerFirmId { get; set; }
        //Gönderici Firma
        [ForeignKey("ShipperFirm")]
        public int? ShipperFirmId { get; set; }
        [ForeignKey("BuyerFirm")]
        public int? BuyerFirmId { get; set; }

        public decimal? TotalWeight { get; set; }
        //Toplam Hacim
        public decimal? TotalVolume { get; set; }
        public bool? Closed { get; set; }
        [ForeignKey("Customs1")]
        public int? ExitCustomsId  { get; set; }
        [ForeignKey("Customs")]
        public int? EntryCustomsId { get; set; }
        [ForeignKey("Warehouse")]
        public Nullable<int> InWarehouseId { get; set; }

        [ForeignKey("Warehouse1")]
        public Nullable<int> OutWarehouseId { get; set; }
        public Nullable<int> PlantId { get; set; }
        public string Explanation { get; set; }
        public Nullable<int> OrderStatus { get; set; }
        public Nullable<int> ItemRequestId { get; set; }
        public Nullable<decimal> SubTotal { get; set; }
        public Nullable<decimal> TaxPrice { get; set; }
        public Nullable<decimal> OverallTotal { get; set; }
        public Nullable<int> SyncStatus { get; set; }
        public Nullable<int> SyncPointId { get; set; }
        public Nullable<System.DateTime> SyncDate { get; set; }
        public Nullable<int> SyncUserId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
        public string SyncKey { get; set; }
    
        public virtual Firm CustomerFirm { get; set; }
        public virtual Firm ShipperFirm { get; set; }
        public virtual Firm BuyerFirm { get; set; }
        public virtual Customs Customs { get; set; }
        public virtual Customs Customs1 { get; set; }
        public virtual ItemRequest ItemRequest { get; set; }
        public virtual Plant Plant { get; set; }
        public virtual SyncPoint SyncPoint { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual Warehouse Warehouse1 { get; set; }

        [InverseProperty("ItemOrder")]
        public virtual ICollection<ItemOrderDetail> ItemOrderDetail { get; set; }

        [InverseProperty("ItemOrder")]
        public virtual ICollection<ItemOrderItemNeeds> ItemOrderItemNeeds { get; set; }

        [InverseProperty("ItemOrder")]
        public virtual ICollection<ItemReceipt> ItemReceipt { get; set; }
    }
}
