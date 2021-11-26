using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class WorkOrderSerialModel : IDataObject
    {
        public int Id { get; set; }
        public int? WorkOrderDetailId { get; set; }
        public int? WorkOrderId { get; set; }
        public string SerialNo { get; set; }
        public int? SerialType { get; set; }
        public bool? IsGeneratedBySignal { get; set; }
        public int? SerialStatus { get; set; }
        public decimal? FirstQuantity { get; set; }
        public decimal? LiveQuantity { get; set; }
        public int? InPackageQuantity { get; set; }
        public int? ShiftId { get; set; }
        public int? ItemReceiptDetailId { get; set; }
        public int? QualityStatus { get; set; }
        public string QualityExplanation { get; set; }
        public int? TargetWarehouseId { get; set; }

        #region VISUAL ELEMENTS
        public int? ItemId { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string CreatedDateStr { get; set; }
        public string ShiftCode { get; set; }
        public string ShiftName { get; set; }
        public string FirmCode { get; set; }
        public string FirmName { get; set; }
        public string MachineCode { get; set; }
        public string MachineName { get; set; }
        public string QualityStatusText { get; set; }
        #endregion
    }
}
