using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public class ServiceItem
    {
        public ServiceItem()
        {
            this.LoadInvoice = new HashSet<LoadInvoice>();
        }
        public int Id { get; set; }
        public string ServiceItemCode { get; set; }
        public string ServiceItemName { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        [InverseProperty("ServiceItem")]
        public virtual ICollection<LoadInvoice> LoadInvoice { get; set; }
    }
}
