using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelSystem.Models
{
    public partial class ApplicantsInsurances
    {
        public int Id { get; set; }
        [Required(ErrorMessage="Required")]
        public string InsuranceCompanyName { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Agent { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Telephone { get; set; }
        [ForeignKey(nameof(Applicant))]
        public int ApplicantId { get; set; }

        public ApplicantDetails Applicant { get; set; }
    }
}
