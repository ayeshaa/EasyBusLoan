using System;
using System.Collections.Generic;

namespace TravelSystem.Models
{
    public partial class VehicleTypes
    {
        public VehicleTypes()
        {
            Vehicles = new HashSet<Vehicles>();
            ApplicantVehicles = new HashSet<ApplicantVehicles>();
        }
        public int Id { get; set; }
        public string VehicleTypeName { get; set; }
        public string Description { get; set; }

        public ICollection<Vehicles> Vehicles { get; set; }
        public ICollection<ApplicantVehicles> ApplicantVehicles { get; set; }
    }
}
