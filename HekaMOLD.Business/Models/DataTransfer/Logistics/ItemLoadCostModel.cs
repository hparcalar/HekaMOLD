using HekaMOLD.Business.Base;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
    public class ItemLoadCostModel : IDataObject
    {
        public int Id { get; set; }
        public int ItemLoadId { get; set; }
        public int LoadCostCategoryId { get; set; }
        public decimal? CostPrice { get; set; }
        public int? ForexTypeId { get; set; }
        public string Explanation { get; set; }

        #region VISULAL ELEMENTS
        public string ItemLoadCode { get; set; }
        public string LoadCostCategoryName { get; set; }
        public string ForexTypeCode { get; set; }
        #endregion
    }
}
