namespace TmsApi.Models;
public class Student
{
// ef core recognizes a property named Id as the primary key
    public int Id{get; set; }
    //In short: = string.Empty makes the property 
    // start as a valid blank value instead of a possibly null value.
    public string RegistrationNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal GPA { get; set; }
    public bool IsActive{get; set;}
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
}