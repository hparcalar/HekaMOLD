using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Summary
{
    public class MachineStatsModel
    {
        public decimal AvgInflationTime { get; set; }
        public int AvgProductionCount { get; set; }
        public ShiftStatsModel[] ShiftStats { get; set; }
    }

    public class ShiftStatsModel
    {
        public int ShiftId { get; set; }
        public string ShiftCode { get; set; }
        public decimal AvgInflationTime { get; set; }
        public int AvgProductionCount { get; set; }
    }
}
