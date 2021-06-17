using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class MoldTestModel : IDataObject
    {
        public int Id { get; set; }
        public int? MachineId { get; set; }
        public DateTime? TestDate { get; set; }
        public string ProductDescription { get; set; }
        public int? RawMaterialId { get; set; }
        public string RawMaterialName { get; set; }
        public decimal? RawMaterialGr { get; set; }
        public decimal? RawMaterialTolerationGr { get; set; }
        public string RawMaterialGrText { get; set; }
        public int? InflationTimeSeconds { get; set; }
        public int? TotalTimeSeconds { get; set; }
        public int? DyeId { get; set; }
        public string DyeCode { get; set; }
        public string RalCode { get; set; }
        public string PackageDimension { get; set; }
        public int? InPackageQuantity { get; set; }
        public int? NutQuantity { get; set; }
        public string NutCaliber { get; set; }
        public int? InPalletPackageQuantity { get; set; }
        public int? ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int? MoldId { get; set; }
        public string MoldCode { get; set; }
        public string MoldName { get; set; }
        public int? PlantId { get; set; }

        #region VISUAL ELEMENTS
        public string TestDateStr { get; set; }
        public string MachineCode { get; set; }
        public string MachineName { get; set; }
        public string RawItemCode { get; set; }
        public string RawItemName { get; set; }
        #endregion
    }
}
