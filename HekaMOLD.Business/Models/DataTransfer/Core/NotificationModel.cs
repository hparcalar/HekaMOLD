using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class NotificationModel
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? NotifyType { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public int? RecordId { get; set; }
        public int? SeenStatus { get; set; }
        public DateTime? SeenDate { get; set; }
        public bool? IsProcessed { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public DateTime? CreatedDate { get; set; }

        #region VISUAL ELEMENTS
        public string CreatedDateStr { get; set; }
        public string UserName { get; set; }
        #endregion
    }
}
