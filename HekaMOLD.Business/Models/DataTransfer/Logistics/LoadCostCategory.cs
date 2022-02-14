using HekaMOLD.Business.Base;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
    public class LoadCostCategoryModel : IDataObject
    {
        public int Id { get; set; }
        public string LoadCostCategoryCode { get; set; }
        public string LoadCostCategoryName { get; set; }
    }
}
