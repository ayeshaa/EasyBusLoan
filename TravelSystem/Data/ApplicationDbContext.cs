using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace TravelSystem.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Admins> Admins { get; set; }
        public virtual DbSet<BlogCategories> BlogCategories { get; set; }
        public virtual DbSet<Blogs> Blogs { get; set; }
        public virtual DbSet<BlogComments> BlogComments { get; set; }
        public virtual DbSet<ApplicantDetails> ApplicantDetails { get; set; }
        public virtual DbSet<ApplicantsCreditReferences> ApplicantsCreditReferences { get; set; }
        public virtual DbSet<ApplicantsFinancedEquipments> ApplicantsFinancedEquipments { get; set; }
        public virtual DbSet<ApplicantsGuarantors> ApplicantsGuarantors { get; set; }
        public virtual DbSet<ApplicantsInsurances> ApplicantsInsurances { get; set; }
        public virtual DbSet<VehicleImages> VehicleImages { get; set; }
        public virtual DbSet<Vehicles> Vehicles { get; set; }
        public virtual DbSet<VehicleTypes> VehicleTypes { get; set; }
        public virtual DbSet<VehiclesCarts> VehiclesCarts { get; set; }
        public virtual DbSet<VehicleRatings> VehicleRatings { get; set; }
        public virtual DbSet<Payments> Payments { get; set; }
        public virtual DbSet<HowItWorks> HowItWorks { get; set; }
        public virtual DbSet<PrivacyPolicy> PrivacyPolicy { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<AboutUs> About { get; set; }
        public virtual DbSet<TermsAndConditions> TermsAndConditions { get; set; }
        public virtual DbSet<TermsOfUseSite> TermsOfUseSite { get; set; }
        public virtual DbSet<spGetVehicleDetail> spGetVehicleDetail { get; set; }
        public virtual DbSet<PaymentVehicles> PaymentVehicles { get; set; }
        public virtual DbSet<ApplicantVehicles> ApplicantVehicles { get; set; }
        public virtual DbSet<Conversations> Conversation { get; set; }
        public virtual DbSet<spGetVehiclePaymentsResult> spGetVehiclePaymentsResult { get; set; }
        public virtual DbSet<spGetAllVehiclesResult> spGetAllVehiclesResult { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicantDetails>(entity =>
            {
                entity.Property(e => e.Address)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CompanyInformation).IsUnicode(false);

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ContactPerson)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FaxNumber)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FederalTaxIdNumber)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.IsCcorp).HasColumnName("IsCCorp");

                entity.Property(e => e.IsLlc).HasColumnName("IsLLC");

                entity.Property(e => e.IsScorp).HasColumnName("IsSCorp");

                entity.Property(e => e.Message).HasMaxLength(250);

                entity.Property(e => e.ReasonForAcquisition).IsUnicode(false);

                entity.Property(e => e.State)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Telephone)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TradeStyle)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.YearEstablished).HasColumnType("date");

                entity.Property(e => e.ZipCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ApplicantDetails)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicantDetails_Users");

                entity.HasOne(d => d.Vehicle)
                    .WithMany(p => p.ApplicantDetails)
                    .HasForeignKey(d => d.VehicleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicantDetails_Vehicles");
            });

            modelBuilder.Entity<ApplicantsCreditReferences>(entity =>
            {
                entity.Property(e => e.AccountNumber)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.BankName)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Contact)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Telephone)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Applicant)
                    .WithMany(p => p.ApplicantsCreditReferences)
                    .HasForeignKey(d => d.ApplicantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicantsCreditReferences_ApplicantDetails");
            });

            modelBuilder.Entity<ApplicantsFinancedEquipments>(entity =>
            {
                entity.Property(e => e.Srequested)
                    .HasColumnName("SRequested")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Terms)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.YearManufactureModel)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.Applicant)
                    .WithMany(p => p.ApplicantsFinancedEquipments)
                    .HasForeignKey(d => d.ApplicantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicantsFinancedEquipments_ApplicantDetails");
            });

            modelBuilder.Entity<ApplicantsGuarantors>(entity =>
            {
                entity.Property(e => e.Address)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.OwnerShip)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PrincipleOwner)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SocialSecurityNumber)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.State)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ZipCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Applicant)
                    .WithMany(p => p.ApplicantsGuarantors)
                    .HasForeignKey(d => d.ApplicantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicantsGuarantors_ApplicantDetails");
            });

            modelBuilder.Entity<ApplicantsInsurances>(entity =>
            {
                entity.Property(e => e.Agent)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.InsuranceCompanyName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Telephone)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Applicant)
                    .WithMany(p => p.ApplicantsInsurances)
                    .HasForeignKey(d => d.ApplicantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicantsInsurances_ApplicantDetails");
            });

            //modelBuilder.Entity<Users>(entity =>
            //{
            //    entity.Property(e => e.CompanyName)
            //        .HasMaxLength(150)
            //        .IsUnicode(false);

            //    entity.Property(e => e.Email)
            //        .IsRequired()
            //        .HasMaxLength(100)
            //        .IsUnicode(false);

            //    entity.Property(e => e.FullName)
            //        .IsRequired()
            //        .HasMaxLength(100)
            //        .IsUnicode(false);

            //    entity.Property(e => e.Password)
            //        .IsRequired()
            //        .HasMaxLength(100)
            //        .IsUnicode(false);

            //    entity.Property(e => e.Phone)
            //        .HasMaxLength(50)
            //        .IsUnicode(false);
            //});

            modelBuilder.Entity<VehicleImages>(entity =>
            {
                entity.Property(e => e.FileNameOnDisk).IsRequired();

                entity.HasOne(d => d.Vehicle)
                    .WithMany(p => p.VehicleImages)
                    .HasForeignKey(d => d.VehicleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VehicleImages_Vehicles");
            });

            modelBuilder.Entity<Vehicles>(entity =>
            {
                entity.Property(e => e.CityOrState)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.MakeYear).HasColumnType("date");

                entity.Property(e => e.Mileage)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Model)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Vinnumber)
                    .IsRequired()
                    .HasColumnName("VINNumber")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Vehicles)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Vehicles_Users");
            });

            modelBuilder.Entity<VehicleTypes>(entity =>
            {
                entity.Property(e => e.Description)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.VehicleTypeName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });
        }
    }
}
