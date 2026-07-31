using System.Diagnostics; //impoorts Stopwatch class for stopwatch
//middleware is a code that runs on every HTTP request
public class RequestLoggingMiddleware
{
    //request delegate is a function that takes an HttpContext and returns a Task
    //Holds the next middleware delegate in the pipeline
    //This is the function that this middlware calls to continue request processing
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;//holds a logger instance used to write log messages.
public RequestLoggingMiddleware(
    RequestDelegate next, 
    ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

//This is the middleware execution method
//ASP.NET Core calls it for each incoming request

    public async Task InvokeAsync(HttpContext context)
    {
      var correlationId= Guid.NewGuid().ToString("N")[..8];
      //Adds a custom response header named X-Correlation-Id.
      context.Response.Headers["X-Correlation-Id"] = correlationId;
      
      //Starts a timer to measure request duration
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation(
    "Incoming Request : {Method} {Path} CorrelationId= {CorrelationId}",
    context.Request.Method, 
    context.Request.Path, 
    correlationId
        );
//Calls the next middleware in the pipeline.
//This is where the request is actually forwarded and later handled.
//Execution resumes here after downstream middleware and endpoint handling complete.

        await _next(context); //calls the next middleware in the pipeline

        stopwatch.Stop(); //stops the stopwatch after the request has been processed

        _logger.LogInformation(
            "Outgoing Response : StatusCode={StatusCode} Elapsed={Elapsed}ms CorrelationId={CorrelationId}",
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds,
            correlationId
        );
    }
}

