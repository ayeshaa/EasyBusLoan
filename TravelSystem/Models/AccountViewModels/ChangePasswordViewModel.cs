
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TravelSystem.Models.AccountViewModels
{
    public class ChangePasswordViewModel
    {
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$",
          ErrorMessage = "Passwords must be at least eight characters, at least one uppercase letter, one lowercase letter, one number and one special character (e.g. !@#$%^&*)")]
        [Display(Name = "New Password")]
        [Required(ErrorMessage = "Required")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        [Required(ErrorMessage = "Required")]
        public string NewConfirmPassword { get; set; }
        public string Email { get; set; }
    }
}
