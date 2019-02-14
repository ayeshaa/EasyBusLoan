using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelSystem.Models
{
    [Table("Blogs")]
    public partial class Blogs
    {
        public Blogs()
        {
            
        }
        public int Id { get; set; }
        [Required(ErrorMessage ="Required")]
        public string Title { get; set; }
        public string Body { get; set; }
        [Required(ErrorMessage = "Required")]
        public string CategoryIds { get; set; }
        public DateTime CreatedDate { get; set; }
        [ForeignKey(nameof(Admins))]
        public int AdminId { get; set; }
        public virtual Admins Admins { get; set; }
    }
}
