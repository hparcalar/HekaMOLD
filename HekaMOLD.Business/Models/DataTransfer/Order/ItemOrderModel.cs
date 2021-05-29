using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Order
{
    public class ItemOrderModel : IDataObject
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public string DocumentNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? OrderType { get; set; }
        public DateTime? DateOfNeed { get; set; }
        public int? FirmId { get; set; }
        public int? InWarehouseId { get; set; }
        public int? OutWarehouseId { get; set; }
        public int? PlantId { get; set; }
        public string Explanation { get; set; }
        public int? OrderStatus { get; set; }
        public int? ItemRequestId { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? TaxPrice { get; set; }
        public decimal? OverallTotal { get; set; }
        public int? SyncStatus { get; set; }
        public int? SyncPointId { get; set; }
        public DateTime? SyncDate { get; set; }
        public int? SyncUserId { get; set; }
        public ItemOrderDetailModel[] Details { get; set; }

        #region VISUAL ELEMENTS
        public string FirmCode { get; set; }
        public string FirmName { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public string OrderStatusStr { get; set; }
        public string CreatedDateStr { get; set; }
        public string DateOfNeedStr { get; set; }
        public string OrderDateStr { get; set; }
        public string CreatedUserName { get; set; }
        #endregion
    }
}
