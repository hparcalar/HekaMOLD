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

    public partial class ItemOrderDetail
    {
        
        public ItemOrderDetail()
        {
            this.ItemOrderItemNeeds = new HashSet<ItemOrderItemNeeds>();
            this.ItemReceiptDetail = new HashSet<ItemReceiptDetail>();
            this.ItemOrderConsume = new HashSet<ItemOrderConsume>();
            this.WorkOrderDetail = new HashSet<WorkOrderDetail>();
        }
    
        public int Id { get; set; }

        [ForeignKey("ItemOrder")]
        public Nullable<int> ItemOrderId { get; set; }
        public Nullable<int> LineNumber { get; set; }

        [ForeignKey("Item")]
        public Nullable<int> ItemId { get; set; }

        [ForeignKey("UnitType")]
        public Nullable<int> UnitId { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<decimal> NetQuantity { get; set; }
        public Nullable<decimal> GrossQuantity { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public int? ShortWidth { get; set; }
        public int? LongWidth { get; set; }
        public decimal? Volume { get; set; }
        public int? Height { get; set; }
        public int? Weight { get; set; }
        //istiflenebilir
        public bool? Stackable { get; set; }
        public decimal? Desi { get; set; }
        public int? CalculationTypeEnum { get; set; }
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
        public Nullable<int> OrderStatus { get; set; }
        public Nullable<int> SyncStatus { get; set; }
        public Nullable<System.DateTime> SyncDate { get; set; }

        [ForeignKey("ItemRequestDetail")]
        public Nullable<int> ItemRequestDetailId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
    
        public virtual ForexType ForexType { get; set; }
        public virtual Item Item { get; set; }
        public virtual ItemOrder ItemOrder { get; set; }
        public virtual ItemRequestDetail ItemRequestDetail { get; set; }
        public virtual UnitType UnitType { get; set; }

        [InverseProperty("ItemOrderDetail")]
        public virtual ICollection<ItemOrderItemNeeds> ItemOrderItemNeeds { get; set; }

        [InverseProperty("ItemOrderDetail")]
        public virtual ICollection<ItemReceiptDetail> ItemReceiptDetail { get; set; }

        [InverseProperty("ItemOrderDetail")]
        public virtual ICollection<ItemOrderConsume> ItemOrderConsume { get; set; }

        [InverseProperty("ItemOrderDetail")]
        public virtual ICollection<WorkOrderDetail> WorkOrderDetail { get; set; }
    }
}
