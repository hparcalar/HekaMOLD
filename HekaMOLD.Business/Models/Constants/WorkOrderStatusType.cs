using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Constants
{
    public enum WorkOrderStatusType
    {
        Created = 0,
        Planned = 1,
        OnHold = 2,
        InProgress = 3,
        Completed = 4,
        Delivered = 5,
        Cancelled = 6
    }
}
