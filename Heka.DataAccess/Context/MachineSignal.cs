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
    
    public partial class MachineSignal
    {
        public int Id { get; set; }
        public Nullable<int> MachineId { get; set; }
        public Nullable<int> WorkOrderDetailId { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> Duration { get; set; }
        public Nullable<int> SignalStatus { get; set; }
    
        public virtual Machine Machine { get; set; }
        public virtual WorkOrderDetail WorkOrderDetail { get; set; }
    }
}
