using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/courses")]
public class CoursesController(ICourseService courseService) : ControllerBase
{
    // GET /api/courses returns all course records
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var courses = await courseService.GetAllAsync();
        return Ok(courses);
    }

    // GET /api/courses/{code} returns one or 404
    [HttpGet("{code}")]
    public async Task<IActionResult> GetById(string code)
    {
        var course = await courseService.GetByIdAsync(code);
        return course is not null ? Ok(course) : NotFound();
    }

    // POST /api/courses creates and returns 201 with Location header
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCourseRequest request)
    {
        var course = await courseService.CreateAsync(request.Code, request.Title, request.Capacity);
        return CreatedAtAction(nameof(GetById), new { code = course.Code }, course);
    }

    // PUT /api/courses/{code} updates and returns 200 or 404
    [HttpPut("{code}")]
    public async Task<IActionResult> Update(string code, [FromBody] UpdateCourseRequest request)
    {
        var course = await courseService.UpdateAsync(code, request.Title, request.Capacity);
        return course is not null ? Ok(course) : NotFound();
    }

    // DELETE /api/courses/{code} returns 204 or 404
    [HttpDelete("{code}")]
    public async Task<IActionResult> Delete(string code)
    {
        var deleted = await courseService.DeleteAsync(code);
        return deleted ? NoContent() : NotFound();
    }
}

public record CreateCourseRequest(string Code, string Title, int Capacity);
public record UpdateCourseRequest(string Title, int Capacity);
