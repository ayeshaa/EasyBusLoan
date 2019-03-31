using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TravelSystem.Models
{
    [Table("BlogComments")]
    public partial class BlogComments
    {
        public BlogComments()
        {

        }
        public int Id { get; set; }
        [ForeignKey(nameof(Blogs))]
        public int BlogId { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public string Comment { get; set; }
        public ApplicationUser User { get; set; }
        public Blogs Blogs { get; set; }
    }
}
