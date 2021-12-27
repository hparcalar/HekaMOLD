using HekaMOLD.Business.Base;

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
        //Punta
        public bool? Center { get; set; }
        //Karisim 
        public bool? Mix { get; set; }
        public int? YarnRecipeType { get; set; }
        public int? YarnLot { get; set; }
        public int? YarnColourId { get; set; }
        public int? FirmId { get; set; }
        public string FirmName { get; set; }
        public string YarnColourName { get; set; }
        public string YarnBreedName { get; set; }
        public string YarnRecipeTypeStr { get; set; }


    }
}
