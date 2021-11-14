using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Layout
{
    public class LayoutItemModel
    {
        public int Id { get; set; }
        public int? MachineId { get; set; }
        public string PositionData { get; set; }
        public string RotationData { get; set; }
        public string ScalingData { get; set; }
        public string Title { get; set; }
        public int? PlantId { get; set; }

        #region VISUAL ELEMENTS
        public string MachineCode { get; set; }
        public string MachineName { get; set; }
        public string MachineGroupCode { get; set; }
        public string MachineGroupName { get; set; }
        #endregion
    }
}
