﻿using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class RouteModel : IDataObject
    {
        public int Id { get; set; }
        public string RouteCode { get; set; }
        public string RouteName { get; set; }
        public int? PlantId { get; set; }
        public bool? IsActive { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? ForexId { get; set; }

        public RouteItemModel[] RouteItems { get; set; }

        #region VISUAL ELEMENTS
        public string ForexTypeCode { get; set; }
        #endregion
    }
}
