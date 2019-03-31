using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TravelSystem.Models
{
    public class ProfileViewModel
    {
        public ProfileViewModel()
        {

        }
        [Required(ErrorMessage="Required")]
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Phone { get; set; }
        public string Photo { get; set; }
    }
}
