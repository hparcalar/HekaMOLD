using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class ItemDemandModel
    {
        public int Id { get; set; }
        public Nullable<int> ItemId { get; set; }

        public Nullable<decimal> DemandQuantity { get; set; }
        public Nullable<decimal> SuppliedQuantity { get; set; }
        public Nullable<int> WorkOrderDetailId { get; set; }
        public string Explanation { get; set; }

        public Nullable<DateTime> DemandDate { get; set; }
        public Nullable<DateTime> SupplyDate { get; set; }
        public Nullable<int> DemandedUserId { get; set; }
        public Nullable<int> SupplierUserId { get; set; }
        public Nullable<int> DemandStatus { get; set; }

        #region VISUAL ELEMENTS
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string WorkOrderNo { get; set; }
        public string ItemOrderDocumentNo { get; set; }
        public string DemandedUserName { get; set; }
        public string SupplierUserName { get; set; }
        public string DemandDateStr { get; set; }
        public string SupplyDateStr { get; set; }
        public bool IsLackOfStock { get; set; }
        public int LackOfStockCount { get; set; }
        #endregion
    }
}
