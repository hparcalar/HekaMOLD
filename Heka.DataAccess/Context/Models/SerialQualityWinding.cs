using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public class SerialQualityWinding
    {
        public SerialQualityWinding()
        {
            this.SerialQualityWindingFault = new HashSet<SerialQualityWindingFault>();
        }
        public int Id { get; set; }
        public string SerialNo { get; set; }

        [ForeignKey("WorkOrderDetail")]
        public Nullable<int> WorkOrderDetailId { get; set; }
        [ForeignKey("WorkOrderSerial")]
        public Nullable<int> WorkOrderSerialId { get; set; }

        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }

        [ForeignKey("User")]
        public Nullable<int> OperatorId { get; set; }

        public Nullable<bool> IsOk { get; set; }
        public Nullable<int> FaultCount { get; set; }
        public string Explanation { get; set; }
        public Nullable<decimal> TotalMeters { get; set; }
        public Nullable<decimal> TotalQuantity { get; set; }

        public virtual WorkOrderDetail WorkOrderDetail { get; set; }
        public virtual WorkOrderSerial WorkOrderSerial { get; set; }
        public virtual User User { get; set; }

        [InverseProperty("SerialQualityWinding")]
        public ICollection<SerialQualityWindingFault> SerialQualityWindingFault { get; set; }
    }
}
