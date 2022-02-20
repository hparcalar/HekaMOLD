namespace Heka.DataAccess.Context
{
    public partial class CodeCounter
    {
        public CodeCounter()
        {

        }
        public int Id { get; set; }
        public int? CounterType { get; set; }
        public int? FirstValue { get; set; }
        public int? Export { get; set; }
        public int? Import { get; set; }
        public int? Domestic { get; set; }
        public int? Transit { get; set; }
        public int? IncrementsValue { get; set; }

    }
}
