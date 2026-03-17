using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Data.Context
{
    public class SmartPcosDbContext : IdentityDbContext<ApplicationUser>
    {
        public SmartPcosDbContext(DbContextOptions<SmartPcosDbContext> options)
            : base(options)
        {
        }

        // Business Tables
        public DbSet<ClinicalData> ClinicalData { get; set; }
        public DbSet<HormoneLabResult> HormoneLabResults { get; set; }
        public DbSet<UltrasoundImage> UltrasoundImages { get; set; }
        public DbSet<ContactUsMessage> ContactUsMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. ربط الـ Entities بالأسماء اللي ظاهرة في الـ SQL عندك بالظبط
            modelBuilder.Entity<ClinicalData>().ToTable("clinical_data");
            modelBuilder.Entity<UltrasoundImage>().ToTable("ultrasound_images");
            modelBuilder.Entity<HormoneLabResult>().ToTable("hormone_lab_results");
            modelBuilder.Entity<ContactUsMessage>().ToTable("contact_us_messages");

            // 2. الـ Identity Tables اللي إنت مغيرها أصلاً
            modelBuilder.Entity<ApplicationUser>().ToTable("users");
            modelBuilder.Entity<IdentityRole>().ToTable("roles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("user_roles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("user_claims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("user_logins");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("user_tokens");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("role_claims");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SmartPcosDbContext).Assembly);
        }
    }
}