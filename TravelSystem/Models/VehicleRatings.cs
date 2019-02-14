using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelSystem.Models
{
    public partial class VehicleRatings
    {
        public VehicleRatings()
        {
           
        }

        public int Id { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        [ForeignKey(nameof(Vehicles))]
        public int VehicleId { get; set; }
        public int Stars { get; set; }
        public string Comment { get; set; }
        public ApplicationUser User { get; set; }
        public Vehicles Vehicles { get; set; }
    }
}
