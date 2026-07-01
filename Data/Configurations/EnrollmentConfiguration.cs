using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TmsApi.Entities;

namespace TmsApi.Data.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.StudentId)
            .IsRequired();
        
        builder.Property(e => e.CourseId)
            .IsRequired();
        
        builder.Property(e => e.Grade)
            .HasPrecision(3, 2);  // e.g., 3.85, optional field
        
        builder.Property(e => e.EnrolledAt)
            .IsRequired();
        
        // Exercise 5: Configure relationships with OnDelete behavior
        // Restrict: A course with enrollments cannot be deleted (must handle in application code)
        // This prevents accidental data loss - students' enrollment history is preserved
        builder.HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);  // Cannot delete student with enrollments
        
        builder.HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Restrict);  // Cannot delete course with enrollments
    }
}
