﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Virtual
{
    public class AllocatedPrintDataModel
    {
        public int? WorkOrderDetailId { get; set; }
        public string Code { get; set; }
        public int? ReportTemplateId { get; set; }
    }
}
