namespace TmsApi.Entities;

public class Enrollment
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public Student? Student { get; set; }

    public int CourseId { get; set; }

    public Course Course { get; set; } = null!;

    public decimal? Grade { get; set; }

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    // Used by ExecuteUpdateAsync in Exercise 9.
    public bool IsArchived { get; set; }
}