using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelSystem.Models
{
    [Table("BlogCategories")]
    public partial class BlogCategories
    {
        public BlogCategories()
        {
            
        }
        public int Id { get; set; }
        [Required(ErrorMessage = "Required")]
        public string CategoryName { get; set; }

    }
}
