﻿using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Receipt
{
    public class ItemSerialModel : IDataObject
    {
        public int Id { get; set; }
        public int? ItemId { get; set; }
        public int? ItemReceiptDetailId { get; set; }
        public string SerialNo { get; set; }
        public int? ReceiptType { get; set; }
        public int? SerialStatus { get; set; }
        public decimal? FirstQuantity { get; set; }
        public decimal? LiveQuantity { get; set; }
        public int? WorkOrderDetailId { get; set; }
        public int? InPackageQuantity { get; set; }
        public int? SerialType { get; set; }

        #region VISUAL ELEMENTS
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string CreatedDateStr { get; set; }
        public string MachineCode { get; set; }
        public string MachineName { get; set; }
        public string FirmCode { get; set; }
        public string FirmName { get; set; }
        #endregion
    }
}