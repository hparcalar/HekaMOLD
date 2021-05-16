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
    
    public partial class ItemOrder
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ItemOrder()
        {
            this.ItemOrderDetail = new HashSet<ItemOrderDetail>();
            this.ItemReceipt = new HashSet<ItemReceipt>();
        }
    
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public string DocumentNo { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public Nullable<int> OrderType { get; set; }
        public Nullable<System.DateTime> DateOfNeed { get; set; }
        public Nullable<int> FirmId { get; set; }
        public Nullable<int> InWarehouseId { get; set; }
        public Nullable<int> OutWarehouseId { get; set; }
        public Nullable<int> PlantId { get; set; }
        public string Explanation { get; set; }
        public Nullable<int> OrderStatus { get; set; }
        public Nullable<int> ItemRequestId { get; set; }
        public Nullable<int> SyncStatus { get; set; }
        public Nullable<int> SyncPointId { get; set; }
        public Nullable<System.DateTime> SyncDate { get; set; }
        public Nullable<int> SyncUserId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
        public Nullable<decimal> SubTotal { get; set; }
        public Nullable<decimal> TaxPrice { get; set; }
        public Nullable<decimal> OverallTotal { get; set; }
    
        public virtual Firm Firm { get; set; }
        public virtual ItemRequest ItemRequest { get; set; }
        public virtual Plant Plant { get; set; }
        public virtual SyncPoint SyncPoint { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual Warehouse Warehouse1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemOrderDetail> ItemOrderDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemReceipt> ItemReceipt { get; set; }
    }
}
