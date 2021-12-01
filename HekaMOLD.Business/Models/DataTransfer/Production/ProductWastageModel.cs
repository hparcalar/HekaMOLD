using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class ProductWastageModel : IDataObject
    {
        public int Id { get; set; }
        public int? WorkOrderDetailId { get; set; }
        public int? ProductId { get; set; }
        public int? MachineId { get; set; }
        public int? ShiftId { get; set; }
        public DateTime? EntryDate { get; set; }
        public decimal? Quantity { get; set; }
        public int? WastageStatus { get; set; }
        public DateTime? ShiftBelongsToDate { get; set; }
        public bool? IsAfterScrap { get; set; }

        #region VISUAL ELEMENTS
        public string WorkOrderNo { get; set; }
        public string ItemOrderNo { get; set; }
        public string MachineCode { get; set; }
        public string MachineName { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ShiftCode { get; set; }
        public string ShiftName { get; set; }
        public string EntryDateStr { get; set; }
        public string CreatedUserName { get; set; }
        public string WastageStatusText { get; set; }
        #endregion
    }
}
