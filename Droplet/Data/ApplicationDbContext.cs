using Droplet.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Emit;

namespace Droplet.Data
{
        public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Transfusion> Transfusions { get; set; }
        public DbSet<Bank> Donations { get; set; }
        public DbSet<Donor> Donors { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Recipient> Recipients { get; set; } 
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Donor
            modelBuilder.Entity<Donor>()
                .HasKey(d => d.Id);
            modelBuilder.Entity<Donor>()
                .HasMany(d => d.Donations)
                .WithOne(b => b.Donor)
                .HasForeignKey(b => b.IdDonor)
                .OnDelete(DeleteBehavior.Restrict);

            // Recipient
            modelBuilder.Entity<Recipient>()
                .HasKey(r => r.Id);
            modelBuilder.Entity<Recipient>()
                .HasMany(t => t.Transfusions)
                .WithOne(r => r.Recipient)
                .HasForeignKey(t => t.IdRecipient)
                .OnDelete(DeleteBehavior.Restrict);

            // Hospital
            modelBuilder.Entity<Hospital>()
                .HasKey(h => h.Id);
            modelBuilder.Entity<Hospital>()
                .HasMany(h => h.Transfusions)
                .WithOne(t => t.Hospital)
                .HasForeignKey(t => t.IdHospital)
                .OnDelete(DeleteBehavior.Restrict);

            // Doctor
            modelBuilder.Entity<Doctor>()
                .HasKey(d => d.Id);
            modelBuilder.Entity<Doctor>()
                .HasMany(d => d.Hospitals)
                .WithMany(h => h.Doctors)
                .UsingEntity(j => j.ToTable("DoctorHospital"));

            // Blood bank
            modelBuilder.Entity<Bank>()
                .HasKey(b => b.Id);
            modelBuilder.Entity<Bank>()
                .HasOne(b => b.Donor)
                .WithMany(d => d.Donations)
                .HasForeignKey(b => b.IdDonor)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Bank>()
                .HasOne(b => b.Transfusion)
                .WithMany(t => t.BloodUsed)
                .HasForeignKey(b => b.IdTransfusion)
                .OnDelete(DeleteBehavior.Restrict);

            // Transfusion
            modelBuilder.Entity<Transfusion>()
                .HasKey(t => t.Id);
            modelBuilder.Entity<Transfusion>()
                .HasOne(t => t.Recipient)
                .WithMany(r => r.Transfusions)
                .HasForeignKey(t => t.IdRecipient)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Transfusion>()
                .HasOne(t => t.Hospital)
                .WithMany(h => h.Transfusions)
                .HasForeignKey(t => t.IdHospital)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Transfusion>()
                .HasOne(t => t.Doctor)
                .WithMany(d => d.Transfusions)
                .HasForeignKey(t => t.IdDoctor)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Transfusion>()
                .HasMany(t => t.BloodUsed)
                .WithOne(b => b.Transfusion)
                .HasForeignKey(b => b.IdTransfusion)
                .OnDelete(DeleteBehavior.Restrict);



            // Seeding data
            modelBuilder.Entity<Hospital>().HasData(
                new Hospital
                {
                    Id = 1,
                    Name = "Central Clinical Hospital of the Ministry of the Interior and Administration",
                    Address = "137",
                    Street = "Wołoska",
                    PostalCode = "02-507"
                },
                new Hospital
                {
                    Id = 2,
                    Name = "John Paul II Hospital",
                    Address = "80",
                    Street = "Prądnicka",
                    PostalCode = "31-202"
                },
                new Hospital
                {
                    Id = 3,
                    Name = "University Clinical Center",
                    Address = "7",
                    Street = "Dębinki",
                    PostalCode = "80-211"
                },
                new Hospital
                {
                    Id = 4,
                    Name = "Children's Memorial Health Institute",
                    Address = "20",
                    Street = "Aleja Dzieci Polskich",
                    PostalCode = "04-730"
                },
                new Hospital
                {
                    Id = 5,
                    Name = "Wielkopolska Center of Pulmonology and Thoracic Surgery",
                    Address = "62",
                    Street = "Szamarzewskiego",
                    PostalCode = "60-569"
                },
                new Hospital
                {
                    Id = 6,
                    Name = "Independent Public Central Clinical Hospital",
                    Address = "1a",
                    Street = "Banacha",
                    PostalCode = "02-097"
                }
            );

            modelBuilder.Entity<Doctor>().HasData(
                new Doctor { Id = 1, FirstName = "John", LastName = "Doe", PESEL = "90060184361" },
                new Doctor { Id = 2, FirstName = "Jane", LastName = "Doe", PESEL = "94110926169" },
                new Doctor { Id = 3, FirstName = "Alice", LastName = "Smith", PESEL = "99061937114" },
                new Doctor { Id = 4, FirstName = "Bob", LastName = "Johnson", PESEL = "56051657197" },
                new Doctor { Id = 5, FirstName = "Charlie", LastName = "Brown", PESEL = "84101422862" },
                new Doctor { Id = 6, FirstName = "Dave", LastName = "Wilson", PESEL = "96110262292" },
                new Doctor { Id = 7, FirstName = "Eve", LastName = "Davis", PESEL = "62030165469" },
                new Doctor { Id = 8, FirstName = "Frank", LastName = "Miller", PESEL = "72061521868" }
                );

            modelBuilder.Entity<Donor>().HasData(
                new Donor { Id = 1, FirstName = "Frank", LastName = "Miller", PESEL = "78011762657", BloodType = Enum.BloodTypeEnum.A_Negative },
                new Donor { Id = 2, FirstName = "Grace", LastName = "Adams", PESEL = "58031151389", BloodType = Enum.BloodTypeEnum.B_Negative },
                new Donor { Id = 3, FirstName = "Hank", LastName = "Baker", PESEL = "63082969285", BloodType = Enum.BloodTypeEnum.AB_Negative },
                new Donor { Id = 4, FirstName = "Ivy", LastName = "Clark", PESEL = "71021617898", BloodType = Enum.BloodTypeEnum.A_Positive },
                new Donor { Id = 5, FirstName = "Jack", LastName = "Evans", PESEL = "61020226252", BloodType = Enum.BloodTypeEnum.O_Negative },
                new Donor { Id = 6, FirstName = "Kate", LastName = "Fisher", PESEL = "60042994741", BloodType = Enum.BloodTypeEnum.O_Positive },
                new Donor { Id = 7, FirstName = "Leo", LastName = "Garcia", PESEL = "64082237664", BloodType = Enum.BloodTypeEnum.AB_Positive },
                new Donor { Id = 8, FirstName = "Mia", LastName = "Harris", PESEL = "70070919982", BloodType = Enum.BloodTypeEnum.B_Positive }, 
                new Donor { Id = 9, FirstName = "Nina", LastName = "Ivanov", PESEL = "83100453712", BloodType = Enum.BloodTypeEnum.A_Positive },
                new Donor { Id = 10, FirstName = "Oscar", LastName = "Jones", PESEL = "70011425996", BloodType = Enum.BloodTypeEnum.O_Positive },
                new Donor { Id = 11, FirstName = "Paul", LastName = "Kim", PESEL = "90071459573", BloodType = Enum.BloodTypeEnum.B_Negative },
                new Donor { Id = 12, FirstName = "Quinn", LastName = "Lopez", PESEL = "80082622961", BloodType = Enum.BloodTypeEnum.AB_Negative },
                new Donor { Id = 13, FirstName = "Rose", LastName = "Martinez", PESEL = "96110619575", BloodType = Enum.BloodTypeEnum.O_Negative }
                );

            modelBuilder.Entity<Recipient>().HasData(
                new Recipient { Id = 1, FirstName = "Sam", LastName = "Nelson", PESEL = "73040165268", BloodType = Enum.BloodTypeEnum.AB_Negative },
                new Recipient { Id = 2, FirstName = "Tina", LastName = "O'Neill", PESEL = "74071689484", BloodType = Enum.BloodTypeEnum.A_Positive },
                new Recipient { Id = 3, FirstName = "Uma", LastName = "Perez", PESEL = "90102796295", BloodType = Enum.BloodTypeEnum.O_Negative },
                new Recipient { Id = 4, FirstName = "Victor", LastName = "Quinn", PESEL = "65110111691", BloodType = Enum.BloodTypeEnum.O_Positive },
                new Recipient { Id = 5, FirstName = "Wendy", LastName = "Reed", PESEL = "71102928565", BloodType = Enum.BloodTypeEnum.AB_Positive },
                new Recipient { Id = 6, FirstName = "Xander", LastName = "Sanchez", PESEL = "93100227217", BloodType = Enum.BloodTypeEnum.B_Positive },
                new Recipient { Id = 7, FirstName = "Yara", LastName = "Thomas", PESEL = "97120413337", BloodType = Enum.BloodTypeEnum.A_Positive }
                );

        }
    }
}
