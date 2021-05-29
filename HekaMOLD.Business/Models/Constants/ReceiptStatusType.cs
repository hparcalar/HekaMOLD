using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Constants
{
    public enum ReceiptStatusType
    {
        Created = 0,
        InUse = 1,
        WaitingQualityApprove=2,
        QualityIsApproved=3,
        Blocked = 4,
        Closed = 5
    }
}
