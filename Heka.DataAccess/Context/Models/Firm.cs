namespace Heka.DataAccess.Context
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Firm
    {
        
        public Firm()
        {
            this.EntryQualityData = new HashSet<EntryQualityData>();
            this.FirmAuthor = new HashSet<FirmAuthor>();
            this.Invoice = new HashSet<Invoice>();
            this.Item = new HashSet<Item>();
            this.ItemOrder = new HashSet<ItemOrder>();
            this.ItemReceipt = new HashSet<ItemReceipt>();
            this.Mold = new HashSet<Mold>();
            this.WorkOrder = new HashSet<WorkOrder>();
        }
    
        public int Id { get; set; }
        public string FirmCode { get; set; }
        public string FirmName { get; set; }
        public string FirmTitle { get; set; }

        [ForeignKey("Plant")]
        public Nullable<int> PlantId { get; set; }
        public Nullable<int> FirmType { get; set; }
        public string Explanation { get; set; }
        public string Phone { get; set; }
        public string Gsm { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Author { get; set; }
        public string Email { get; set; }
        public string TaxNo { get; set; }
        public string TaxOffice { get; set; }
        public Nullable<bool> IsApproved { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
    
        
        public virtual ICollection<EntryQualityData> EntryQualityData { get; set; }
        public virtual Plant Plant { get; set; }
        
        [InverseProperty("Firm")]
        public virtual ICollection<FirmAuthor> FirmAuthor { get; set; }

        [InverseProperty("Firm")]
        public virtual ICollection<Invoice> Invoice { get; set; }

        [InverseProperty("Firm")]
        public virtual ICollection<Item> Item { get; set; }

        [InverseProperty("Firm")]
        public virtual ICollection<ItemOrder> ItemOrder { get; set; }

        [InverseProperty("Firm")]
        public virtual ICollection<ItemReceipt> ItemReceipt { get; set; }
        
        [InverseProperty("Firm")]
        public virtual ICollection<Mold> Mold { get; set; }

        [InverseProperty("Firm")]
        public virtual ICollection<WorkOrder> WorkOrder { get; set; }
    }
}
