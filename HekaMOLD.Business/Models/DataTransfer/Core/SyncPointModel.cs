using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class SyncPointModel : IDataObject
    {
        public int Id { get; set; }
        public string SyncPointCode { get; set; }
        public string SyncPointName { get; set; }
        public int? SyncPointType { get; set; }
        public string ConnectionString { get; set; }
        public int? PlantId { get; set; }
        public bool? IsActive { get; set; }
        public bool? EnabledOnPurchaseOrders { get; set; }
        public bool? EnabledOnSalesOrders { get; set; }
        public bool? EnabeldOnPurchaseItemReceipts { get; set; }
        public bool? EnabledOnSalesItemReceipts { get; set; }
        public bool? EnabledOnConsumptionReceipts { get; set; }
    }
}
