public interface IStudentService
{
    Task<Student> CreateAsync(string name, int age, decimal gpa);
    Task<Student?> GetByIdAsync(string id);
    Task<IReadOnlyList<Student>> GetAllAsync();
    Task<Student?> UpdateAsync(string id, string name, int age, decimal gpa);
    Task<bool> DeleteAsync(string id);
}

public class StudentService : IStudentService
{
    private readonly Dictionary<string, Student> _store = new();
    private readonly ILogger<StudentService> _logger;
    
    public StudentService(ILogger<StudentService> logger)
    {
        _logger = logger;
    }

    public Task<Student> CreateAsync(string name, int age, decimal gpa)
    {
        var id = Guid.NewGuid().ToString("N")[..8];
        var student = new Student
        {
            Id = id,
            Name = name,
            Age = age,
            GPA = gpa
        };
        _store[id] = student;
        
        _logger.LogInformation("Created student {Id} - {Name}", id, name);
        
        return Task.FromResult(student);
    }

    public Task<Student?> GetByIdAsync(string id)
    {
        _store.TryGetValue(id, out var student);
        if (student is null)
        {
            _logger.LogWarning("Student {Id} not found", id);
        }
        return Task.FromResult(student);
    }

    public Task<IReadOnlyList<Student>> GetAllAsync()
    {
        IReadOnlyList<Student> all = _store.Values.ToList();
        return Task.FromResult(all);
    }

    public Task<Student?> UpdateAsync(string id, string name, int age, decimal gpa)
    {
        if (!_store.TryGetValue(id, out var existing))
        {
            _logger.LogWarning("Update failed - student {Id} not found", id);
            return Task.FromResult<Student?>(null);
        }

        var updated = new Student 
        { 
            Id = id, 
            Name = name, 
            Age = age, 
            GPA = gpa 
        };
        _store[id] = updated;
        
        _logger.LogInformation("Updated student {Id} - {Name}", id, name);
        
        return Task.FromResult<Student?>(updated);
    }

    public Task<bool> DeleteAsync(string id)
    {
        var removed = _store.Remove(id);
        if (removed)
        {
            _logger.LogInformation("Deleted student {Id}", id);
        }
        else
        {
            _logger.LogWarning("Delete failed - student {Id} not found", id);
        }
        return Task.FromResult(removed);
    }
}
