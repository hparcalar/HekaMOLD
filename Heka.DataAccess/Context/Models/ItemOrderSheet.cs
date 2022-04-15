using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public class ItemOrderSheet
    {
        public ItemOrderSheet()
        {
            this.ItemOrderDetail = new HashSet<ItemOrderDetail>();
            this.ItemOrderSheetUsage = new HashSet<ItemOrderSheetUsage>();
            this.WorkOrderDetail = new HashSet<WorkOrderDetail>();
        }

        public int Id { get; set; }

        [ForeignKey("ItemOrder")]
        public Nullable<int> ItemOrderId { get; set; }

        public string SheetName { get; set; }
        public Nullable<int> SheetNo { get; set; }
        public byte[] SheetVisual { get; set; }
        public Nullable<DateTime> PerSheetTime { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> Thickness { get; set; }
        public Nullable<decimal> SheetWidth { get; set; }
        public Nullable<decimal> SheetHeight { get; set; }
        public Nullable<decimal> Eff { get; set; }
        public Nullable<int> SheetStatus { get; set; }

        [ForeignKey("SheetItem")]
        public Nullable<int> SheetItemId { get; set; }
        public virtual ItemOrder ItemOrder { get; set; }
        public virtual Item SheetItem { get; set; }

        [InverseProperty("ItemOrderSheet")]
        public virtual ICollection<ItemOrderDetail> ItemOrderDetail { get; set; }

        [InverseProperty("ItemOrderSheet")]
        public virtual ICollection<ItemOrderSheetUsage> ItemOrderSheetUsage { get; set; }

        [InverseProperty("ItemOrderSheet")]
        public virtual ICollection<WorkOrderDetail> WorkOrderDetail { get; set; }
    }
}
