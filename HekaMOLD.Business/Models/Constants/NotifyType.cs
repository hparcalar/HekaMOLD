using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Constants
{
    public enum NotifyType
    {
        ItemRequestWaitForApproval=1,
        ItemRequestIsApproved=2,
        ItemOrderWaitForApproval=3,
        ItemOrderIsApproved=4,
        IncidentCreated=5,
        PostureCreated=6,
        IncidentTouched=7,
        IncidentResolved=8,
        PostureTouched=9,
        PostureResolved=10,
        ItemAtMinimumInWarehouse=11,
        ItemAtMaximumInWarehouse=12,
        ItemLoadWaitForApproval=13,
        ItemLoadIsApproved = 14,
    }
}
