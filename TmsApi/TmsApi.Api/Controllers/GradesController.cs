using Microsoft.AspNetCore.Mvc;

namespace TmsApi.Api.Controllers;

public record GradePayload(int StudentId, int CourseId, decimal Score);

[ApiController]
[Route("api/grades")]
public class GradesController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PostGrade([FromBody] GradePayload payload, CancellationToken ct)
    {
        // Simulate real-world database / calculation latency so slow-network exhaustMap testing is observable
        await Task.Delay(500, ct);

        var recordId = Guid.NewGuid().ToString("N")[..8];
        return Ok(new { id = recordId, success = true });
    }
}
