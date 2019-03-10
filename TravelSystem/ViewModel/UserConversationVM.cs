using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace TravelSystem.ViewModel
{
    public class UserConversationVM
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string ImageUrl { get; set; }
        public int? count { get; set; }
        public bool IsLogin { get; set; }
    }
}