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

    public partial class FirmAuthor
    {
        public int Id { get; set; }

        [ForeignKey("Firm")]
        public Nullable<int> FirmId { get; set; }
        public string AuthorName { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Gsm { get; set; }
        public Nullable<bool> SendMailForPurchaseOrder { get; set; }
        public Nullable<bool> SendMailForProduction { get; set; }
    
        public virtual Firm Firm { get; set; }
    }
}