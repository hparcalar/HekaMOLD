using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Offers
{
    public class ItemOfferModel
    {
        public int Id { get; set; }
        public string OfferNo { get; set; }
        public int OfferType { get; set; }
        public Nullable<int> FirmId { get; set; }
        public Nullable<DateTime> OfferDate { get; set; }
        public string Explanation { get; set; }
        public Nullable<decimal> TotalQuantity { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
        public string Expiration { get; set; }
        public Nullable<int> PlantId { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        #region VISUAL ELEMENTS
        public bool HasAnyOrder { get; set; } = false;
        public ItemOfferDetailModel[] Details { get; set; }
        public string OfferDateStr { get; set; }
        public string FirmCode { get; set; }
        public string FirmName { get; set; }
        public int ItemOrderId { get; set; }
        public string ItemOrderNo { get; set; }
        public string CreatedUserName { get; set; }
        public string FirmResponsible { get; set; }
        #endregion
    }
}
