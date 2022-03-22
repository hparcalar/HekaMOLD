using HekaMOLD.Business.Base;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
    public class VoyageCostModel : IDataObject
    {
        public int Id { get; set; }
        public int VoyageId { get; set; }
        public VoyageCostDetailModel[] VoyageCostDetails { get; set; }

        #region VISUAL ELEMENTS
        public string VoyageCode { get; set; }
        public string VoyageStatusStr { get; set; }
        public string TrailerPlate { get; set; }
        public string VoyageStartDateStr { get; set; }
        public string VoyageEndDateStr { get; set; }
        public string OrderTransationDirectionTypeStr { get; set; }
        #endregion
    }
}
