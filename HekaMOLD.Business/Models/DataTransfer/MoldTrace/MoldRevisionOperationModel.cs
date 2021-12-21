using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.MoldTrace
{
    public class MoldRevisionOperationModel
    {
        public int Id { get; set; }
        public Nullable<int> ItemReceiptDetailId { get; set; }
        public Nullable<int> MoldId { get; set; }
        public string Explanation { get; set; }
        public Nullable<int> OperationStatus { get; set; }
        public Nullable<int> OperationResult { get; set; }
        public Nullable<DateTime> FinishDate { get; set; }
        public string ResultExplanation { get; set; }

        #region VISUAL ELEMENTS
        public string MoldCode { get; set; }
        public string MoldName { get; set; }
        #endregion
    }
}
