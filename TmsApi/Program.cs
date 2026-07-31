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

builder.Services.AddSingleton<IEnrollmentService, EnrollmentService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

// Add this line
app.UseAuthentication();

// Keep this after UseAuthentication
app.UseAuthorization();

app.MapControllers();

app.Run();