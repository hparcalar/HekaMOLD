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
    
    public partial class UserAuth
    {
        public int Id { get; set; }
        public Nullable<int> AuthTypeId { get; set; }
        public Nullable<int> UserRoleId { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<bool> IsGranted { get; set; }
    
        public virtual User User { get; set; }
        public virtual UserAuthType UserAuthType { get; set; }
        public virtual UserRole UserRole { get; set; }
    }
}
