﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Authentication
{
    public class UserAuthTypeModel
    {
        public int Id { get; set; }
        public string AuthTypeCode { get; set; }
        public string AuthTypeName { get; set; }
    }
}
