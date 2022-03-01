using HekaMOLD.Business.Models.Constants;
using System.Collections.Generic;

namespace HekaMOLD.Business.Models.Dictionaries
{
    public class DictVoyageStatus
    {
        public static Dictionary<VoyageStatus, string> Values
    = new Dictionary<VoyageStatus, string>()
{
            { VoyageStatus.Created, "Sefer Oluşturuldu" },
            { VoyageStatus.Approved, "Sefer Onaylandı" },
            { VoyageStatus.Cancelled, "Sefer İptal Edildi" },
            { VoyageStatus.Ready, "Hazır Bekliyor" },
            { VoyageStatus.InWarehouse, "Sefer Depoda" },
            { VoyageStatus.ToBeLoadedFromCustomer, "Müşteriden Alınacak" },
            { VoyageStatus.AtDomesticCustoms, "Yurtiçi Gümrükte"},
            { VoyageStatus.InKapukule, "Kapıkulede"},
            { VoyageStatus.OnTheWayAbroad, "Yurtdışı Yolda"},
            { VoyageStatus.AtCustomsAbroad, "Yurtdışı Gümrükte"},
            { VoyageStatus.BeingEmptied, "Boşaltmada"},
            { VoyageStatus.Emptied, "Boşaltıldı"},
            { VoyageStatus.InLoading, "Sefer Yüklemede"},
            { VoyageStatus.Loaded, "Sefer Yüklendi"},
            { VoyageStatus.Completed, "Tamamlandı"},
};

    }
}
