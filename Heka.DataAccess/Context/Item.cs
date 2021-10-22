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
    
    public partial class Item
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Item()
        {
            this.ItemLiveStatus = new HashSet<ItemLiveStatus>();
            this.ItemOrderDetail = new HashSet<ItemOrderDetail>();
            this.ItemReceiptDetail = new HashSet<ItemReceiptDetail>();
            this.ItemRequestDetail = new HashSet<ItemRequestDetail>();
            this.ItemSerial = new HashSet<ItemSerial>();
            this.ItemUnit = new HashSet<ItemUnit>();
            this.ItemWarehouse = new HashSet<ItemWarehouse>();
            this.ProductRecipe = new HashSet<ProductRecipe>();
            this.ProductRecipeDetail = new HashSet<ProductRecipeDetail>();
            this.WorkOrderDetail = new HashSet<WorkOrderDetail>();
            this.WorkOrderItemNeeds = new HashSet<WorkOrderItemNeeds>();
            this.ProductQualityData = new HashSet<ProductQualityData>();
            this.EntryQualityData = new HashSet<EntryQualityData>();
            this.MoldProduct = new HashSet<MoldProduct>();
            this.Mold1 = new HashSet<Mold>();
            this.ItemOrderItemNeeds = new HashSet<ItemOrderItemNeeds>();
            this.ProductWastage = new HashSet<ProductWastage>();
        }
    
        public int Id { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public Nullable<int> ItemType { get; set; }
        public Nullable<int> ItemCategoryId { get; set; }
        public Nullable<int> ItemGroupId { get; set; }
        public Nullable<int> SupplierFirmId { get; set; }
        public Nullable<int> PlantId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
        public Nullable<int> MoldId { get; set; }
    
        public virtual Firm Firm { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        public virtual ItemGroup ItemGroup { get; set; }
        public virtual Plant Plant { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemLiveStatus> ItemLiveStatus { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemOrderDetail> ItemOrderDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemReceiptDetail> ItemReceiptDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemRequestDetail> ItemRequestDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemSerial> ItemSerial { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemUnit> ItemUnit { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemWarehouse> ItemWarehouse { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductRecipe> ProductRecipe { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductRecipeDetail> ProductRecipeDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkOrderDetail> WorkOrderDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkOrderItemNeeds> WorkOrderItemNeeds { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductQualityData> ProductQualityData { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EntryQualityData> EntryQualityData { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MoldProduct> MoldProduct { get; set; }
        public virtual Mold Mold { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Mold> Mold1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemOrderItemNeeds> ItemOrderItemNeeds { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductWastage> ProductWastage { get; set; }
    }
}
