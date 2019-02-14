using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelSystem.Models
{
    public partial class ApplicantDetails
    {
        public ApplicantDetails()
        {
            ApplicantsCreditReferences = new List<ApplicantsCreditReferences>();
            ApplicantsFinancedEquipments = new List<ApplicantsFinancedEquipments>();
            ApplicantsGuarantors = new List<ApplicantsGuarantors>();
            ApplicantsInsurances = new List<ApplicantsInsurances>();
            ApplicantVehicles = new List<ApplicantVehicles>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Required")]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "Required")]
        public string TradeStyle { get; set; }
        [Required(ErrorMessage = "Required")]
        public DateTime? YearEstablished { get; set; }
        [Required(ErrorMessage = "Required")]
        public string FederalTaxIdNumber { get; set; }
        [Required(ErrorMessage = "Required")]
        public string CurrentFederalTaxIdNumber { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Required")]
        public string City { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Required")]
        public string State { get; set; }
        [Required(ErrorMessage = "Required")]
        public string ZipCode { get; set; }
        [Required(ErrorMessage = "Required")]
        public string ContactPerson { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Telephone { get; set; }
        [Required(ErrorMessage = "Required")]
        public string FaxNumber { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public bool IsCorportation { get; set; }
        public bool IsPartnership { get; set; }
        public bool IsProprietorship { get; set; }
        public bool IsLlc { get; set; }
        public bool IsCcorp { get; set; }
        public bool IsScorp { get; set; }
        public string Message { get; set; }
        [Required(ErrorMessage = "Required")]
        public int? CurrentCoaches { get; set; }
        [Required(ErrorMessage = "Required")]
        public int? CurrentVans { get; set; }
        [Required(ErrorMessage = "Required")]
        public int? CurrentSchoolBuses { get; set; }
        [Required(ErrorMessage = "Required")]
        public int? CurrentLimes { get; set; }
        [Required(ErrorMessage = "Required")]
        public int? CurrentOthers { get; set; }
        [Required(ErrorMessage = "Required")]
        public string CompanyInformation { get; set; }
        [Required(ErrorMessage = "Required")]
        public string ReasonForAcquisition { get; set; }
        [ForeignKey(nameof(Vehicle))]
        public int? VehicleId { get; set; }
        public bool ToBeDelievered { get; set; }
        [ForeignKey(nameof(ApproveUser))]
        public int? ApprovedByUserId { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public ApplicationUser User { get; set; }
        public ApplicationUser ApproveUser { get; set; }
        public Vehicles Vehicle { get; set; }
        public IList<ApplicantsCreditReferences> ApplicantsCreditReferences { get; set; }
        public IList<ApplicantsFinancedEquipments> ApplicantsFinancedEquipments { get; set; }
        public IList<ApplicantsGuarantors> ApplicantsGuarantors { get; set; }
        public IList<ApplicantsInsurances> ApplicantsInsurances { get; set; }
        public IList<ApplicantVehicles> ApplicantVehicles { get; set; }
    }
}
