using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TmsApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication("Training")
    .AddScheme<AuthenticationSchemeOptions, TrainingAuthHandler>("Training", null);

builder.Services.AddAuthorization();

// Register TmsDbContext scoped for incoming HTTP requests
builder.Services.AddDbContext<TmsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TmsDatabase"))
        .LogTo(Console.WriteLine, LogLevel.Information) // Log SQL to output window
        .EnableSensitiveDataLogging()); // Show parameters in query logs (dev only)

// Old in-memory services (Module 4) - will be replaced with EF Core repositories
// builder.Services.AddSingleton<IEnrollmentService, EnrollmentService>();
// builder.Services.AddSingleton<EnrollmentWorker>();
// builder.Services.AddSingleton<IStudentService, StudentService>();
// builder.Services.AddSingleton<ICourseService, CourseService>();

builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});

builder.Services.AddOptions<PaymentOptions>()
    .BindConfiguration("Payments")
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Seed test data at startup
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<TmsDbContext>();
        context.Database.Migrate(); // Applies any pending migrations; keeps migration history intact

        if (!context.Students.Any())
        {
            var students = new List<TmsApi.Entities.Student>
            {
                new() { RegistrationNumber = "TMS-2026-0001", Name = "Alice Smith", GPA = 3.8m, IsActive = true },
                new() { RegistrationNumber = "TMS-2026-0002", Name = "Bob Jones", GPA = 2.9m, IsActive = true },
                new() { RegistrationNumber = "TMS-2026-0003", Name = "Charlie Brown", GPA = 3.4m, IsActive = false },
                new() { RegistrationNumber = "TMS-2026-0004", Name = "Diana Prince", GPA = 3.9m, IsActive = true },
                new() { RegistrationNumber = "TMS-2026-0005", Name = "Evan Wright", GPA = 2.5m, IsActive = true }
            };
            context.Students.AddRange(students);

            var courses = new List<TmsApi.Entities.Course>
            {
                new() { Code = "CS-101", Title = "Introduction to Computer Science", Capacity = 30 },
                new() { Code = "CS-201", Title = "Data Structures and Algorithms", Capacity = 25 },
                new() { Code = "MAT-101", Title = "Calculus I", Capacity = 40 }
            };
            context.Courses.AddRange(courses);
            context.SaveChanges();

            var enrollments = new List<TmsApi.Entities.Enrollment>
            {
                new() { StudentId = students[0].Id, CourseId = courses[0].Id, Grade = 4.0m },
                new() { StudentId = students[0].Id, CourseId = courses[1].Id, Grade = 3.6m },
                new() { StudentId = students[1].Id, CourseId = courses[0].Id, Grade = 2.8m },
                new() { StudentId = students[3].Id, CourseId = courses[1].Id, Grade = 3.9m }
            };
            context.Enrollments.AddRange(enrollments);
            context.SaveChanges();
            
            Console.WriteLine("✅ Seed data inserted successfully!");
        }
        else
        {
            Console.WriteLine("ℹ️  Database already contains data. Skipping seed.");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Seeding failed: {ex.Message}");
    Console.WriteLine($"Stack: {ex.StackTrace}");
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/api/error", () =>
{
    throw new TmsDatabaseException("Simulated database failure for ProblemDetails testing");
});

app.MapGet("/api/assessments/results", () => Results.Ok(new
{
    courseCode = "CS-101",
    studentId = "S-001",
    letterGrade = "A"
}))
.RequireAuthorization();

app.Run();
