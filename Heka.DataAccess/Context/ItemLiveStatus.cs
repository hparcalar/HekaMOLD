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
    
    public partial class ItemLiveStatus
    {
        public int Id { get; set; }
        public Nullable<int> ItemId { get; set; }
        public Nullable<int> WarehouseId { get; set; }
        public Nullable<decimal> InQuantity { get; set; }
        public Nullable<decimal> OutQuantity { get; set; }
        public Nullable<decimal> LiveQuantity { get; set; }
    
        public virtual Item Item { get; set; }
        public virtual Warehouse Warehouse { get; set; }
    }
}
