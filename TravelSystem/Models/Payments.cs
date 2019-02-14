using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelSystem.Models
{
    public partial class Payments
    {
        public Payments()
        {
            PaymentVehicles = new List<PaymentVehicles>();
        }
        public int Id { get; set; }
        [ForeignKey(nameof(Users))]
        public int? UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public string StripeCustomerId { get; set; }
        public string IpAddress { get; set; }
        public virtual ApplicationUser Users { get; set; }
        public IList<PaymentVehicles> PaymentVehicles { get; set; }
        
    }
}
