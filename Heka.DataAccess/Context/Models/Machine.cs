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

    public partial class Machine
    {
        
        public Machine()
        {
            this.ActualRouteHistory = new HashSet<ActualRouteHistory>();
            this.Equipment = new HashSet<Equipment>();
            this.Incident = new HashSet<Incident>();
            this.LayoutItem = new HashSet<LayoutItem>();
            this.MachineMaintenanceInstruction = new HashSet<MachineMaintenanceInstruction>();
            this.MachinePlan = new HashSet<MachinePlan>();
            this.MachineSignal = new HashSet<MachineSignal>();
            this.MoldTest = new HashSet<MoldTest>();
            this.ProductionPosture = new HashSet<ProductionPosture>();
            this.ProductQualityData = new HashSet<ProductQualityData>();
            this.RouteItem = new HashSet<RouteItem>();
            this.ShiftTarget = new HashSet<ShiftTarget>();
            this.UserWorkOrderHistory = new HashSet<UserWorkOrderHistory>();
            this.WorkOrderDetail = new HashSet<WorkOrderDetail>();
            this.ProductWastage = new HashSet<ProductWastage>();
            this.Item = new HashSet<Item>();
        }

        public int Id { get; set; }
        public string MachineCode { get; set; }
        public string MachineName { get; set; }
        public Nullable<int> MachineType { get; set; }
        public Nullable<int> PlantId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsWatched { get; set; }
        public string WatchCycleStartCondition { get; set; }
        public string DeviceIp { get; set; }
        public Nullable<int> PostureExpirationCycleCount { get; set; }
        public Nullable<bool> IsUpToPostureEntry { get; set; }
        public Nullable<int> WorkingUserId { get; set; }
        public string BackColor { get; set; }
        public string ForeColor { get; set; }
        public Nullable<int> MachineStatus { get; set; }

        [ForeignKey("MachineGroup")]
        public Nullable<int> MachineGroupId { get; set; }
        public Nullable<int> SignalEndDelay { get; set; }

        [InverseProperty("Machine")]
        public virtual ICollection<ActualRouteHistory> ActualRouteHistory { get; set; }

        [InverseProperty("Machine")]
        public virtual ICollection<Equipment> Equipment { get; set; }

        [InverseProperty("Machine")]
        public virtual ICollection<Incident> Incident { get; set; }

        [InverseProperty("Machine")]
        public virtual ICollection<LayoutItem> LayoutItem { get; set; }

        [InverseProperty("Machine")]
        public virtual ICollection<MachineMaintenanceInstruction> MachineMaintenanceInstruction { get; set; }

        [InverseProperty("Machine")]
        public virtual ICollection<MachinePlan> MachinePlan { get; set; }

        [InverseProperty("Machine")]
        public virtual ICollection<MachineSignal> MachineSignal { get; set; }

        [InverseProperty("Machine")]
        public virtual ICollection<MoldTest> MoldTest { get; set; }

        [InverseProperty("Machine")]
        public virtual ICollection<ProductionPosture> ProductionPosture { get; set; }

        [InverseProperty("Machine")]
        public virtual ICollection<ProductQualityData> ProductQualityData { get; set; }

        [InverseProperty("Machine")]
        public virtual ICollection<RouteItem> RouteItem { get; set; }

        [InverseProperty("Machine")]
        public virtual ICollection<ShiftTarget> ShiftTarget { get; set; }

        [InverseProperty("Machine")]
        public virtual ICollection<UserWorkOrderHistory> UserWorkOrderHistory { get; set; }
        public virtual MachineGroup MachineGroup { get; set; }

        [InverseProperty("Machine")]
        public virtual ICollection<WorkOrderDetail> WorkOrderDetail { get; set; }

        [InverseProperty("Machine")]
        public virtual ICollection<ProductWastage> ProductWastage { get; set; }
        [InverseProperty("Machine")]
        public virtual ICollection<Item> Item { get; set; }
    }
}
