using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelSystem.Models
{
    public partial class Users
    {
        public Users()
        {
            ApplicantApproveDetails = new List<ApplicantDetails>();
            ApplicantDetails = new List<ApplicantDetails>();
            Vehicles = new List<Vehicles>();
            VehiclesCarts = new List<VehiclesCarts>();
            VehicleRatings = new List<VehicleRatings>();
            Payments = new List<Payments>();
        }

        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        [NotMapped]
        public string ConfirmPassword { get; set; }
        public bool IsSeller { get; set; }
        public bool IsBuyer { get; set; }
        public bool IsLendor { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }

        public IList<ApplicantDetails> ApplicantDetails { get; set; }
        public IList<ApplicantDetails> ApplicantApproveDetails { get; set; }
        public IList<Vehicles> Vehicles { get; set; }
        public IList<VehiclesCarts> VehiclesCarts { get; set; }
        public IList<VehicleRatings> VehicleRatings { get; set; }
        public IList<Payments> Payments { get; set; }
    }
}
