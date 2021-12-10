﻿using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class ItemModel : IDataObject
    {
        public int Id { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public int? ItemType { get; set; }
        public int? ItemCategoryId { get; set; }
        public int? ItemGroupId { get; set; }
        public int? SupplierFirmId { get; set; }
        public int? PlantId { get; set; }
        public int? MoldId { get; set; }
        //Desen
        public int? Pattern { get; set; }
        //Ham
        public int? CrudeWidth { get; set; }
        public decimal? CrudeGramaj { get; set; }
        //Mamul En
        public int? ProductWidth { get; set; }
        public decimal? ProductGramaj { get; set; }
        //Cozgu Tel sayisi
        public int? WarpWireCount { get; set; }
        public decimal? MeterGramaj { get; set; }
        //Kesme	
        public string Cutting { get; set; }
        //Boyahane
        public string Dyehouse { get; set; }
        //konfeksiyon
        public string Apparel { get; set; }
        //Kursun
        public string Bullet { get; set; }
        public int? CombWidth { get; set; }
        //Atki Rapor Boyu
        public int? WeftReportLength { get; set; }
        //Cozgu Rapor Boyu
        public int? WarpReportLength { get; set; }
        //Atki Sikligi
        public int? WeftDensity { get; set; }

        public int? MachineId { get; set; }

        public int? ItemQualityTypeId { get; set; }

        public ItemWarehouseModel[] Warehouses { get; set; }
        public ItemUnitModel[] Units { get; set; }
        public ItemLiveStatusModel[] LiveStatus { get; set; }
        public decimal? TotalInQuantity { get; set; }
        public decimal? TotalOutQuantity { get; set; }
        public decimal? TotalOverallQuantity { get; set; }

        #region VISUAL ELEMENTS
        public string ItemTypeStr { get; set; }
        public string CategoryName { get; set; }
        public string GroupName { get; set; }
        #endregion
    }
}
