using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Quality
{
    public class ProductQualityPlanModel
    {
        public int Id { get; set; }
        public string ProductQualityCode { get; set; }
        public string CheckProperties { get; set; }
        public string PeriodType { get; set; }
        public string AcceptanceCriteria { get; set; }
        public string ControlDevice { get; set; }
        public string Method { get; set; }
        public string Responsible { get; set; }
        public int? OrderNo { get; set; }
    }
}
