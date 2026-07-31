using Microsoft.AspNetCore.Mvc;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentController : ControllerBase
{
    // This controller handles HTTP requests related to enrollments. It uses the IEnrollmentService to perform operations on enrollment records.
    private readonly IEnrollmentService _service;

    public EnrollmentController(IEnrollmentService service)
    {
        _service = service;
        
        
    }
    // This action method handles GET requests to retrieve an enrollment record by its ID. It returns a 404 Not Found response if the record does not exist.
   // The controller:
// 1. Receives the request.
// 2. Calls EnrollAsync().
// 3. Returns the created enrollment.
    [HttpPost]
public async Task<IActionResult> Enroll(string studentId, string courseCode)
{
    var result = await _service.EnrollAsync(studentId, courseCode);
    return Ok(result);
}
[HttpGet]
public async Task<IActionResult> GetAll()
{
    var enrollments = await _service.GetAllAsync();
    return Ok(enrollments);
}
}