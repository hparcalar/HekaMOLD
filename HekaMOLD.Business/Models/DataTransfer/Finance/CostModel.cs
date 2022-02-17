using HekaMOLD.Business.Base;

namespace HekaMOLD.Business.Models.DataTransfer.Finance
{
    public class CostModel : IDataObject
    {
        public int? Id { get; set; }
        public string CostCode { get; set; }
        public string CostName { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? OverallTotal { get; set; }
        public string Explanation { get; set; }
        public int? ForexTypeId { get; set; }
        public int? UnitTypeId { get; set; }
        public int? CostCategoryId { get; set; }
        #region VISUAL ELEMENTS

        public string ForexTypeCode { get; set; }
        public string UnitTypeCode { get; set; }
        public string CostCategoryCode { get; set; }
        public string CostCategoryName { get; set; }
        #endregion
    }
}
