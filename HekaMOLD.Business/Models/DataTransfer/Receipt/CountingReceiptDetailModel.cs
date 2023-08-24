using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Receipt
{
    public class CountingReceiptDetailModel
    {
        public int Id { get; set; }
        public Nullable<int> CountingReceiptId { get; set; }
        public Nullable<int> ItemId { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<int> PackageQuantity { get; set; }
        public Nullable<int> WarehouseId { get; set; }

        #region VISUAL ELEMENTS
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public string ItemTypeStr { get; set; }
        public string CategoryName { get; set; }
        public string GroupName { get; set; }
        #endregion
    }
}
