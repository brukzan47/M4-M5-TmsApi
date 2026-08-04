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

/*builder.Services
    .AddAuthentication(TrainingAuthHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, TrainingAuthHandler>(
        TrainingAuthHandler.SchemeName, _ => { });
*/

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
//builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

builder.Services.AddScoped<ICourseService,CourseService>();
//builder.Services.AddSingleton<EnrollmentWorker>();


builder.Services
    .AddOptions<PaymentOptions>()
    .BindConfiguration(PaymentOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<TmsDbContext>();
await DataSeeder.SeedAsync(context);
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



app.Run();

