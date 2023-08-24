using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class UserWorkOrderHistoryModel
    {
        public int Id { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> WorkOrderDetailId { get; set; }
        public Nullable<int> MachineId { get; set; }
        public Nullable<decimal> StartQuantity { get; set; }
        public Nullable<decimal> EndQuantity { get; set; }
        public Nullable<decimal> FinishedQuantity { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }


        public string StartDateStr { get; set; }
        public string EndDateStr { get; set; }
        public string UserName { get; set; }
    }
}
