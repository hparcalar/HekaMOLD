using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Quality
{
    public class EntryQualityDataModel : IDataObject
    {
        public int Id { get; set; }
        public int? QualityPlanId { get; set; }
        public int? QualityPlanDetailId { get; set; }
        public bool? IsOk { get; set; }
        public string Explanation { get; set; }
        public int? ItemEntryDetailId { get; set; }
        public string ItemLot { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public int? ItemId { get; set; }
        public int? FirmId { get; set; }
        public string WaybillNo { get; set; }
        public decimal? EntryQuantity { get; set; }
        public decimal? CheckedQuantity { get; set; }
        public string LotNumbers { get; set; }
        public decimal? SampleQuantity { get; set; }

        public EntryQualityDataDetailModel[] Details { get; set; }

        #region VISUAL ELEMENTS
        public string CreatedDateStr { get; set; }
        public string FirmName { get; set; }
        #endregion
    }
}
