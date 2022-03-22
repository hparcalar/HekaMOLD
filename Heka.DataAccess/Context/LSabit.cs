namespace Heka.DataAccess.Context
{
    public static class LSabit
    {

        #region ORDERUPLOADPONTTYPE
        public static string GET_EXPORT { get { return "İhracat"; } }
        public static string GET_IMPORT { get { return "İthalat"; } }
        public static string GET_DOMESTIC { get { return "Yurt İçi"; } }
        public static string GET_TRASFER { get { return "Transit"; } }
        #endregion

        #region ORDERUPLOADPONTTYPE
        public static string GET_FROMCUSTOMER { get { return "Müşteriden yükleme"; } }
        public static string GET_FROMWAREHOUSE { get { return "Depodan Yükleme"; } }
        #endregion

        #region ORDERUPLOADTYPE
        public static string GET_GRUPAJ { get { return "Grupaj"; } }
        public static string GET_COMPLATE { get { return "Komple"; } }
        #endregion

        #region CALCULATIONTYPE
        public static string GET_VOLUMETRIC { get { return "Ağırlık"; } }
        public static string GET_WEIGHTTED { get { return "Hacim"; } }
        public static string GET_LADAMETRE { get { return "Ladametre"; } }
        public static string GET_COMPLET { get { return "Komple"; } }
        public static string GET_MINIMUM { get { return "Minimum"; } }

        #endregion

        #region CODECOUNTERTYPE
        public static int GET_OWN_EXPORT { get { return 1; } }
        public static int GET_OWN_IMPORT { get { return 2; } }
        public static int GET_OWN_DOMESTIC { get { return 3; } }
        public static int GET_OWN_TRANSIT { get { return 4; } }
        public static int GET_RENTAL_EXPORT { get { return 5; } }
        public static int GET_RENTAL_IMPORT { get { return 6; } }
        public static int GET_RENTAL_DOMESTIC { get { return 7; } }
        public static int GET_RENTAL_TRANSIT { get { return 8; } }
        #endregion

        #region VOYAGECOSTTYPE
        public static int GET_VOYAGECOST_CASH { get { return 1; } }
        public static int GET_VOYAGECOST_CREDIT { get { return 2; } }
        #endregion

        #region FOREXTYPE
        public static int GET_FOREXTYPE_TL { get { return 1; } }
        public static int GET_FOREXTYPE_USD { get { return 2; } }
        public static int GET_FOREXTYPE_EURO { get { return 3; } }
        #endregion
    }
}
