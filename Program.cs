using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TmsApi.Authentication;
using TmsApi.Data;
using TmsApi.Entities;
using TmsApi.Exceptions;
using TmsApi.Middleware;
using TmsApi.Models;
using TmsApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

builder.Services
    .AddAuthentication(TrainingAuthHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, TrainingAuthHandler>(
        TrainingAuthHandler.SchemeName, _ => { });

builder.Services.AddAuthorization();

builder.Services.AddDbContext<TmsDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("TmsDatabase"));

    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
    }
});


builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
//builder.Services.AddSingleton<EnrollmentWorker>();


builder.Services
    .AddOptions<PaymentOptions>()
    .BindConfiguration(PaymentOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();


var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    TmsDbContext context = scope.ServiceProvider
        .GetRequiredService<TmsDbContext>();

    // Apply pending migrations.
    context.Database.Migrate();

    // Seed only when no students exist.
    if (!context.Students.Any())
    {
        var students = new List<Student>
        {
            new()
            {
                RegistrationNumber = "TMS-2026-0001",
                Name = "Alice Smith",
                GPA = 3.8m,
                IsActive = true
            },
            new()
            {
                RegistrationNumber = "TMS-2026-0002",
                Name = "Bob Jones",
                GPA = 2.9m,
                IsActive = true
            },
            new()
            {
                RegistrationNumber = "TMS-2026-0003",
                Name = "Charlie Brown",
                GPA = 3.4m,
                IsActive = false
            },
            new()
            {
                RegistrationNumber = "TMS-2026-0004",
                Name = "Diana Prince",
                GPA = 3.9m,
                IsActive = true
            },
            new()
            {
                RegistrationNumber = "TMS-2026-0005",
                Name = "Evan Wright",
                GPA = 2.5m,
                IsActive = true
            }
        };

        context.Students.AddRange(students);

        var courses = new List<Course>
        {
            new()
            {
                Code = "CS-101",
                Title = "Introduction to Computer Science",
                Capacity = 30
            },
            new()
            {
                Code = "CS-201",
                Title = "Data Structures and Algorithms",
                Capacity = 25
            },
            new()
            {
                Code = "MAT-101",
                Title = "Calculus I",
                Capacity = 40
            }
        };

        context.Courses.AddRange(courses);

        // Save first so Student and Course IDs are generated.
        context.SaveChanges();

        var enrollments = new List<Enrollment>
        {
            new()
            {
                StudentId = students[0].Id,
                CourseId = courses[0].Id
            },
            new()
            {
                StudentId = students[0].Id,
                CourseId = courses[1].Id
            },
            new()
            {
                StudentId = students[1].Id,
                CourseId = courses[0].Id
            },
            new()
            {
                StudentId = students[3].Id,
                CourseId = courses[1].Id
            }
        };

        context.Enrollments.AddRange(enrollments);
        context.SaveChanges();
    }
}

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


    app.MapOpenApi();
    app.MapScalarApiReference();

app.MapControllers();

/*app.MapGet("/api/assessments/results", () => Results.Ok(new
{
    courseCode = "CS-101",
    studentId = "S-001",
    letterGrade = "A"
})).RequireAuthorization();

app.MapGet("/api/error", () =>
{
    throw new TmsDatabaseException("Simulated database failure.");
});

app.MapGet("/api/enrollments/worker-smoke", async (
    EnrollmentWorker worker,
    CancellationToken ct) =>
{
    await worker.ProcessBatchAsync(ct);
    return Results.Ok("processed");
});*/











app.Run();

