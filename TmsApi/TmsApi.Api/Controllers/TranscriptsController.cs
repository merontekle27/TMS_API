using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace TmsApi.Api.Controllers;

[ApiController]
[Route("api/v2/transcripts")]
public class TranscriptsController : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("transcripts")]
    public async Task<IActionResult> RequestTranscript([FromBody] object? _, CancellationToken ct)
    {
        // Simulate brief work so the concurrency limiter can measure in-flight executions
        await Task.Delay(500, ct);
        return Ok();
    }
}
