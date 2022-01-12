using HekaMOLD.Business.Base;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class KnitYarnModel : IDataObject
    {
        public int Id { get; set; }

        public int YarnRecipeId { get; set; }

        public int FirmId { get; set; }

        public int ItemId { get; set; }

        public int? YarnType { get; set; }

        //Rapor Tel Sayisi
        public decimal? ReportWireCount { get; set; }

        //Metre Tel Sayisi
        public int? MeterWireCount { get; set; }
        public decimal? Gramaj { get; set; }
        public int? Density { get; set; }


        public string YarnRecipeCode { get; set; }
        public string YarnRecipeName { get; set; }
        public string FirmCode { get; set; }
        public string FirmName { get; set; }
        public int? Denier { get; set; }
        public string YarnTypeStr { get; set; }

        #region VISUAL ELEMENTS
        public bool NewDetail { get; set; }
        #endregion

    }
}
