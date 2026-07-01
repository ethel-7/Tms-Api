using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TmsApi.Entities;

namespace TmsApi.Data.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.RegistrationNumber)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(s => s.GPA)
            .IsRequired()
            .HasPrecision(3, 2);  // e.g., 3.85
        
        builder.Property(s => s.IsActive)
            .IsRequired();
        
        // Exercise 8: Shadow property for audit (LastUpdated)
        builder.Property<DateTime>("LastUpdated")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        // Exercise 8: Concurrency token (row version)
        builder.Property(s => s.Version)
            .IsRowVersion();
        
        // Exercise 9: Soft delete query filter
        builder.HasQueryFilter(s => !s.IsDeleted);
        
        // One Student has many Enrollments
        builder.HasMany(s => s.Enrollments)
            .WithOne(e => e.Student)
            .HasForeignKey(e => e.StudentId);
    }
}
