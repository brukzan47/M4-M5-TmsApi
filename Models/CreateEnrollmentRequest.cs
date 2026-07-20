using System.ComponentModel.DataAnnotations;

namespace TmsApi.Models;

public sealed record CreateEnrollmentRequest(
    [property: Required] int StudentId,
    [property: Required] int CourseId);
