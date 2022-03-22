using HekaMOLD.Business.Models.Constants;
using System.Collections.Generic;

namespace HekaMOLD.Business.Models.Dictionaries
{
    public class DictVoyageStatus
    {
        public static Dictionary<VoyageStatus, string> Values
    = new Dictionary<VoyageStatus, string>()
{
            { VoyageStatus.Created, "Oluşturuldu" },
            { VoyageStatus.Approved, "Onaylandı" },
            { VoyageStatus.Cancelled, "İptal Edildi" },
            { VoyageStatus.Ready, "Hazır Bekliyor" },
            { VoyageStatus.InWarehouse, "Depoda" },
            { VoyageStatus.ToBeLoadedFromCustomer, "Müşteriden Yüklendi" },
            { VoyageStatus.AtDomesticCustoms, "Yurtiçi Gümrükte"},
            { VoyageStatus.InKapukule, "Kapıkulede"},
            { VoyageStatus.OnTheWayAbroad, "Yurtdışı Yolda"},
            { VoyageStatus.AtCustomsAbroad, "Yurtdışı Gümrükte"},
            { VoyageStatus.BeingEmptied, "Boşaltmada"},
            { VoyageStatus.Emptied, "Boşaltıldı"},
            { VoyageStatus.InLoading, "Yüklemede"},
            { VoyageStatus.Loaded, "Yüklendi"},
            { VoyageStatus.Completed, "Tamamlandı"},
};

    }
}
