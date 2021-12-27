using HekaMOLD.Business.Base;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class ItemQualityTypeModel : IDataObject
    {

        public int Id { get; set; }
        public string ItemQualityTypeCode { get; set; }
        public string ItemQualityTypeName { get; set; }

    }
}
