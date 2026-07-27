using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TmsApi.Entities;

namespace TmsApi.Data.Configurations;

public sealed class EnrollmentConfiguration
    : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.HasKey(enrollment => enrollment.Id);

        builder.HasIndex(enrollment => new
        {
            enrollment.StudentId,
            enrollment.CourseId
        })
        .IsUnique();

        builder.Property(enrollment => enrollment.Grade)
            .HasPrecision(4, 2);

        builder.Property(enrollment => enrollment.IsArchived)
            .HasDefaultValue(false);

        builder.HasOne(enrollment => enrollment.Student)
            .WithMany(student => student.Enrollments)
            .HasForeignKey(enrollment => enrollment.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(enrollment => enrollment.Course)
            .WithMany(course => course.Enrollments)
            .HasForeignKey(enrollment => enrollment.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        // Filter out enrollments of deleted students.
        builder.HasQueryFilter(e => !e.Student!.IsDeleted);
    }
}