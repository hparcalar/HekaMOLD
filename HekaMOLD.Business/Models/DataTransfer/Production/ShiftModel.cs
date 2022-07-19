using HekaMOLD.Business.Models.DataTransfer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class ShiftModel
    {
        public int Id { get; set; }
        public string ShiftCode { get; set; }
        public string ShiftName { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public bool? IsActive { get; set; }
        public int? ShiftChiefId { get; set; }

        #region VISUAL ELEMENTS
        public string StartTimeStr { get; set; }
        public string EndTimeStr { get; set; }
        public DateTime? ShiftBelongsToDate { get; set; }
        public string ChiefCode { get; set; }
        public string ChiefName { get; set; }

        public UserModel[] Users { get; set; }
        #endregion
    }
}
