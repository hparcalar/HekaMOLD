﻿using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class UsageDocumentModel : IDataObject
    {
        public int Id { get; set; }
        public string DocumentTitle { get; set; }
        public string DocumentData { get; set; }
    }
}
