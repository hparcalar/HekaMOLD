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
    
    public partial class Notification
    {
        public int Id { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> NotifyType { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public Nullable<int> RecordId { get; set; }
        public Nullable<int> SeenStatus { get; set; }
        public Nullable<System.DateTime> SeenDate { get; set; }
        public Nullable<bool> IsProcessed { get; set; }
        public Nullable<System.DateTime> ProcessedDate { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    
        public virtual User User { get; set; }
    }
}
