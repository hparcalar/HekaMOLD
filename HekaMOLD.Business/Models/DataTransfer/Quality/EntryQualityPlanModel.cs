using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Quality
{
    public class EntryQualityPlanModel
    {
        public int Id { get; set; }
        public string QualityControlCode { get; set; }
        public int? OrderNo { get; set; }
        public int? ItemGroupId { get; set; }
        public int? ItemCategoryId { get; set; }
        public string ItemGroupText { get; set; }
        public string PeriodType { get; set; }
        public string AcceptanceCriteria { get; set; }
        public string ControlDevice { get; set; }
        public string Method { get; set; }
        public string Responsible { get; set; }
        public EntryQualityPlanDetailModel[] Details { get; set; }

        #region VISUAL ELEMENTS
        public string ItemGroupCode { get; set; }
        public string ItemGroupName { get; set; }
        public string ItemCategoryCode { get; set; }
        public string ItemCategoryName { get; set; }
        public string CheckPropertyList { get; set; }
        #endregion
    }
}
