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
        public int? OwnExport { get; set; }
        public int? RentalExport { get; set; }
        public int? OwnImport { get; set; }
        public int? RentalImport { get; set; }
        public int? OwnDomestic { get; set; }
        public int? RentalDomestic { get; set; }
        public int? OwnTransit { get; set; }
        public int? RentalTransit { get; set; }
        public int? IncrementsValue { get; set; }

    }
}
