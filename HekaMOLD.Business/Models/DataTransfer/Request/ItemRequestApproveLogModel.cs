using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Request
{
    public class ItemRequestApproveLogModel
    {
        public int Id { get; set; }
        public int? ItemRequestId { get; set; }
        public int? OldRequestStatus { get; set; }
        public int? NewRequestStatus { get; set; }
        public int? ActorUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
