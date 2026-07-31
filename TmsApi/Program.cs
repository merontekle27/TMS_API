using Microsoft.AspNetCore.Authentication;

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
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

// Add this line
app.UseAuthentication();

// Keep this after UseAuthentication
app.UseAuthorization();

app.MapControllers();

app.Run();