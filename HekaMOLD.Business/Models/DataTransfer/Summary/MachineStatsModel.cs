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

        public bool IsLiveModel { get; set; } = false;

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
        public bool IsLiveModel { get; set; } = false;
        public bool IsCurrentShift { get; set; } = false;
        public int ActiveProductionCount { get; set; }
        public int ActiveWastageCount { get; set; }
        public ShiftProductionModel[] ProductionData { get; set; }
    }

    public class ShiftProductionModel
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int ProdCount { get; set; }
        public int WastageCount { get; set; }
    }
}
