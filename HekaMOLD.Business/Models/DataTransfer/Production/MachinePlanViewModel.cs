using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class MachinePlanViewModel
    {
        public int Id { get; set; }
        public Nullable<DateTime> ViewDate { get; set; }
        public Nullable<int> PlantId { get; set; }

        #region VISUAL ELEMENTS
        public string ViewDateStr { get; set; }
        public int WeekOfYear { get; set; }
        public string PlantCode { get; set; }
        public string PlantName { get; set; }
        #endregion
    }
}
