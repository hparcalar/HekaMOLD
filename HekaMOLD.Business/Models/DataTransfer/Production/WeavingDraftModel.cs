using HekaMOLD.Business.Base;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
   public class WeavingDraftModel : IDataObject
    {
        public int Id { get; set; }
        public string WeavingDraftCode { get; set; }
        public int? MachineBreedId { get; set; }
        public string NumberOfFramaes { get; set; }
        public string Report { get; set; }
        public int? PlatinumNumber { get; set; }
        public string JakarReport { get; set; }
        #region VISUAL ELEMNTS
        public string MachineBreedCode { get; set; }
        public string MachineBreedName { get; set; }

        #endregion
    }
}
