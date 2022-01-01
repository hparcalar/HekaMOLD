using HekaMOLD.Business.Base;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class YarnRecipeMixModel : IDataObject
    {
        public int Id { get; set; }
        public int YarnRecipeId { get; set; }
        public int? YarnBreedId { get; set; }
        //Yuzde Oran
        public decimal? Percentage { get; set; }

        #region VISUAL ELEMENTS
        public string YarnRecipeCode { get; set; }
        public string YarnRecipeName { get; set; }
        public string YarnBreedCode { get; set; }
        public string YarnBreedName { get; set; }
        public bool NewDetail { get; internal set; }
        #endregion
    }
}