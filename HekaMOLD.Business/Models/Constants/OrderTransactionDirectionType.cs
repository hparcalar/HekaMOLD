namespace HekaMOLD.Business.Models.Constants
{
    public enum OrderTransactionDirectionType
    {
        //Yurt Dışı/İhracat
        AbroadExport = 1,
        //Yurt Dışı/İthalat
        AbroadImport = 2,
        //Yurt İçi
        Domestic = 3,
        //Yurt içi Transfer
        DomesticTransfer = 4,
        //Yurt Dışı Transfer
        AbroadTransfer = 5,
    }
}
