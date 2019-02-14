using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelSystem.Models
{
    public partial class VehiclesCarts
    {
        public int Id { get; set; }
        public string MachineName { get; set; }
        [ForeignKey(nameof(Users))]
        public int? UserId { get; set; }
        [ForeignKey(nameof(Vehicle))]
        public int VehicleId { get; set; }

        public Vehicles Vehicle { get; set; }
        public ApplicationUser Users { get; set; }
        public spGetVehicleDetail SpGetVehicleDetail { get; set; }
    }
}
