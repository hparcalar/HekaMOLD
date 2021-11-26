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
    
    public partial class MachineGroup
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MachineGroup()
        {
            this.ActualRouteHistory = new HashSet<ActualRouteHistory>();
            this.Machine = new HashSet<Machine>();
            this.RouteItem = new HashSet<RouteItem>();
        }
    
        public int Id { get; set; }
        public string MachineGroupCode { get; set; }
        public string MachineGroupName { get; set; }
        public Nullable<int> PlantId { get; set; }
        public Nullable<int> LayoutObjectTypeId { get; set; }
        public Nullable<bool> IsProduction { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActualRouteHistory> ActualRouteHistory { get; set; }
        public virtual LayoutObjectType LayoutObjectType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Machine> Machine { get; set; }
        public virtual Plant Plant { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RouteItem> RouteItem { get; set; }
    }
}
