using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelSystem.Models
{
    public class ConversationVM
    {
        public int From { get; set; }
        public int To { get; set; }
        public string Text { get; set; }
        public bool IsSeen { get; set; }
        public DateTime Date { get; set; }
        public string Image { get; set; }

        public string FromImage { get; set; }

    }

    public partial class LatestConversationVM
    {
        public string FromName { get; set; }
        public int From { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public DateTime Date { get; set; }
    }
}
