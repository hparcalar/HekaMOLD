using System;

namespace Heka.DataAccess.Context.Models
{
    public partial class SerialFaultType
    {
        public SerialFaultType()
        {

        }
        public int Id { get; set; }
        public string FaultCode { get; set; }
        public string FaultName { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
    }
}
