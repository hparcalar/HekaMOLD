namespace Heka.DataAccess.Context
{
    public static class LSabit
    {

        #region ORDERUPLOADPONTTYPE
        public static string GET_ABROAD_EXPORT { get { return "Yurt Dışı/İhracat"; } }
        public static string GET_DOMESTIC { get { return "Yurt İçi"; } }
        public static string GET_ABROAD_IMPORT { get { return "Yurt Dışı/İthalat"; } }
        public static string GET_DOMESTIC_TRASFER { get { return "Yurt içi Transfer"; } }
        public static string GET_ABROAD_TRASFER { get { return "Yurt Dışı Transfer"; } }
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
        #endregion
    }
}
