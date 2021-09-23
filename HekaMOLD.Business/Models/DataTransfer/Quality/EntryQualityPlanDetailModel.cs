using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Quality
{
    public class EntryQualityPlanDetailModel
    {
        public int Id { get; set; }
        public int? EntryQualityPlanId { get; set; }
        public string CheckProperty { get; set; }
        public bool? IsRequired { get; set; }
        public int? OrderNo { get; set; }
        public string PeriodType { get; set; }
        public string AcceptanceCriteria { get; set; }
        public string ControlDevice { get; set; }
        public string Method { get; set; }
        public string Responsible { get; set; }
        public decimal? SampleQuantity { get; set; }
        public bool NewDetail { get; set; }
    }
}
