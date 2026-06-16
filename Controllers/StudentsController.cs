using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/students")]
public class StudentsController(IStudentService studentService) : ControllerBase
{
    // GET /api/students returns all student records
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var students = await studentService.GetAllAsync();
        return Ok(students);
    }

    // GET /api/students/{id} returns one or 404
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var student = await studentService.GetByIdAsync(id);
        return student is not null ? Ok(student) : NotFound();
    }

    // POST /api/students creates and returns 201 with Location header
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStudentRequest request)
    {
        var student = await studentService.CreateAsync(request.Name, request.Age, request.GPA);
        return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
    }

    // PUT /api/students/{id} updates and returns 200 or 404
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateStudentRequest request)
    {
        var student = await studentService.UpdateAsync(id, request.Name, request.Age, request.GPA);
        return student is not null ? Ok(student) : NotFound();
    }

    // DELETE /api/students/{id} returns 204 or 404
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await studentService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}

public record CreateStudentRequest(string Name, int Age, decimal GPA);
public record UpdateStudentRequest(string Name, int Age, decimal GPA);
