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
    
    public partial class EntryQualityData
    {
        public int Id { get; set; }
        public Nullable<int> QualityPlanId { get; set; }
        public Nullable<int> QualityPlanDetailId { get; set; }
        public Nullable<bool> IsOk { get; set; }
        public string Explanation { get; set; }
        public Nullable<int> ItemEntryDetailId { get; set; }
        public string ItemLot { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
    
        public virtual EntryQualityPlan EntryQualityPlan { get; set; }
        public virtual EntryQualityPlanDetail EntryQualityPlanDetail { get; set; }
        public virtual ItemReceiptDetail ItemReceiptDetail { get; set; }
    }
}
