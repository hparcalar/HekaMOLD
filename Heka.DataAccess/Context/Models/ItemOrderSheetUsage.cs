using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public class ItemOrderSheetUsage
    {
        public int Id { get; set; }

        [ForeignKey("ItemOrderDetail")]
        public Nullable<int> ItemOrderDetailId { get; set; }

        [ForeignKey("ItemOrderSheet")]
        public Nullable<int> ItemOrderSheetId { get; set; }
        public Nullable<int> Quantity { get; set; }

        public virtual ItemOrderDetail ItemOrderDetail { get; set; }
        public virtual ItemOrderSheet ItemOrderSheet { get; set; }
    }
}
