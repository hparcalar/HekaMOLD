using HekaMOLD.Business.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Dictionaries
{
    public class DictLoadStatusType
    {
        public static Dictionary<LoadStatusType, string> Values
    = new Dictionary<LoadStatusType, string>()
{
            { LoadStatusType.Created, "Yük Oluşturuldu" },
            { LoadStatusType.Cancelled, "Yük İptal Edildi" },
            { LoadStatusType.Ready, "Hazır Bekliyor" },
            { LoadStatusType.InWarehouse, "Yük Depoda" },
            { LoadStatusType.ToBeLoadedFromCustomer, "Müşteriden Alınacak" },
            { LoadStatusType.AtDomesticCustoms, "Yurtiçi Gümrükte"},
            { LoadStatusType.InKapukule, "Kapıkulede"},
            { LoadStatusType.OnTheWayAbroad, "Yurtdışı Yolda"},
            { LoadStatusType.AtCustomsAbroad, "Yurtdışı Gümrükte"},
            { LoadStatusType.BeingEmptied, "Boşaltmada"},
            { LoadStatusType.Emptied, "Boşaltıldı"},
            { LoadStatusType.Completed, "Tamamlandı"},
            { LoadStatusType.InLoading, "Yüklemede"},
            { LoadStatusType.Loaded, "Yüklendi"},


};

    }
}
