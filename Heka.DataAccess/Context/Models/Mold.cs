namespace Heka.DataAccess.Context
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Mold
    {
        
        public Mold()
        {
            this.ItemList = new HashSet<Item>();
            this.MoldProduct = new HashSet<MoldProduct>();
            this.MoldTest = new HashSet<MoldTest>();
            this.WorkOrderDetail = new HashSet<WorkOrderDetail>();
        }
    
        public int Id { get; set; }
        public string MoldCode { get; set; }
        public string MoldName { get; set; }
        public Nullable<int> PlantId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
        public Nullable<bool> IsActive { get; set; }

        [ForeignKey("Firm")]
        public Nullable<int> FirmId { get; set; }
        public Nullable<int> LifeTimeTicks { get; set; }
        public Nullable<int> CurrentTicks { get; set; }

        [ForeignKey("ItemMold")]
        public Nullable<int> MoldItemId { get; set; }
        public Nullable<System.DateTime> OwnedDate { get; set; }
        public Nullable<int> MoldStatus { get; set; }
        public string Explanation { get; set; }

        [ForeignKey("Warehouse")]
        public Nullable<int> InWarehouseId { get; set; }

        public virtual Firm Firm { get; set; }
        public virtual Warehouse Warehouse { get; set; }

        [InverseProperty("Mold")]
        public virtual ICollection<Item> ItemList { get; set; }
        public virtual Item ItemMold { get; set; }
        
        [InverseProperty("Mold")]
        public virtual ICollection<MoldProduct> MoldProduct { get; set; }

        [InverseProperty("Mold")]
        public virtual ICollection<MoldTest> MoldTest { get; set; }

        [InverseProperty("Mold")]
        public virtual ICollection<WorkOrderDetail> WorkOrderDetail { get; set; }
    }
}
