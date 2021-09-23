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
    
    public partial class EntryQualityPlanDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EntryQualityPlanDetail()
        {
            this.EntryQualityData = new HashSet<EntryQualityData>();
            this.EntryQualityDataDetail = new HashSet<EntryQualityDataDetail>();
        }
    
        public int Id { get; set; }
        public Nullable<int> EntryQualityPlanId { get; set; }
        public string CheckProperty { get; set; }
        public Nullable<bool> IsRequired { get; set; }
        public Nullable<int> OrderNo { get; set; }
        public string PeriodType { get; set; }
        public string AcceptanceCriteria { get; set; }
        public string ControlDevice { get; set; }
        public string Method { get; set; }
        public string Responsible { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EntryQualityData> EntryQualityData { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EntryQualityDataDetail> EntryQualityDataDetail { get; set; }
        public virtual EntryQualityPlan EntryQualityPlan { get; set; }
    }
}
