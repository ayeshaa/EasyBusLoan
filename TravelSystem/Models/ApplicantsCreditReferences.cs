using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelSystem.Models
{
    public partial class ApplicantsCreditReferences
    {
        public int Id { get; set; }
        [ForeignKey(nameof(Applicant))]
        public int ApplicantId { get; set; }
        [Required(ErrorMessage = "Required")]
        public string BankName { get; set; }
        [Required(ErrorMessage = "Required")]
        public string AccountNumber { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Contact { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Telephone { get; set; }

        public ApplicantDetails Applicant { get; set; }
    }
}
