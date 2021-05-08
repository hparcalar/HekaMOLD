using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Request
{
    public class ItemRequestModel : IDataObject
    {
        public int Id { get; set; }
        public string RequestNo { get; set; }
        public int? RequestType { get; set; }
        public DateTime? DateOfNeed { get; set; }
        public int? RequestStatus { get; set; }
        public int? PlantId { get; set; }
        public string Explanation { get; set; }

        #region VISUAL ELEMENTS
        public string DateOfNeedStr { get; set; }
        public string CreatedDateStr { get; set; }
        public string RequestStatusStr { get; set; }
        public ItemRequestDetailModel[] Details { get; set; }
        #endregion
    }
}
