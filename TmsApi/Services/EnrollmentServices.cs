public interface IEnrollmentService
{
Task<EnrollmentRecord> EnrollAsync(string studentId, string courseCode);
Task<EnrollmentRecord?> GetByIdAsync(string id);
Task<IReadOnlyList<EnrollmentRecord>> GetAllAsync();
Task<bool> DeleteAsync(string id);
}
//a record is a special type used to represent data with value
//Unlike a normal class, a record is intended to store data rather than behavior.

public class EnrollmentService : IEnrollmentService
{
    //Dictionary ..instead of a database, we're temporarily using memory.
    private readonly Dictionary<string, EnrollmentRecord> _store = new();
    //RWhy inject ILogger? in Session 1 You created logging middleware.
//Now the service itself can also write logs:
    private readonly ILogger<EnrollmentService> _logger;

    public EnrollmentService(ILogger<EnrollmentService> logger)
    {
        _logger = logger;
    }

    // Methods will go here
}
public record EnrollmentRecord(
    string Id,
    string StudentId,
    string CourseCode,
    DateTime EnrolledAt
);