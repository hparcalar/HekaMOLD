using System;

namespace Heka.DataAccess.Context.Models
{
    public partial class CustomsDoor
    {
        public CustomsDoor()
        {

        }
        public int Id { get; set; }
        public string CustomsDoorCode { get; set; }
        public string CustomsDoorName { get; set; }

        public Nullable<int> PlantId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
    }
}
