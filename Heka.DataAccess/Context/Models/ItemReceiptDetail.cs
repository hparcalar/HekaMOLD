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
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class ItemReceiptDetail
    {
        
        public ItemReceiptDetail()
        {
            this.EntryQualityData = new HashSet<EntryQualityData>();
            this.ItemReceiptConsumeByConsumed = new HashSet<ItemReceiptConsume>();
            this.ItemReceiptConsumeByConsumer = new HashSet<ItemReceiptConsume>();
            this.WorkOrderAllocation = new HashSet<WorkOrderAllocation>();
            this.WorkOrderSerial = new HashSet<WorkOrderSerial>();
            this.ItemSerial = new HashSet<ItemSerial>();
            this.ItemOrderConsumeByConsumer = new HashSet<ItemOrderConsume>();
            this.ItemOrderConsumeByConsumed = new HashSet<ItemOrderConsume>();
        }
    
        public int Id { get; set; }
        public Nullable<int> ItemReceiptId { get; set; }
        public Nullable<int> LineNumber { get; set; }

        [ForeignKey("Item")]
        public Nullable<int> ItemId { get; set; }
        [ForeignKey("UnitType")]
        public Nullable<int> UnitId { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<decimal> NetQuantity { get; set; }
        public Nullable<decimal> GrossQuantity { get; set; }
        public Nullable<decimal> UsableQuantity { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        [ForeignKey("ForexType")]
        public Nullable<int> ForexId { get; set; }
        public Nullable<decimal> ForexRate { get; set; }
        public Nullable<decimal> ForexUnitPrice { get; set; }
        public Nullable<bool> TaxIncluded { get; set; }
        public Nullable<int> TaxRate { get; set; }
        public Nullable<decimal> TaxAmount { get; set; }
        public Nullable<decimal> DiscountRate { get; set; }
        public Nullable<decimal> DiscountAmount { get; set; }
        public Nullable<decimal> SubTotal { get; set; }
        public Nullable<decimal> OverallTotal { get; set; }
        public string Explanation { get; set; }
        public Nullable<int> ReceiptStatus { get; set; }
        public Nullable<int> SyncStatus { get; set; }
        public Nullable<System.DateTime> SyncDate { get; set; }

        [ForeignKey("ItemOrderDetail")]
        public Nullable<int> ItemOrderDetailId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }


        [InverseProperty("ItemReceiptDetail")]
        public virtual ICollection<EntryQualityData> EntryQualityData { get; set; }
        public virtual ForexType ForexType { get; set; }
        public virtual Item Item { get; set; }
        public virtual ItemOrderDetail ItemOrderDetail { get; set; }
        public virtual ItemReceipt ItemReceipt { get; set; }

        [InverseProperty("ItemReceiptDetailConsumed")]
        public virtual ICollection<ItemReceiptConsume> ItemReceiptConsumeByConsumed { get; set; }

        [InverseProperty("ItemReceiptDetailConsumer")]
        public virtual ICollection<ItemReceiptConsume> ItemReceiptConsumeByConsumer { get; set; }
        public virtual UnitType UnitType { get; set; }

        [InverseProperty("ItemReceiptDetail")]
        public virtual ICollection<WorkOrderAllocation> WorkOrderAllocation { get; set; }

        [InverseProperty("ItemReceiptDetail")]
        public virtual ICollection<WorkOrderSerial> WorkOrderSerial { get; set; }

        [InverseProperty("ItemReceiptDetail")]
        public virtual ICollection<ItemSerial> ItemSerial { get; set; }

        [InverseProperty("ItemReceiptDetailConsumer")]
        public virtual ICollection<ItemOrderConsume> ItemOrderConsumeByConsumer { get; set; }

        [InverseProperty("ItemReceiptDetailConsumed")]
        public virtual ICollection<ItemOrderConsume> ItemOrderConsumeByConsumed { get; set; }
    }
}