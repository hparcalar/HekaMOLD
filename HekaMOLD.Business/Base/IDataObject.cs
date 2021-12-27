using System;

namespace HekaMOLD.Business.Base
{
    public class IDataObject
    {
        public DateTime? CreatedDate { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedUserId { get; set; }
    }
}
