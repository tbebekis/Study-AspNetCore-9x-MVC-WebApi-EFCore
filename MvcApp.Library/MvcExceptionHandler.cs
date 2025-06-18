namespace MvcApp.Library
{

    /// <summary>
    /// A global exception handler
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
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            DataResult Result = new();
            Result.ExceptionResult(exception); 
            string JsonText = Json.Serialize(Result);
            await httpContext.Response.WriteAsync(JsonText);
 
            return true;
        }
    }
}
