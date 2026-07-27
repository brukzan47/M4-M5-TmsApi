using Microsoft.EntityFrameworkCore;
using TmsApi.Entities;

namespace TmsApi.Data;

public sealed class TmsDbContext(
    DbContextOptions<TmsDbContext> options)
    : DbContext(options)
{
    public DbSet<Student> Students => Set<Student>();

    public DbSet<Course> Courses => Set<Course>();

    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(TmsDbContext).Assembly
        );
    }

    public override int SaveChanges()
    {
        UpdateStudentAuditValues();

        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        UpdateStudentAuditValues();

        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateStudentAuditValues()
    {
        DateTime currentTime = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<Student>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                entry.Property("LastUpdated").CurrentValue = currentTime;
            }
        }
    }
}