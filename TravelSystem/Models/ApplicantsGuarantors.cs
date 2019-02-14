using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelSystem.Models
{
    public partial class ApplicantsGuarantors
    {
        public int Id { get; set; }
        [Required(ErrorMessage="Required")]
        public string PrincipleOwner { get; set; }
        [Required(ErrorMessage = "Required")]
        public string OwnerShip { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Required")]
        public string SocialSecurityNumber { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Required")]
        public string State { get; set; }
        [Required(ErrorMessage = "Required")]
        public string ZipCode { get; set; }
        [ForeignKey(nameof(Applicant))]
        public int ApplicantId { get; set; }
        [Required(ErrorMessage = "Required")]
        public string City { get; set; }
        public ApplicantDetails Applicant { get; set; }
    }
}
