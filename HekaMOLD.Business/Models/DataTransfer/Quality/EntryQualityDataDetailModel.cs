using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Quality
{
    public class EntryQualityDataDetailModel
    {
        public int Id { get; set; }
        public int? EntryQualityDataId { get; set; }
        public int? EntryQualityPlanDetailId { get; set; }
        public int? OrderNo { get; set; }
        public decimal? SampleQuantity { get; set; }
        public string FaultExplanation { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedUserId { get; set; }
        public bool NewDetail { get; set; }
    }
}
