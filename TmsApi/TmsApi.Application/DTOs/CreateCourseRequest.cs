using System.ComponentModel.DataAnnotations;

namespace TmsApi.Application.DTOs;

public record CreateCourseRequest
{
    [Required, RegularExpression(@"^[A-Z]{2,3}-\d{3}$", ErrorMessage = "Code must follow the pattern XX-000 or XXX-000 (e.g., CS-101, CSE-101).")]
    public required string Code { get; init; }

    [Required, MaxLength(200)]
    public required string Title { get; init; }

    [Range(1, 200)]
    public int MaxCapacity { get; init; }
}
