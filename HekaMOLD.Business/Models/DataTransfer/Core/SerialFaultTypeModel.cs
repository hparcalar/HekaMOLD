using HekaMOLD.Business.Base;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class SerialFaultTypeModel : IDataObject
    {
        public int Id { get; set; }
        public string FaultCode { get; set; }
        public string FaultName { get; set; }
    }
}
