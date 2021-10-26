using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Summary
{
    public class ProductionHistoryModel
    {
        public int WorkOrderDetailId { get; set; }
        public DateTime? WorkDate { get; set; }
        public string WorkDateStr { get; set; }
        public string WorkOrderNo { get; set; }
        public string SaleOrderNo { get; set; }
        public decimal? WorkQuantity { get; set; }
        public decimal? OrderQuantity { get; set; }
        public decimal? CompleteQuantity { get; set; }
        public decimal? WastageQuantity { get; set; }
        public int? MachineId { get; set; }
        public string MachineCode { get; set; }
        public string MachineName { get; set; }
        public int? ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int? ShiftId { get; set; }
        public string ShiftCode { get; set; }
        public string ShiftName { get; set; }
        public int? SerialCount { get; set; }

    }
}
