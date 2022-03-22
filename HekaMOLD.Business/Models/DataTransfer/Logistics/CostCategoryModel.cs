using HekaMOLD.Business.Base;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
    public class CostCategoryModel : IDataObject
    {
        public int Id { get; set; }
        public string CostCategoryCode { get; set; }
        public string CostCategoryName { get; set; }
    }
}
