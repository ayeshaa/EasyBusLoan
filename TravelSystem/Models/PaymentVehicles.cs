using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelSystem.Models
{
    public partial class PaymentVehicles
    {
        public PaymentVehicles()
        {

        }
        public int Id { get; set; }
        [ForeignKey(nameof(Vehicles))]
        public int VehicleId { get; set; }
        [ForeignKey(nameof(Payments))]
        public int PaymentId { get; set; }
        public virtual Vehicles Vehicles { get; set; }
        public virtual Payments Payments { get; set; }

    }
}
