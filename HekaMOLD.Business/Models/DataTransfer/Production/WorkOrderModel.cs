﻿using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class WorkOrderModel : IDataObject
    {
        public int Id { get; set; }
        public string WorkOrderNo { get; set; }
        public string DocumentNo { get; set; }
        public DateTime? WorkOrderDate { get; set; }
        public int? FirmId { get; set; }
        public int? PlantId { get; set; }
        public int? WorkOrderStatus { get; set; }
        public string Explanation { get; set; }
        public int? WorkOrderType { get; set; }
        public string TrialFirmName { get; set; }
        public int? WorkOrderCategoryId { get; set; }
        public WorkOrderDetailModel[] Details { get; set; }

        #region VISUAL ELEMENTS
        public string WorkOrderStatusStr { get; set; }
        public string WorkOrderDateStr { get; set; }
        public string FirmCode { get; set; }
        public string FirmName { get; set; }
        public string WorkOrderCategoryCode { get; set; }
        public string WorkOrderCategoryName { get; set; }
        #endregion
    }
}
