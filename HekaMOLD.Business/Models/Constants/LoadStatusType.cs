namespace HekaMOLD.Business.Models.Constants
{
    public enum LoadStatusType
    {
        Created = 0,
        Cancelled = 1,
        Ready = 2,//hazır
        InWarehouse = 3,
        ToBeLoadedFromCustomer = 4,//Müşteriden alınacak
        AtDomesticCustoms = 5, //yurtici gumrukte
        InKapukule = 6, //Kapıkulede
        OnTheWayAbroad = 7, // Yurtdisi yolda
        AtCustomsAbroad = 8, // Yurtdisi Gumrukte
        BeingEmptied = 9, // Bosaltmada
        Emptied =10,
        Completed = 11,
        InLoading = 12,
        Loaded=13

    }
}
