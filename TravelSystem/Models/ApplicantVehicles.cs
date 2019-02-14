using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelSystem.Models
{
    public partial class ApplicantVehicles
    {
        public ApplicantVehicles()
        {
           
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
        [Required(ErrorMessage = "Required")]
        [ForeignKey(nameof(ApplicantDetails))]
        public int ApplicantId { get; set; }
        [ForeignKey(nameof(VehicleTypes))]
        public int VehicleTypeId { get; set; }
        public ApplicantDetails ApplicantDetails { get; set; }
        public VehicleTypes VehicleTypes { get; set; }
    }
}
