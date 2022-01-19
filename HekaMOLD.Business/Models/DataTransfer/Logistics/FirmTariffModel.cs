using HekaMOLD.Business.Base;
using System;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
    public class FirmTariffModel : IDataObject
    {
        public int Id { get; set; }
        public decimal? LadametrePrice { get; set; }
        public decimal? MeterCupPrice { get; set; }
        public decimal? WeightPrice { get; set; }
        public int? ForexTypeId { get; set; }
        public int FirmId { get; set; }
        public int? UnitTypeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        #region VISUAL ELEMENTS
        public string ForexTypeCode { get; set; }
        public string UnitTypeCode { get; set; }
        public string FirmCode { get; set; }
        public string FirmName { get; set; }
        public string StartDatestr { get; set; }
        public string EndDatestr { get; set; }

        #endregion
    }
}
