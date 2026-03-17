using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configuration
{
    public class ClinicalDataConfiguration : IEntityTypeConfiguration<ClinicalData>
    {
        public void Configure(EntityTypeBuilder<ClinicalData> builder)
        {
            builder.ToTable("clinical_data");
            builder.HasKey(c => c.Id);

            // Configure text columns for large data
            builder.Property(c => c.PhysicalSymptoms).HasColumnType("text");
            builder.Property(c => c.RecentMedicalReports).HasColumnType("text");

            // Avoid SQL 'timestamp' type to prevent explicit insert errors
            builder.Property(c => c.CreatedAt).HasColumnType("nvarchar(max)");
        }
    }
}