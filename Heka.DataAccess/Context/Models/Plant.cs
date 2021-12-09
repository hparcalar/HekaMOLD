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

    public partial class Plant
    {
        
        public Plant()
        {
            this.Equipment = new HashSet<Equipment>();
            this.EquipmentCategory = new HashSet<EquipmentCategory>();
            this.Firm = new HashSet<Firm>();
            this.Invoice = new HashSet<Invoice>();
            this.Item = new HashSet<Item>();
            this.ItemCategory = new HashSet<ItemCategory>();
            this.ItemGroup = new HashSet<ItemGroup>();
            this.ItemOrder = new HashSet<ItemOrder>();
            this.ItemReceipt = new HashSet<ItemReceipt>();
            this.ItemRequest = new HashSet<ItemRequest>();
            this.LayoutItem = new HashSet<LayoutItem>();
            this.SyncPoint = new HashSet<SyncPoint>();
            this.UnitType = new HashSet<UnitType>();
            this.UserRole = new HashSet<UserRole>();
            this.Warehouse = new HashSet<Warehouse>();
            this.User = new HashSet<User>();
            this.Process = new HashSet<Process>();
            this.ProcessGroup = new HashSet<ProcessGroup>();
            this.Route = new HashSet<Route>();
            this.MachineGroup = new HashSet<MachineGroup>();
            this.WorkOrder = new HashSet<WorkOrder>();
        }
    
        public int Id { get; set; }
        public string PlantCode { get; set; }
        public string PlantName { get; set; }
        public byte[] LogoData { get; set; }

        [InverseProperty("Plant")]
        public virtual ICollection<Equipment> Equipment { get; set; }

        [InverseProperty("Plant")]
        public virtual ICollection<EquipmentCategory> EquipmentCategory { get; set; }

        [InverseProperty("Plant")]
        public virtual ICollection<Firm> Firm { get; set; }

        [InverseProperty("Plant")]
        public virtual ICollection<Invoice> Invoice { get; set; }

        [InverseProperty("Plant")]
        public virtual ICollection<Item> Item { get; set; }

        [InverseProperty("Plant")]
        public virtual ICollection<ItemCategory> ItemCategory { get; set; }

        [InverseProperty("Plant")]
        public virtual ICollection<ItemGroup> ItemGroup { get; set; }

        [InverseProperty("Plant")]
        public virtual ICollection<ItemOrder> ItemOrder { get; set; }

        [InverseProperty("Plant")]
        public virtual ICollection<ItemReceipt> ItemReceipt { get; set; }

        [InverseProperty("Plant")]
        public virtual ICollection<ItemRequest> ItemRequest { get; set; }

        [InverseProperty("Plant")]
        public virtual ICollection<LayoutItem> LayoutItem { get; set; }

        [InverseProperty("Plant")]
        public virtual ICollection<SyncPoint> SyncPoint { get; set; }

        [InverseProperty("Plant")]
        public virtual ICollection<UnitType> UnitType { get; set; }

        [InverseProperty("Plant")]
        public virtual ICollection<UserRole> UserRole { get; set; }

        [InverseProperty("Plant")]
        public virtual ICollection<Warehouse> Warehouse { get; set; }

        [InverseProperty("Plant")]
        public virtual ICollection<User> User { get; set; }

        [InverseProperty("Plant")]
        public virtual ICollection<Process> Process { get; set; }

        [InverseProperty("Plant")]
        public virtual ICollection<ProcessGroup> ProcessGroup { get; set; }

        [InverseProperty("Plant")]
        public virtual ICollection<Route> Route { get; set; }

        [InverseProperty("Plant")]
        public virtual ICollection<MachineGroup> MachineGroup { get; set; }

        [InverseProperty("Plant")]
        public virtual ICollection<WorkOrder> WorkOrder { get; set; }
    }
}