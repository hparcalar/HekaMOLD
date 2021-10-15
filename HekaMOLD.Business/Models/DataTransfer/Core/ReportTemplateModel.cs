using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class ReportTemplateModel
    {
        public int Id { get; set; }
        public int? ReportType { get; set; }
        public string ReportCode { get; set; }
        public string ReportName { get; set; }
        public string FileName { get; set; }
        public bool? IsActive { get; set; }

        #region VISUAL ELEMENTS
        public string ReportTypeStr { get; set; }
        #endregion
    }
}
