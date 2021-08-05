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
    
    public partial class WorkOrderDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WorkOrderDetail()
        {
            this.WorkOrderAllocation = new HashSet<WorkOrderAllocation>();
            this.WorkOrderControl = new HashSet<WorkOrderControl>();
            this.WorkOrderSerial = new HashSet<WorkOrderSerial>();
            this.MachinePlan = new HashSet<MachinePlan>();
            this.ProductionPosture = new HashSet<ProductionPosture>();
            this.MachineSignal = new HashSet<MachineSignal>();
        }
    
        public int Id { get; set; }
        public Nullable<int> WorkOrderId { get; set; }
        public Nullable<int> ItemId { get; set; }
        public Nullable<int> MoldId { get; set; }
        public Nullable<int> DyeId { get; set; }
        public Nullable<int> MachineId { get; set; }
        public Nullable<int> MoldTestId { get; set; }
        public Nullable<int> InflationTimeSeconds { get; set; }
        public Nullable<decimal> RawGr { get; set; }
        public Nullable<decimal> RawGrToleration { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<int> WorkOrderStatus { get; set; }
        public Nullable<int> InPalletPackageQuantity { get; set; }
        public Nullable<int> InPackageQuantity { get; set; }
        public Nullable<int> SaleOrderDetailId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
    
        public virtual Dye Dye { get; set; }
        public virtual Item Item { get; set; }
        public virtual ItemOrderDetail ItemOrderDetail { get; set; }
        public virtual Machine Machine { get; set; }
        public virtual Mold Mold { get; set; }
        public virtual MoldTest MoldTest { get; set; }
        public virtual WorkOrder WorkOrder { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkOrderAllocation> WorkOrderAllocation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkOrderControl> WorkOrderControl { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkOrderSerial> WorkOrderSerial { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MachinePlan> MachinePlan { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductionPosture> ProductionPosture { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MachineSignal> MachineSignal { get; set; }
    }
}
