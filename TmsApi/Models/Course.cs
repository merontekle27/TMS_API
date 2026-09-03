namespace TmsApi.Models;

public class Course
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Capacity { get; set; }
   public ICollection<Enrollment> Enrollments {get; set;}= new List<Enrollment>();
}