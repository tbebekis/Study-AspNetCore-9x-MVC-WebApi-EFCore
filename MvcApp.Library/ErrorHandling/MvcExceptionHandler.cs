namespace MvcApp.Library
{

    /// <summary>
    /// Catches <strong>any</strong> exception thrown by an application (in controllers, views, pages, etc), except of exceptions thrown by the application startup code.
    /// <para>The developer may use this handler in order to log the error or even write directly to the response stream.</para>
    /// <para>In case the error is handled the handler must return <strong>true</strong>.</para>
    /// <para>To register</para>
    /// <code>
    ///   builder.Services.AddExceptionHandler&lt;GlobalExceptionHandler&gt;();
    ///   builder.Services.AddProblemDetails();
    /// </code>
    /// <para>SEE: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling</para>
    /// <para>SEE: https://learn.microsoft.com/en-us/aspnet/core/web-api/handle-errors</para>
    /// </summary>
    public class MvcExceptionHandler : IExceptionHandler
    {
        public MvcExceptionHandler()
        {
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            // use the following if you want to directly write to the response stream
            /* 
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            DataResult Result = new();
            Result.ExceptionResult(exception); 
            string JsonText = Json.Serialize(Result);
            await httpContext.Response.WriteAsync(JsonText);
 
            return true;
            */

            await Task.CompletedTask;
            return false;
        }
    }
}
