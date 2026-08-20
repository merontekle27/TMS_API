//import all the Ef classes like DbContext,DbSet,migration,Modelbuilder..
using Microsoft.EntityFrameworkCore;
using TmsApi.Models;

namespace TmsApi.Data;

public class TmsDbContext : DbContext //our database context inherits all the  func of EF Core's DbContext
{
    //this constructor tells when creating this context, use the db config provided in program.cs
    public TmsDbContext(DbContextOptions<TmsDbContext> options)
        : base(options)
    {
    }
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
}