using HekaMOLD.Business.Base;
using System;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
  public  class DriverModel:IDataObject
    {

        public int Id { get; set; }
        public string DriverName { get; set; }
        public string DriverSurName { get; set; }
        public string Tc { get; set; }
        public string PlaceOfBirth { get; set; }
        public DateTime? BirthDate { get; set; }
        public string PassportNo { get; set; }
        public int? CountryId { get; set; }
        public int? VisaType { get; set; }
        public DateTime? VisaStartDate { get; set; }
        public DateTime? VisaEndDate { get; set; }
        public byte[] ProfileImage { get; set; }

        public string CountryName { get; set; }
        public string ProfileImageBase64 { get; set; }
        public string BirthDateStr { get; set; }
        public string VisaStartDateStr { get; set; }
        public string VisaEndDateStr { get; set; }
        public string VisaTypeStr { get; set; }

    }
}
