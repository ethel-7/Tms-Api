public interface ICourseService
{
    Task<Course> CreateAsync(string code, string title, int capacity);
    Task<Course?> GetByIdAsync(string code);
    Task<IReadOnlyList<Course>> GetAllAsync();
    Task<Course?> UpdateAsync(string code, string title, int capacity);
    Task<bool> DeleteAsync(string code);
}

public class CourseService : ICourseService
{
    private readonly Dictionary<string, Course> _store = new();
    private readonly ILogger<CourseService> _logger;
    
    public CourseService(ILogger<CourseService> logger)
    {
        _logger = logger;
    }

    public Task<Course> CreateAsync(string code, string title, int capacity)
    {
        if (_store.ContainsKey(code))
        {
            _logger.LogWarning(
                "Duplicate course creation attempt - {Code} already exists",
                code);
            return Task.FromResult(_store[code]);
        }

        var course = new Course
        {
            Code = code,
            Title = title,
            Capacity = capacity,
            EnrolledCount = 0
        };
        _store[code] = course;
        
        _logger.LogInformation("Created course {Code} - {Title}", code, title);
        
        return Task.FromResult(course);
    }

    public Task<Course?> GetByIdAsync(string code)
    {
        _store.TryGetValue(code, out var course);
        if (course is null)
        {
            _logger.LogWarning("Course {Code} not found", code);
        }
        return Task.FromResult(course);
    }

    public Task<IReadOnlyList<Course>> GetAllAsync()
    {
        IReadOnlyList<Course> all = _store.Values.ToList();
        return Task.FromResult(all);
    }

    public Task<Course?> UpdateAsync(string code, string title, int capacity)
    {
        if (!_store.TryGetValue(code, out var existing))
        {
            _logger.LogWarning("Update failed - course {Code} not found", code);
            return Task.FromResult<Course?>(null);
        }

        existing.Title = title;
        existing.Capacity = capacity;
        
        _logger.LogInformation("Updated course {Code} - {Title}", code, title);
        
        return Task.FromResult<Course?>(existing);
    }

    public Task<bool> DeleteAsync(string code)
    {
        var removed = _store.Remove(code);
        if (removed)
        {
            _logger.LogInformation("Deleted course {Code}", code);
        }
        else
        {
            _logger.LogWarning("Delete failed - course {Code} not found", code);
        }
        return Task.FromResult(removed);
    }
}
