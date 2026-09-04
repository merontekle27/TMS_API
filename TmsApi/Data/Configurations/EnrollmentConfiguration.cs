using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TmsApi.Models;

namespace TmsApi.Data.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Grade)
            .HasPrecision(3, 2);

        builder.Property(e => e.EnrolledAt)
            .IsRequired();

        // DeleteBehavior.Restrict prevents accidental cascade deletion of student academic records and enrollment history when a student or course is deleted.
        builder.HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        // DeleteBehavior.Restrict ensures a course with active enrollments cannot be cascade-deleted without first handling its enrolled student records.
        builder.HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
