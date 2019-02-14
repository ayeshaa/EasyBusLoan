using System;
using System.Collections.Generic;

namespace TravelSystem.Models
{
    public partial class spGetVehicleDetail
    {
        
        public int Id { get; set; }
        public int TotalStars { get; set; }
        public int TotalRatings { get; set; }
        public decimal SalesPrice { get; set; }
        public string VehicleTypeName { get; set; }
        public string ImageNameOnDisk { get; set; }
    }
}
