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
        public int? PostureExpirationCycleCount { get; set; }
        public bool? IsUpToPostureEntry { get; set; }
        public int? WorkingUserId { get; set; }
        public string BackColor { get; set; }
        public string ForeColor { get; set; }
        public int? MachineStatus { get; set; }
        public int? MachineGroupId { get; set; }
        public int? SignalEndDelay { get; set; }
        public int? Width { get; set; }
        public string NumberOfFramaes { get; set; }
        //Tahar
        public string WeavingDraftInfo { get; set; }
        public int? MachineBreedId { get; set; }
        public int? WeavingDraftId { get; set; }

        public MachinePlanModel ActivePlan { get; set; }
        public MachinePlanModel[] Plans { get; set; }
        public MachineMaintenanceInstructionModel[] Instructions { get; set; }
        public EquipmentModel[] Equipments { get; set; }

        #region VISUAL ELEMENTS
        public string CreatedDateStr { get; set; }
        public string MachineStatusText { get; set; }
        public MachineStatsModel MachineStats { get; set; }
        public string WorkingUserCode { get; set; }
        public string WorkingUserName { get; set; }
        public string MachineGroupCode { get; set; }
        public string MachineGroupName { get; set; }
        public string DesignPath { get; set; }
        public bool IsInIncident { get; set; }
        public bool IsInPosture { get; set; }
        public string ActivePostureText { get; set; }
        public string ActiveIncidentText { get; set; }
        public string MachineBreedCode { get; set; }
        public string MachineBreedName { get; set; }
        public string WeavingDraftCode { get; set; }

        #endregion
    }
}
