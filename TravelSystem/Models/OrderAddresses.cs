using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TravelSystem.Models
{
    [Table("OrderAddresses")]
    public partial class OrderAddresses
    {
        public OrderAddresses()
        {

        }
        public int Id { get; set; }
        public int PaymentId { get; set; }
        [Required(ErrorMessage = "Required")]
        public string ShippingName { get; set; }
        [Required(ErrorMessage = "Required")]
        public string ShippingCompany { get; set; }
        [Required(ErrorMessage = "Required")]
        public string ShippingAddress1 { get; set; }
        [Required(ErrorMessage = "Required")]
        public string ShippingAddress2 { get; set; }
        [Required(ErrorMessage = "Required")]
        public string ShippingCity { get; set; }
        [Required(ErrorMessage = "Required")]
        public string ShippingCountry { get; set; }
        [Required(ErrorMessage = "Required")]
        public string ShippingZipCode { get; set; }
        [Required(ErrorMessage = "Required")]
        public string BillingName { get; set; }
        [Required(ErrorMessage = "Required")]
        public string BillingCompany { get; set; }
        [Required(ErrorMessage = "Required")]
        public string BillingAddress1 { get; set; }
        [Required(ErrorMessage = "Required")]
        public string BillingAddress2 { get; set; }
        [Required(ErrorMessage = "Required")]
        public string BillingCity { get; set; }
        [Required(ErrorMessage = "Required")]
        public string BillingCountry { get; set; }
        [Required(ErrorMessage = "Required")]
        public string BillingZipCode { get; set; }
    }
}
