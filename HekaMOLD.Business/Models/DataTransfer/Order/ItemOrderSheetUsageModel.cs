using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Order
{
    public class ItemOrderSheetUsageModel
    {
        public int Id { get; set; }
        public Nullable<int> ItemOrderDetailId { get; set; }
        public Nullable<int> ItemOrderSheetId { get; set; }
        public Nullable<int> Quantity { get; set; }

        #region VISUAL ELEMENTS
        public int SheetNo { get; set; }
        public int? ItemId { get; set; }
        public string PartCode { get; set; }
        public string PartName { get; set; }
        public string PartVisualStr { get; set; }
        #endregion
    }
}
