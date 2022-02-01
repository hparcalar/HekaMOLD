namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
    public class CodeCounterModel
    {
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
