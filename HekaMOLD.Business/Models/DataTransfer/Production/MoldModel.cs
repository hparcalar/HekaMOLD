using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class MoldModel : IDataObject
    {
        public int Id { get; set; }
        public string MoldCode { get; set; }
        public string MoldName { get; set; }
        public int? PlantId { get; set; }
        public bool? IsActive { get; set; }

        #region VISUAL ELEMENTS
        public string CreatedDateStr { get; set; }
        #endregion
    }
}
