using HekaMOLD.Business.Base;

namespace HekaMOLD.Business.Models.DataTransfer.Finance
{
    public class CostCategoryModel : IDataObject
    {
        public int Id { get; set; }
        public string CostCategoryCode { get; set; }
        public string CostCategoryName { get; set; }
        public string Explanation { get; set; }
    }
}
