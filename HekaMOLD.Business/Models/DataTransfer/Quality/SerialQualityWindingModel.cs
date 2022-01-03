using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Quality
{
    public class SerialQualityWindingModel
    {
        public int Id { get; set; }
        public string SerialNo { get; set; }
        public Nullable<int> WorkOrderDetailId { get; set; }
        public Nullable<int> WorkOrderSerialId { get; set; }

        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public Nullable<int> OperatorId { get; set; }

        public Nullable<bool> IsOk { get; set; }
        public Nullable<int> FaultCount { get; set; }
        public string Explanation { get; set; }
        public Nullable<decimal> TotalMeters { get; set; }
        public Nullable<decimal> TotalQuantity { get; set; }

        #region VISUAL ELEMENTS
        public string StartDateStr { get; set; }
        public string EndDateStr { get; set; }
        public string OperatorName { get; set; }
        #endregion
    }
}
