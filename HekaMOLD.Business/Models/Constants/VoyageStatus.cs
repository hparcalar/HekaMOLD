using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Constants
{
    public enum VoyageStatus
    {
        Created = 0,
        Approved = 1,
        Ready = 2,//hazır
        InWarehouse = 3,
        ToBeLoadedFromCustomer = 4,//Müşteriden alınacak
        AtDomesticCustoms = 5, //yurtici gumrukte
        InKapukule = 6, //Kapıkulede
        OnTheWayAbroad = 7, // Yurtdisi yolda
        AtCustomsAbroad = 8, // Yurtdisi Gumrukte
        BeingEmptied = 9, // Bosaltmada
        Emptied = 10,
        InLoading = 11,
        Loaded = 12,
        Completed = 13,
        Cancelled = 14,


    }
}
