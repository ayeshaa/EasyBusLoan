using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelSystem.Models
{
    public partial class spGetAllVehiclesResult
    {
        public spGetAllVehiclesResult()
        {
            
        }
        public int Id { get; set; }
        public string FileNameOnDisk { get; set; }
        public string Mileage { get; set; }
        public string Model { get; set; }
        public int PassengersSeats { get; set; }
        public decimal SalesPrice { get; set; }
        public string Location { get; set; }
        public string CityOrState { get; set; }
        public string Vinnumber { get; set; }
        public DateTime MakeYear { get; set; }
        public int? UserId { get; set; }
        public int VehicleTypeId { get; set; }
        public string VehicleTypeName { get; set; }
    }
}
