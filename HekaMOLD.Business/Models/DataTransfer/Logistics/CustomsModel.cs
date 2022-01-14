using HekaMOLD.Business.Base;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
   public class CustomsModel : IDataObject
    {
        public int Id { get; set; }
        public string CustomsCode { get; set; }
        public string CustomsName { get; set; }
    }
}
