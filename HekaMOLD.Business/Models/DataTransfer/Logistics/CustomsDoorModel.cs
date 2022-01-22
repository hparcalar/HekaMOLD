using HekaMOLD.Business.Base;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
   public class CustomsDoorModel : IDataObject
    {
        public int Id { get; set; }
        public string CustomsDoorCode { get; set; }
        public string CustomsDoorName { get; set; }
    }
}
