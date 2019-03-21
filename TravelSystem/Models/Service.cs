using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TravelSystem.Models
{
        [Table("Services")]
        public partial class Service
        {
            public Service()
            {

            }
            public int Id { get; set; }
            [Required(ErrorMessage = "Required")]
            public string Description { get; set; }
        }
}
