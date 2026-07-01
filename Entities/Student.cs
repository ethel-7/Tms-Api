namespace TmsApi.Entities;

public class Student
{
    public int Id { get; set; } // surrogate primary key — internal, used by foreign keys
    public required string RegistrationNumber { get; set; } // natural key — human-readable (uniqueness configured in Session 2)
    public required string Name { get; set; }
    public decimal GPA { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Exercise 8: Concurrency token
    public uint Version { get; set; }
    
    // Exercise 9: Soft delete support
    public bool IsDeleted { get; set; } = false;

    // Navigation property for many-to-many relationship
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    
    // Navigation property for one-to-many relationship with Certificates
    public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
}
