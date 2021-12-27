using HekaMOLD.Business.Base;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
   public class ItemKnitDensityModel : IDataObject
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string YarnRecipeType { get; set; }
        public int Density { get; set; }

        public string ItemCode { get; set; }
        public string YarnRecipeTypeStr { get; set; }
    }
}
