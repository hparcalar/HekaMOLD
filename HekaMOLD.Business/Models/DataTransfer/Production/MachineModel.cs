using HekaMOLD.Business.Base;
using HekaMOLD.Business.Models.DataTransfer.Maintenance;
using HekaMOLD.Business.Models.DataTransfer.Summary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class MachineModel : IDataObject
    {
        public int Id { get; set; }
        public string MachineCode { get; set; }
        public string MachineName { get; set; }
        public int? MachineType { get; set; }
        public int? PlantId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsWatched { get; set; }
        public string WatchCycleStartCondition { get; set; }
        public string DeviceIp { get; set; }
        public MachinePlanModel ActivePlan { get; set; }
        public MachinePlanModel[] Plans { get; set; }
        public MachineMaintenanceInstructionModel[] Instructions { get; set; }

        #region VISUAL ELEMENTS
        public string CreatedDateStr { get; set; }
        public MachineStatsModel MachineStats { get; set; }
        #endregion
    }
}
