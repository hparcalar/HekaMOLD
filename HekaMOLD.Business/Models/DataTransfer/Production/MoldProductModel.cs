using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class MoldProductModel
    {
        public int Id { get; set; }
        public int? MoldId { get; set; }
        public int? ProductId { get; set; }
        public int? LineNumber { get; set; }
        public bool NewDetail { get; set; }

        #region VISUAL ELEMENTS
        public string MoldCode { get; set; }
        public string MoldName { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        #endregion
    }
}
