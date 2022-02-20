using System;

namespace Heka.DataAccess.Context
{
   public partial class VehicleNotification
    {
        public VehicleNotification()
        {


        }
        public int Id { get; set; }
        // Bakım Adım
        public int? CareStep { get; set; }
        public bool? MailNotification { get; set; }
        public bool? SmsNotification { get; set; }
        public bool? ExtraNotification { get; set; }

        public Nullable<int> PlantId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
    }
}
