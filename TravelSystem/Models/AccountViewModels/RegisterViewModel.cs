using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TravelSystem.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        public RegisterViewModel()
        {
            ApplicantApproveDetails = new List<ApplicantDetails>();
            ApplicantDetails = new List<ApplicantDetails>();
            ApplicationUser = new List<ApplicationUser>();
            Vehicles = new List<Vehicles>();
            VehiclesCarts = new List<VehiclesCarts>();
            VehicleRatings = new List<VehicleRatings>();
            Payments = new List<Payments>();
        }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "FullName")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Required")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Required")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$",
            ErrorMessage = "Passwords must be at least eight characters, at least one uppercase letter, one lowercase letter, one number and one special character (e.g. !@#$%^&*)")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public bool IsSeller { get; set; }
        public bool IsBuyer { get; set; }
        public bool IsLendor { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string Photo { get; set; }
        public IList<ApplicationUser> ApplicationUser { get; set; }
        public IList<ApplicantDetails> ApplicantDetails { get; set; }
        public IList<ApplicantDetails> ApplicantApproveDetails { get; set; }
        public IList<Vehicles> Vehicles { get; set; }
        public IList<VehiclesCarts> VehiclesCarts { get; set; }
        public IList<VehicleRatings> VehicleRatings { get; set; }
        public IList<Payments> Payments { get; set; }
    }
}
