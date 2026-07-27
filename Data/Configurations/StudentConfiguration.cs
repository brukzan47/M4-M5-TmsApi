using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TmsApi.Entities;

namespace TmsApi.Data.Configurations;

public sealed class StudentConfiguration
    : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(student => student.Id);

        builder.Property(student => student.RegistrationNumber)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(student => student.RegistrationNumber)
            .IsUnique();

        builder.Property(student => student.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(student => student.GPA)
            .HasPrecision(4, 2);

        // Shadow audit property: not added to the Student C# class.
        builder.Property<DateTime>("LastUpdated")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Optimistic concurrency using PostgreSQL xmin.
        builder.Property(student => student.Version)
            .IsRowVersion();

        // Normal queries automatically hide soft-deleted students.
        builder.HasQueryFilter(student => !student.IsDeleted);
    }
}