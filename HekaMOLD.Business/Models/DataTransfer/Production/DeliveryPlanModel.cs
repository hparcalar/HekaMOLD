﻿using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class DeliveryPlanModel : IDataObject
    {
        public int Id { get; set; }
        public int? WorkOrderDetailId { get; set; }
        public DateTime? PlanDate { get; set; }
        public int? OrderNo { get; set; }
        public int? PlanStatus { get; set; }
        public WorkOrderDetailModel WorkOrder { get; set; }

        #region VISUAL ELEMENTS
        public string PlanDateStr { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string FirmName { get; set; }
        public int? FirmId { get; set; }
        public decimal? Quantity { get; set; }
        #endregion
    }
}
