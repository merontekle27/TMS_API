using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TmsApi.Api.Hubs;
using TmsApi.Application.Enrollments.Commands;
using TmsApi.Application.Enrollments.Queries;
using TmsApi.Application.Hubs;

namespace TmsApi.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/enrollments")]
[Route("api/enrollments")]
[ApiVersion("2.0")]
public class EnrollmentsController(
    IMediator mediator,
    IHubContext<TmsHub, ITmsHubClient> hubContext) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        var sampleEnrollments = new[]
        {
            new { id = "1", studentId = 1, studentName = "Liya Kebede", courseId = 101, courseName = "Advanced Java Services", status = "Pending", enrolledAt = DateTimeOffset.UtcNow.ToString("O") },
            new { id = "2", studentId = 2, studentName = "Dawit Alemu", courseId = 102, courseName = "Angular UI Lab", status = "Approved", enrolledAt = DateTimeOffset.UtcNow.ToString("O") },
            new { id = "3", studentId = 3, studentName = "Sara Tesfaye", courseId = 103, courseName = "Database Design", status = "Pending", enrolledAt = DateTimeOffset.UtcNow.ToString("O") },
            new { id = "4", studentId = 4, studentName = "Abebe Bikila", courseId = 104, courseName = "API Security Workshop", status = "Pending", enrolledAt = DateTimeOffset.UtcNow.ToString("O") }
        };
        return Ok(sampleEnrollments);
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> Approve(string id, CancellationToken ct)
    {
        await hubContext.Clients.All.ReceiveEnrollmentStatusUpdated(id, "Approved");
        return NoContent();
    }
    [HttpPost]
    public async Task<IActionResult> Enroll(
        EnrollStudentCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.Match<IActionResult>(
            onSuccess: created => CreatedAtAction(
                nameof(GetSchedule),
                new { studentId = created.StudentId },
                created),
            onFailure: error =>
            {
                var status = error.Code switch
                {
                    "course_not_found" => StatusCodes.Status404NotFound,
                    "course_full" or "already_enrolled" => StatusCodes.Status409Conflict,
                    _ => StatusCodes.Status400BadRequest
                };
                return Problem(
                    statusCode: status,
                    title: "Enrollment rejected",
                    detail: error.Message,
                    type: $"https://tms.local/errors/{error.Code}");
            });
    }

    [HttpGet("{studentId}/schedule")]
    public async Task<IActionResult> GetSchedule(
        int studentId, CancellationToken ct)
    {
        var schedule = await mediator.Send(
            new GetStudentScheduleQuery(studentId), ct);
        return Ok(schedule);
    }
}
