using HekaMOLD.Business.Base;
using HekaMOLD.Business.Models.DataTransfer.Production;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class YarnRecipeModel : IDataObject
    {
        public int Id { get; set; }
        public string YarnRecipeCode { get; set; }
        public string YarnRecipeName { get; set; }
        public int? YarnBreedId { get; set; }
        public int? Denier { get; set; }
        //Katsayi
        public int? Factor { get; set; }
        //Bukum
        public int? Twist { get; set; }
        public string TwistDirection { get; set; }
        //Punta
        public int? CenterType { get; set; }
        //Karisim 
        public bool? Mix { get; set; }
        public int? YarnRecipeType { get; set; }
        public int? YarnLot { get; set; }
        public int? YarnColourId { get; set; }
        public int? FirmId { get; set; }
        public int? ItemId { get; set; }

        public YarnRecipeMixModel[] YarnRecipeMixes { get; set; }

        #region VISUAL ELEMENTS
        public string FirmName { get; set; }
        public int? YarnColourCode { get; set; }
        public string YarnColourName { get; set; }
        public string YarnBreedName { get; set; }
        public string YarnRecipeTypeStr { get; set; }
        public string CenterTypeStr { get; set; }
        #endregion

    }
}
