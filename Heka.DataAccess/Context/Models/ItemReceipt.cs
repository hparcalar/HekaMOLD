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

    public partial class ItemReceipt
    {
        
        public ItemReceipt()
        {
            this.ItemReceiptDetail = new HashSet<ItemReceiptDetail>();
        }
    
        public int Id { get; set; }
        public string ReceiptNo { get; set; }
        public string DocumentNo { get; set; }
        public Nullable<System.DateTime> ReceiptDate { get; set; }
        public Nullable<int> ReceiptType { get; set; }

        [ForeignKey("Firm")]
        public Nullable<int> FirmId { get; set; }

        [ForeignKey("Warehouse")]
        public Nullable<int> InWarehouseId { get; set; }
        [ForeignKey("Warehouse1")]
        public Nullable<int> OutWarehouseId { get; set; }

        [ForeignKey("Plant")]
        public Nullable<int> PlantId { get; set; }
        public string DeliveryAddress { get; set; }
        public string Explanation { get; set; }
        public Nullable<int> ReceiptStatus { get; set; }
        public Nullable<int> SyncStatus { get; set; }

        [ForeignKey("SyncPoint")]
        public Nullable<int> SyncPointId { get; set; }
        public Nullable<System.DateTime> SyncDate { get; set; }
        public Nullable<int> SyncUserId { get; set; }

        [ForeignKey("ItemOrder")]
        public Nullable<int> ItemOrderId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        [ForeignKey("Invoice")]
        public Nullable<int> InvoiceId { get; set; }

        [ForeignKey("WorkOrderDetail")]
        public Nullable<int> WorkOrderDetailId { get; set; }

        [ForeignKey("ReceiverPlant")]
        public Nullable<int> ReceiverPlantId { get; set; }
        public Nullable<int> PriceCalcType { get; set; }

        public virtual Firm Firm { get; set; }
        public virtual Invoice Invoice { get; set; }
        public virtual ItemOrder ItemOrder { get; set; }
        public virtual Plant Plant { get; set; }
        public virtual Plant ReceiverPlant { get; set; }
        public virtual SyncPoint SyncPoint { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual Warehouse Warehouse1 { get; set; }
        public virtual WorkOrderDetail WorkOrderDetail { get; set; }

        [InverseProperty("ItemReceipt")]
        public virtual ICollection<ItemReceiptDetail> ItemReceiptDetail { get; set; }
    }
}
