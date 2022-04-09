﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Order
{
    public class ItemOrderSheetModel
    {
        public int Id { get; set; }
        public Nullable<int> ItemOrderId { get; set; }
        public string SheetName { get; set; }
        public Nullable<int> SheetNo { get; set; }
        public byte[] SheetVisual { get; set; }
        public Nullable<DateTime> PerSheetTime { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> Thickness { get; set; }
        public Nullable<decimal> Eff { get; set; }
        public Nullable<int> SheetStatus { get; set; }
        public Nullable<int> SheetItemId { get; set; }

        #region VISUAL ELEMENTS
        public string SheetVisualStr { get; set; }
        public int MachineId { get; set; }
        public string OrderNo { get; set; }
        public string OrderDateStr { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string SheetProgramName { get; set; }
        public string FirmName { get; set; }
        public string DeadlineDateStr { get; set; }
        #endregion
    }
}
