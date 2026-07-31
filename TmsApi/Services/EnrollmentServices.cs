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
    //Why inject ILogger? in Session 1 You created logging middleware.
//Now the service itself can also write logs:
    private readonly ILogger<EnrollmentService> _logger;

    public EnrollmentService(ILogger<EnrollmentService> logger)
    {
        _logger = logger;
    }

    // Methods will go here
    //This method creates a new enrollment.
    public Task<EnrollmentRecord> EnrollAsync(string studentId, string courseCode)
{
    //generates an ID
    var id = Guid.NewGuid().ToString("N")[..8];
//creates a new EnrollmentRecord
    var record = new EnrollmentRecord(
        id,
        studentId,
        courseCode,
        DateTime.UtcNow);

//Saves it in memory (in the dictionary)
    _store[id] = record;
//Logs
    _logger.LogInformation(
        "Enrolled {StudentId} in {CourseCode} record {EnrollmentId}",
        studentId,
        courseCode,
        id);
//return the record
    return Task.FromResult(record);
}

//This method returns one enrollment using its ID.
    public Task<EnrollmentRecord?> GetByIdAsync(string id)
    {
        //TryGetValue returns true if the key exists, and false if it doesn't. If it exists, the value is returned in the out parameter.
    _store.TryGetValue(id, out var record);
    return Task.FromResult(record);
    }

//This method returns all enrollments.
    public Task<IReadOnlyList<EnrollmentRecord>> GetAllAsync()
    {
        //_store.Values contains all the enrollments in memory.
        //ToList() converts them into a list that can be returned

    IReadOnlyList<EnrollmentRecord> all = _store.Values.ToList();
    return Task.FromResult(all);
    }
    //This method deletes an enrollment using its ID.
    public Task<bool> DeleteAsync(string id)
    {
    var removed = _store.Remove(id);
    return Task.FromResult(removed);
    }
    }
public record EnrollmentRecord(
    string Id,
    string StudentId,
    string CourseCode,
    DateTime EnrolledAt
);