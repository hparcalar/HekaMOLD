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
        public int CheckType { get; set; } // 1: checkbox, 2: input by tolerance (number)
        public decimal? ToleranceMin { get; set; }
        public decimal? ToleranceMax { get; set; }
        public string MoldTestFieldName { get; set; }
        public bool? Display { get; set; }

    }
}
