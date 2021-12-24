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
        public decimal? WastageCount { get; set; }
        public int PostureCount { get; set; }
        public int IncidentCount { get; set; }
        public ShiftStatsModel[] ShiftStats { get; set; }
    }

    public class ShiftStatsModel
    {
        public int ShiftId { get; set; }
        public string ShiftCode { get; set; }
        public decimal AvgInflationTime { get; set; }
        public int AvgProductionCount { get; set; }
        public decimal? WastageCount { get; set; }
        public int PostureCount { get; set; }
        public int IncidentCount { get; set; }
        public string ChiefUserName { get; set; }
        public int TargetCount { get; set; }
        public string LastProductName { get; set; }
        public bool IsCurrentShift { get; set; } = false;
    }
}
