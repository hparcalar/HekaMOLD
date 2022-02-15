using HekaMOLD.Business.Base;
using HekaMOLD.Business.Models.Virtual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class WorkOrderDetailModel : IDataObject
    {
        public int Id { get; set; }
        public int? WorkOrderId { get; set; }
        public int? ItemId { get; set; }
        public int? MoldId { get; set; }
        public int? DyeId { get; set; }
        public int? MachineId { get; set; }
        public int? MoldTestId { get; set; }
        public int? InflationTimeSeconds { get; set; }
        public decimal? RawGr { get; set; }
        public decimal? RawGrToleration { get; set; }
        public decimal? Quantity { get; set; }
        public int? WorkOrderStatus { get; set; }
        public int? InPalletPackageQuantity { get; set; }
        public int? InPackageQuantity { get; set; }
        public int? SaleOrderDetailId { get; set; }
        public int? QualityStatus { get; set; }
        public int? MoldTestCycle { get; set; }
        public DateTime? DeliveryPlanDate { get; set; }
        public string DeliveryPlanDateStr { get; set; }
        public int? WorkOrderType { get; set; }
        public string TrialProductName { get; set; }
        public string LabelConfig { get; set; }

        #region VISUAL ELEMENTS
        public bool NewDetail { get; set; }
        public string Explanation { get; set; }
        public string ItemOrderDocumentNo { get; set; }
        public int? MachinePlanOrderNo { get; set; }
        public string WorkOrderStatusStr { get; set; }
        public string WorkOrderDateStr { get; set; }
        public string WorkOrderNo { get; set; }
        public string FirmCode { get; set; }
        public string FirmName { get; set; }
        public string QualityStatusText { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string DyeCode { get; set; }
        public string RalCode { get; set; }
        public string DyeName { get; set; }
        public string MachineCode { get; set; }
        public string MachineName { get; set; }
        public string MoldCode { get; set; }
        public string MoldName { get; set; }
        public string SaleOrderDocumentNo { get; set; }
        public string SaleOrderReceiptNo { get; set; }
        public string SaleOrderDate { get; set; }
        public string SaleOrderDeadline { get; set; }
        public int CompleteQuantity { get; set; }
        public int CompleteQuantitySingleProduct { get; set; }
        public decimal? WastageQuantity { get; set; }
        public string ProductDescription { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public string OrderDeadline { get; set; }
        public WorkOrderSerialModel[] Serials { get; set; }
        public LabelConfigModel LabelConfigData
        {
            get
            {
                if (!string.IsNullOrEmpty(this.LabelConfig))
                {
                    try
                    {
                        return 
                            JsonConvert.DeserializeObject<LabelConfigModel>(this.LabelConfig);
                    }
                    catch (Exception)
                    {

                    }
                }

                return null;
            }
            set
            {
                try
                {
                    if (value != null)
                    {
                        this.LabelConfig = JsonConvert.SerializeObject(value);
                    }
                    else
                        this.LabelConfig = null;
                }
                catch (Exception)
                {

                }
            }
        }
        #endregion
    }
}
