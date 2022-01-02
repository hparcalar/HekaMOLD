using HekaMOLD.Business.Base;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class YarnColourModel : IDataObject
    {
        public int Id { get; set; }
        public int YarnColourCode { get; set; }
        public string YarnColourName { get; set; }
        public int YarnColourGroupId { get; set; }
        public string GroupName { get; set; }

    }
}
