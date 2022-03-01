using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heka.DataAccess.Context
{
   public class FirmAddress
    {
        public FirmAddress()
        {

        }
        public int Id { get; set; }
        public string AddressName { get; set; }

        [ForeignKey("Firm")]
        public int FirmId { get; set; }

        [ForeignKey("City")]
        public int? CityId { get; set; }

        [ForeignKey("Country")]
        public int? CountryId { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string MobilePhone { get; set; }
        public string Fax { get; set; }
        public string OfficePhone { get; set; }
        public string AuthorizedInfo { get; set; }
        public int? AddressType { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        public virtual Firm Firm { get; set; }
        public virtual City City { get; set; }
        public virtual Country  Country { get; set; }

    }
}
