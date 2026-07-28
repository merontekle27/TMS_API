using Microsoft.AspNetCore.Authentication;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//this is where services are registered 

builder.Services
//.AddControllers();
.AddAuthentication("TrainingScheme")
.AddScheme<AuthenticationSchemeOptions, TrainingAuthHandler>("TrainingScheme", options => { });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseRouting();
app. UseAuthentication();
app.UseAuthorization();
app.MapGet("/api/assessments/results", () =>
{
    return Results.Ok(new 
    {
        courseCode="CS-101",
        studentId="S-001",
        letterGrade="A"
    });
})
.RequireAuthorization();
// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();
//app.MapControllers();

app.Run();
