using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public class SerialQualityWindingFault
    {
        public int Id { get; set; }

        [ForeignKey("SerialQualityWinding")]
        public Nullable<int> SerialQualityWindingId { get; set; }

        [ForeignKey("SerialFaultType")]
        public Nullable<int> FaultId { get; set; }

        public Nullable<decimal> CurrentMeter { get; set; }
        public Nullable<decimal> EndMeter { get; set; }
        public Nullable<decimal> CurrentQuantity { get; set; }
        public Nullable<decimal> EndQuantity { get; set; }
        public string Explanation { get; set; }

        [ForeignKey("User")]
        public Nullable<int> OperatorId { get; set; }

        public Nullable<DateTime> FaultDate { get; set; }
        public Nullable<int> FaultStatus { get; set; }

        public virtual SerialQualityWinding SerialQualityWinding { get; set; }
        public virtual SerialFaultType SerialFaultType { get; set; }
        public virtual User User { get; set; }
    }
}
