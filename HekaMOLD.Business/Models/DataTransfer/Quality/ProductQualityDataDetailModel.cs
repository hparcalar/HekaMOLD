using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Quality
{
    public class ProductQualityDataDetailModel
    {
        public int Id { get; set; }
        public int? ProductQualityDataId { get; set; }
        public int? ProductQualityPlanId { get; set; }
        public int? OrderNo { get; set; }
        public bool? CheckResult { get; set; }
        public decimal? NumericResult { get; set; }
        public bool? IsOk { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedUserId { get; set; }
        public bool NewDetail { get; set; }

        #region VISUAL ELEMENTS
        public string MoldTestFieldName { get; set; }
        #endregion
    }
}
