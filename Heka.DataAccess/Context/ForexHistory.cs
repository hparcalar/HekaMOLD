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
    
    public partial class ForexHistory
    {
        public int Id { get; set; }
        public Nullable<int> ForexId { get; set; }
        public Nullable<System.DateTime> HistoryDate { get; set; }
        public Nullable<decimal> SalesForexRate { get; set; }
        public Nullable<decimal> BuyForexRate { get; set; }
    
        public virtual ForexType ForexType { get; set; }
    }
}