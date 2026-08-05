using Microsoft.AspNetCore.Authentication;
using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//this is where we register services that will be used by the application, such as controllers, authentication, and authorization services.
builder.Services.AddControllers();

builder.Services
    .AddAuthentication("TrainingScheme")
    .AddScheme<AuthenticationSchemeOptions, TrainingAuthHandler>(
        "TrainingScheme",
        options => { });

builder.Services.AddAuthorization();
//A singleton lives for the entire app
//builder.Services.AddSingleton<IEnrollmentService, EnrollmentService>();
// A scoped service lives once per HTTP request, which is the correct 
// lifetime for services that will eventually work with a database(like EnrollmentService)
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.Configure<EnrollmentOptions>(
    builder.Configuration.GetSection("Enrollment"));
    builder.Services.AddProblemDetails();
    builder.Services.AddOpenApi();
    builder.Services.AddDbContext<TmsDbContext>(options =>
     options.UseNqsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}
app.UseExceptionHandler();
// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

// Add this line
app.UseAuthentication();

// Keep this after UseAuthentication
app.UseAuthorization();

app.MapControllers();

app.MapGet("/api/error", () =>
{
    throw new TmsDatabaseException(
        "Simulated database failure for ProblemDetails testing");
});
app.Run();