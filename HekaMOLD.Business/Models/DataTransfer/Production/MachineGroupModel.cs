using HekaMOLD.Business.Models.DataTransfer.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class MachineGroupModel
    {
        public int Id { get; set; }
        public string MachineGroupCode { get; set; }
        public string MachineGroupName { get; set; }
        public int? PlantId { get; set; }
        public int? LayoutObjectTypeId { get; set; }
        public bool? IsProduction { get; set; }

        #region VISUAL ELEMENTS
        public LayoutObjectTypeModel LayoutObjectTypeData { get; set; }
        #endregion
    }
}
