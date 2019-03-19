using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelSystem.Models
{
    public partial class Vehicles
    {
        public Vehicles()
        {
            ApplicantDetails = new HashSet<ApplicantDetails>();
            VehicleImages = new HashSet<VehicleImages>();
            VehicleRatings = new List<VehicleRatings>();
            PaymentVehicles = new List<PaymentVehicles>();
        }

        public int Id { get; set; }
        [Required(ErrorMessage="Required")]
        public string Mileage { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Model { get; set; }
        [Required(ErrorMessage = "Required")]
        public int PassengersSeats { get; set; }
        [Required(ErrorMessage = "Required")]
        public decimal SalesPrice { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Location { get; set; }
        [Required(ErrorMessage = "Required")]
        public string CityOrState { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Vinnumber { get; set; }
        [Required(ErrorMessage = "Required")]
        public DateTime MakeYear { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        [ForeignKey(nameof(VehicleTypes))]
        public int VehicleTypeId { get; set; }
        public bool IsSold { get; set; }
        public ApplicationUser User { get; set; }
        [NotMapped]
        public bool IsChecked { get; set; }
        [NotMapped]
        public int Stars { get; set; }
        public VehicleTypes VehicleTypes { get; set; }
        public ICollection<ApplicantDetails> ApplicantDetails { get; set; }
        public ICollection<VehicleImages> VehicleImages { get; set; }
        public IList<VehicleRatings> VehicleRatings { get; set; }
        public IList<PaymentVehicles> PaymentVehicles { get; set; }
    }
}
