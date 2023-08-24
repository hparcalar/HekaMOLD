using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class MachinePlanViewDetailModel
    {
        public int Id { get; set; }
        public Nullable<int> MachinePlanViewId { get; set; }
        public Nullable<int> MachineId { get; set; }
        public Nullable<int> WorkOrderDetailId { get; set; }
        public Nullable<int> OrderNo { get; set; }

        #region VISUAL ELEMENTS
        public WorkOrderDetailModel WorkOrder { get; set; }
        public WorkOrderSerialModel[] Serials { get; set; }
        public ProductWastageModel[] Wastages { get; set; }
        #endregion
    }
}
