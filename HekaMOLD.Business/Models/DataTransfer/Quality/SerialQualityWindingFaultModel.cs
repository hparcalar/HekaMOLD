using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Quality
{
    public class SerialQualityWindingFaultModel
    {
        public int Id { get; set; }
        public Nullable<int> SerialQualityWindingId { get; set; }
        public Nullable<int> FaultId { get; set; }

        public Nullable<decimal> CurrentMeter { get; set; }
        public Nullable<decimal> EndMeter { get; set; }
        public Nullable<decimal> CurrentQuantity { get; set; }
        public Nullable<decimal> EndQuantity { get; set; }
        public string Explanation { get; set; }
        public Nullable<int> OperatorId { get; set; }

        public Nullable<DateTime> FaultDate { get; set; }
        public int? FaultStatus { get; set; }

        #region VISUAL ELEMENTS
        public string FaultCode { get; set; }
        public string FaultName { get; set; }
        public string OperatorName { get; set; }
        public string FaultDateStr { get; set; }
        #endregion
    }
}
