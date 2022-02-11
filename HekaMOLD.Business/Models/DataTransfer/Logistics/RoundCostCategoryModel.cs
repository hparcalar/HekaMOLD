using HekaMOLD.Business.Base;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
    public class RoundCostCategoryModel : IDataObject
    {
        public int Id { get; set; }
        public string RoundCostCategoryCode { get; set; }
        public string RoundCostCategoryName { get; set; }
    }
}
