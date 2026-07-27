namespace TmsApi.Models;

public sealed record UpdateStudentRequest(
    string Name,
    decimal GPA,
    uint Version
);