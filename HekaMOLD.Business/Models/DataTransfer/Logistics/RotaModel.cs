using HekaMOLD.Business.Base;
using System;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
    public class RotaModel : IDataObject
    {
        public int Id { get; set; }

        public Nullable<int> CityStartId { get; set; }
        public Nullable<int> CityEndId { get; set; }

        public int KmHour { get; set; }
        public byte[] ProfileImage { get; set; }

        public string ProfileImageBase64 { get; set; }
        public string CityStartName { get; set; }
        public string CityStartPostCode { get; set; }
        public string CityEndName { get; set; }
        public string CityEndPostCode { get; set; }
    }
}
