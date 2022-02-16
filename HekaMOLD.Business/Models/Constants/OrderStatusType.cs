using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Constants
{
    public enum OrderStatusType
    {
        Created = 0,
        Approved = 1,
        Cancelled = 2,
        Completed = 3,
        Planned = 4,
        Delivered = 5,
    }
}
