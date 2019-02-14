using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelSystem.Models
{
    public partial class VehicleImages
    {
        public int Id { get; set; }
        public string FileNameOnDisk { get; set; }
        [ForeignKey(nameof(Vehicle))]
        public int VehicleId { get; set; }

        public Vehicles Vehicle { get; set; }
    }
}
