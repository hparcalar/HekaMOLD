using HekaMOLD.Business.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Dictionaries
{
    public class DictReportType
    {
        public static Dictionary<ReportType, string> Values
            = new Dictionary<ReportType, string>()
        {
            { ReportType.DeliverySerialList, "Sevkiyat Çeki Listesi" },
            { ReportType.ItemReceipt, "İrsaliye" },
            { ReportType.WorkOrder, "İş Emri" },
            { ReportType.WorkOrderDetail, "İş Emri Satır Formu" },
            { ReportType.WorkOrderSerial, "Ürün Etiketi" },
        };
    }
}
