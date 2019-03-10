using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelSystem.Models
{

    [Table("Conversations")]
    public partial class Conversations
    {
        public Conversations()
        {

        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Mid { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public string Text { get; set; }
        public bool IsSeen { get; set; }
        public System.DateTime Date { get; set; }
        public string Image { get; set; }

        //public virtual AspNetUser AspNetUser { get; set; }
        //public virtual AspNetUser AspNetUser1 { get; set; }
    }
}
