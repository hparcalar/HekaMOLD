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

    public partial class ForexType
    {
        
        public ForexType()
        {
            this.ForexHistory = new HashSet<ForexHistory>();
            this.ItemOrderDetail = new HashSet<ItemOrderDetail>();
            this.ItemReceiptDetail = new HashSet<ItemReceiptDetail>();
            this.VehicleInsurance = new HashSet<VehicleInsurance>();
        }

        public int Id { get; set; }
        public string ForexTypeCode { get; set; }
        public Nullable<decimal> ActiveSalesRate { get; set; }
        public Nullable<decimal> ActiveBuyingRate { get; set; }


        [InverseProperty("ForexType")]
        public virtual ICollection<ForexHistory> ForexHistory { get; set; }

        [InverseProperty("ForexType")]
        public virtual ICollection<ItemOrderDetail> ItemOrderDetail { get; set; }

        [InverseProperty("ForexType")]
        public virtual ICollection<ItemReceiptDetail> ItemReceiptDetail { get; set; }

        [InverseProperty("ForexType")]
        public virtual ICollection<VehicleInsurance> VehicleInsurance { get; set; }
    }
}
