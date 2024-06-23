using Droplet.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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
        }
    }
}
