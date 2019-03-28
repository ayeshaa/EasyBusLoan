using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace TravelSystem.Models
{
    public class ApplicationUser:IdentityUser<int>
    {
        public ApplicationUser()
        {
            ApplicantApproveDetails = new List<ApplicantDetails>();
            ApplicantDetails = new List<ApplicantDetails>();
            VehiclesCarts = new List<VehiclesCarts>();
            VehicleRatings = new List<VehicleRatings>();
            Payments = new List<Payments>();
        }
        public string FullName { get; set; }
        public bool IsSeller { get; set; }
        public bool IsBuyer { get; set; }
        public bool IsLendor { get; set; }
        public bool IsBlock { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string Photo { get; set; }
        public bool IsLogin { get; set; }
        public IList<ApplicantDetails> ApplicantDetails { get; set; }
        public IList<ApplicantDetails> ApplicantApproveDetails { get; set; }
        public IList<Vehicles> Vehicles { get; set; }
        public IList<VehiclesCarts> VehiclesCarts { get; set; }
        public IList<VehicleRatings> VehicleRatings { get; set; }
        public IList<Payments> Payments { get; set; }
    }
}
