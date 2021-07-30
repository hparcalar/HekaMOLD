﻿using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class WorkOrderSerialModel : IDataObject
    {
        public int Id { get; set; }
        public int? WorkOrderDetailId { get; set; }
        public int? WorkOrderId { get; set; }
        public string SerialNo { get; set; }
        public int? SerialType { get; set; }
        public bool? IsGeneratedBySignal { get; set; }
        public int? SerialStatus { get; set; }
        public decimal? FirstQuantity { get; set; }
        public decimal? LiveQuantity { get; set; }
        public int? InPackageQuantity { get; set; }

        #region VISUAL ELEMENTS

        #endregion
    }
}