namespace TmsApi.Models;
public class Student
{
    public int Id{get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal GPA { get; set; }
    public bool IsActive{get; set;}
    public ICollection<Enrollment> Enrollments {get ; set; } = new List<Enrollment>();
    }