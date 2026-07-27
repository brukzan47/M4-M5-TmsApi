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


// -------------------------
// Services
// -------------------------

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();


// Authentication
builder.Services
    .AddAuthentication(TrainingAuthHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, TrainingAuthHandler>(
        TrainingAuthHandler.SchemeName,
        _ => { });


builder.Services.AddAuthorization();


// Database
builder.Services.AddDbContext<TmsDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("TmsDatabase")
    );

    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
    }
});


// Application services
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();


// Payment options
builder.Services
    .AddOptions<PaymentOptions>()
    .BindConfiguration(PaymentOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();



var app = builder.Build();


// -------------------------
// Database migration + seed
// -------------------------

using (IServiceScope scope = app.Services.CreateScope())
{
    TmsDbContext context =
        scope.ServiceProvider.GetRequiredService<TmsDbContext>();


    // Apply migrations automatically
    context.Database.Migrate();


    // Seed database only if empty
    if (!context.Students.Any())
    {
        var students = new List<Student>
        {
            new()
            {
                RegistrationNumber = "TMS-2026-0001",
                Name = "Alice Smith",
                GPA = 3.8m,
                IsActive = true,
                IsDeleted = false
            },

            new()
            {
                RegistrationNumber = "TMS-2026-0002",
                Name = "Bob Jones",
                GPA = 2.9m,
                IsActive = true,
                IsDeleted = false
            },

            new()
            {
                RegistrationNumber = "TMS-2026-0003",
                Name = "Charlie Brown",
                GPA = 3.4m,
                IsActive = false,
                IsDeleted = false
            },

            new()
            {
                RegistrationNumber = "TMS-2026-0004",
                Name = "Diana Prince",
                GPA = 3.9m,
                IsActive = true,
                IsDeleted = false
            },

            new()
            {
                RegistrationNumber = "TMS-2026-0005",
                Name = "Evan Wright",
                GPA = 2.5m,
                IsActive = true,
                IsDeleted = false
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


        // Generate IDs
        context.SaveChanges();



        var enrollments = new List<Enrollment>
        {
            new()
            {
                StudentId = students[0].Id,
                CourseId = courses[0].Id,
                IsArchived = false
            },

            new()
            {
                StudentId = students[0].Id,
                CourseId = courses[1].Id,
                IsArchived = false
            },

            new()
            {
                StudentId = students[1].Id,
                CourseId = courses[0].Id,
                IsArchived = false
            },

            new()
            {
                StudentId = students[3].Id,
                CourseId = courses[1].Id,
                IsArchived = false
            }
        };


        context.Enrollments.AddRange(enrollments);

        context.SaveChanges();
    }
}



// -------------------------
// Middleware pipeline
// -------------------------

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseExceptionHandler();

app.UseStatusCodePages();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();



// -------------------------
// API documentation
// -------------------------

app.MapOpenApi();

app.MapScalarApiReference();


// -------------------------
// Controllers
// -------------------------

app.MapControllers();



// -------------------------
// Optional testing endpoints
// -------------------------

/*
app.MapGet("/api/assessments/results", () =>
{
    return Results.Ok(new
    {
        courseCode = "CS-101",
        studentId = "S-001",
        letterGrade = "A"
    });
})
.RequireAuthorization();


app.MapGet("/api/error", () =>
{
    throw new TmsDatabaseException(
        "Simulated database failure."
    );
});


app.MapGet("/api/enrollments/worker-smoke",
async (
    EnrollmentWorker worker,
    CancellationToken ct) =>
{
    await worker.ProcessBatchAsync(ct);

    return Results.Ok("processed");
});
*/


app.Run();