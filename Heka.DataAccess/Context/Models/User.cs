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
    using Heka.DataAccess.Context.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class User
    {
        
        public User()
        {
            this.Equipment = new HashSet<Equipment>();
            this.ItemRequest = new HashSet<ItemRequest>();
            this.ItemRequestApproveLog = new HashSet<ItemRequestApproveLog>();
            this.Notification = new HashSet<Notification>();
            this.TransactionLog = new HashSet<TransactionLog>();
            this.UserAuth = new HashSet<UserAuth>();
            this.Shift = new HashSet<Shift>();
            this.ItemLoad = new HashSet<ItemLoad>();
            this.ItemOrder = new HashSet<ItemOrder>();
            this.ItemLoadByAuthor = new HashSet<ItemLoad>();
            this.Voyage = new HashSet<Voyage>();

        }

        public int Id { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        [ForeignKey("Plant")]
        public Nullable<int> PlantId { get; set; }

        [ForeignKey("UserRole")]
        public Nullable<int> UserRoleId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
        public byte[] ProfileImage { get; set; }


        [InverseProperty("User")]
        public virtual ICollection<Equipment> Equipment { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<ItemRequest> ItemRequest { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<ItemRequestApproveLog> ItemRequestApproveLog { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<Notification> Notification { get; set; }
        public virtual Plant Plant { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<TransactionLog> TransactionLog { get; set; }
        public virtual UserRole UserRole { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<UserAuth> UserAuth { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<Shift> Shift { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<ItemLoad> ItemLoad { get; set; }

        [InverseProperty("UserAuthor")]
        public virtual ICollection<ItemLoad> ItemLoadByAuthor { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<ItemOrder> ItemOrder { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<Voyage> Voyage { get; set; }
    }
}
