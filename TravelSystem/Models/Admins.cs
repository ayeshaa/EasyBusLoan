using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelSystem.Models
{
    [Table("Admins")]
    public partial class Admins
    {
        public Admins()
        {
            Blogs = new HashSet<Blogs>(); 
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public virtual ICollection<Blogs> Blogs { get; set; }
    }
}
