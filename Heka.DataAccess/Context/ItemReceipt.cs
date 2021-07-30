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
    
    public partial class ItemReceipt
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ItemReceipt()
        {
            this.ItemReceiptDetail = new HashSet<ItemReceiptDetail>();
        }
    
        public int Id { get; set; }
        public string ReceiptNo { get; set; }
        public string DocumentNo { get; set; }
        public Nullable<System.DateTime> ReceiptDate { get; set; }
        public Nullable<int> ReceiptType { get; set; }
        public Nullable<int> FirmId { get; set; }
        public Nullable<int> InWarehouseId { get; set; }
        public Nullable<int> OutWarehouseId { get; set; }
        public Nullable<int> PlantId { get; set; }
        public string DeliveryAddress { get; set; }
        public string Explanation { get; set; }
        public Nullable<int> SyncStatus { get; set; }
        public Nullable<int> SyncPointId { get; set; }
        public Nullable<System.DateTime> SyncDate { get; set; }
        public Nullable<int> SyncUserId { get; set; }
        public Nullable<int> ItemOrderId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
        public Nullable<int> ReceiptStatus { get; set; }
    
        public virtual Firm Firm { get; set; }
        public virtual ItemOrder ItemOrder { get; set; }
        public virtual Plant Plant { get; set; }
        public virtual SyncPoint SyncPoint { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual Warehouse Warehouse1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemReceiptDetail> ItemReceiptDetail { get; set; }
    }
}