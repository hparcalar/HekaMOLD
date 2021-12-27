﻿using HekaMOLD.Business.Base;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class FirmModel : IDataObject
    {
        public int Id { get; set; }
        public string FirmCode { get; set; }
        public string FirmName { get; set; }
        public string FirmTitle { get; set; }
        public int? PlantId { get; set; }
        public int? FirmType { get; set; }
        public string Explanation { get; set; }
        public string Phone { get; set; }
        public string Gsm { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Author { get; set; }
        public string Email { get; set; }
        public string TaxNo { get; set; }
        public string TaxOffice { get; set; }
        public bool? IsApproved { get; set; }
        public FirmAuthorModel[] Authors { get; set; }

        #region VISUAL ELEMENTS
        public string FirmTypeStr { get; set; }
        #endregion
    }
}
