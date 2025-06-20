namespace WebApiApp.Library
{


    /// <summary>
    /// A global exception handler
    /// <para>Catches <strong>any</strong> exception thrown by an application (in controllers, actions, views, pages, etc), 
    /// except of exceptions thrown by the application startup code.</para>
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
    public class GlobalExceptionHandlerWebApi : IExceptionHandler
    {
        public GlobalExceptionHandlerWebApi()
        {
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            DataResult Result = new();
            Result.ExceptionResult(exception);

            string JsonText = JsonSerializer.Serialize(Result);
            await httpContext.Response.WriteAsync(JsonText);

            return true;
        }
    }
}
