using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public partial class MoldRevisionOperation
    {
        public MoldRevisionOperation()
        {

        }

        public int Id { get; set; }

        [ForeignKey("ItemReceiptDetail")]
        public Nullable<int> ItemReceiptDetailId { get; set; }

        [ForeignKey("Mold")]
        public Nullable<int> MoldId { get; set; }
        public string Explanation { get; set; }
        public Nullable<int> OperationStatus { get; set; }
        public Nullable<int> OperationResult { get; set; }
        public Nullable<DateTime> FinishDate { get; set; }
        public string ResultExplanation { get; set; }

        public virtual ItemReceiptDetail ItemReceiptDetail { get; set; }
        public virtual Mold Mold { get; set; }
    }
}
