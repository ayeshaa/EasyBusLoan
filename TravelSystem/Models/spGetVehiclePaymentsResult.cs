using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelSystem.Models
{
    public partial class spGetVehiclePaymentsResult
    {
        public spGetVehiclePaymentsResult()
        {
            
        }
        public long Id { get; set; }
        public int PaymentId { get; set; }
        public string VehicleTypeName { get; set; }
        public int? UserId { get; set; }
        public string FullName { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string StripeCustomerId { get; set; }
        public decimal SalesPrice { get; set; }
        public string Vinnumber { get; set; }
    }
}
