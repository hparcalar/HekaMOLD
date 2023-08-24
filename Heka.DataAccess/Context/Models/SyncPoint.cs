namespace Heka.DataAccess.Context
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class SyncPoint
    {
        
        public SyncPoint()
        {
            this.ItemOrder = new HashSet<ItemOrder>();
            this.ItemReceipt = new HashSet<ItemReceipt>();
        }
    
        public int Id { get; set; }
        public string SyncPointCode { get; set; }
        public string SyncPointName { get; set; }
        public Nullable<int> SyncPointType { get; set; }
        public string ConnectionString { get; set; }

        [ForeignKey("Plant")]
        public Nullable<int> PlantId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
        public Nullable<bool> EnabledOnPurchaseOrders { get; set; }
        public Nullable<bool> EnabledOnSalesOrders { get; set; }
        public Nullable<bool> EnabledOnPurchaseItemReceipts { get; set; }
        public Nullable<bool> EnabledOnSalesItemReceipts { get; set; }
        public Nullable<bool> EnabledOnConsumptionReceipts { get; set; }


        [InverseProperty("SyncPoint")]
        public virtual ICollection<ItemOrder> ItemOrder { get; set; }

        [InverseProperty("SyncPoint")]
        public virtual ICollection<ItemReceipt> ItemReceipt { get; set; }
        public virtual Plant Plant { get; set; }
    }
}
