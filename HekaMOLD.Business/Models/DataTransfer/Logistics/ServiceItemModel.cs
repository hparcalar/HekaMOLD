using HekaMOLD.Business.Base;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
    public class ServiceItemModel : IDataObject
    {
        public int Id { get; set; }
        public string ServiceItemCode { get; set; }
        public string ServiceItemName { get; set; }
    }
}
