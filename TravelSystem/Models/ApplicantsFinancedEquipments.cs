using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelSystem.Models
{
    public partial class ApplicantsFinancedEquipments
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Required")]
        public int? Quantity { get; set; }
        [Required(ErrorMessage = "Required")]
        public string YearManufactureModel { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Srequested { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Terms { get; set; }
        public bool IsReplacement { get; set; }
        public bool IsLease { get; set; }
        public bool IsLoan { get; set; }
        public bool IsExpansion { get; set; }
        [ForeignKey(nameof(Applicant))]
        public int ApplicantId { get; set; }

        public ApplicantDetails Applicant { get; set; }
    }
}
