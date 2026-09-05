using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace TmsApi.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/transcripts")]
[Route("api/v2/transcripts")]
[ApiVersion("2.0")]
public class TranscriptsController : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("transcripts")]
    public async Task<IActionResult> RequestTranscript([FromBody] object? _, CancellationToken ct)
    {
        // Simulate a brief in-flight execution so concurrency limits can be measured
        await Task.Delay(1000, ct);
        return Ok();
    }
}
