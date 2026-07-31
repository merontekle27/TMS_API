using Microsoft.AspNetCore.Mvc;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController : ControllerBase
{
    // This controller handles HTTP requests related to enrollments. It uses the IEnrollmentService to perform operations on enrollment records.
    private readonly IEnrollmentService _service;

    public EnrollmentsController(IEnrollmentService service)
    {
        _service = service;
        
        
    }
    // This action method handles GET requests to retrieve an enrollment record by its ID. It returns a 404 Not Found response if the record does not exist.
   // The controller:
// 1. Receives the request.
// 2. Calls EnrollAsync().
// 3. Returns the created enrollment.
    [HttpPost]
[HttpPost]
public async Task<IActionResult> Create([FromBody] EnrollmentRequest request)
{
    var enrollment = await _service.EnrollAsync(
        request.StudentId,
        request.CourseCode);

    return CreatedAtAction(
        nameof(GetById),
        new { id = enrollment.Id },
        enrollment);
}
[HttpGet]
public async Task<IActionResult> GetAll()
{
    var enrollments = await _service.GetAllAsync();
    return Ok(enrollments);
}

[HttpGet("{id}")]
public async Task<IActionResult> GetById(string id)
{
    var enrollment = await _service.GetByIdAsync(id);

    if (enrollment == null)
    {
        return NotFound();
    }

    return Ok(enrollment);
}

[HttpDelete("{id}")]
public async Task<IActionResult> Delete(string id)
{
    var deleted = await _service.DeleteAsync(id);

    if (!deleted)
    {
        return NotFound();
    }

    return NoContent();
}
}