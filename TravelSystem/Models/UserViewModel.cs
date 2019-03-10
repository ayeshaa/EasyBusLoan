using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace TravelSystem.Models
{
    public class UserViewModel
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public bool IsSeller { get; set; }
        public bool IsBuyer { get; set; }
        public bool IsLendor { get; set; }
        public bool IsBlock { get; set; }
    }
}
