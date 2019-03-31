using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TravelSystem.Models
{
    [Table("AboutUs")]
    public partial class AboutUs
    {
        public AboutUs()
        {

        }
        public int Id { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Description { get; set; }
    }
}
