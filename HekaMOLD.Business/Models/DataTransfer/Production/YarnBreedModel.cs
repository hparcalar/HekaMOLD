using HekaMOLD.Business.Base;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class YarnBreedModel : IDataObject
    {
        public int Id { get; set; }
        public string YarnBreedCode { get; set; }
        public string YarnBreedName { get; set; }
    }
}
