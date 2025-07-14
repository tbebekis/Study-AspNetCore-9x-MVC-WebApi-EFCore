namespace WebApiApp.Library
{
    static public class StatusCodeErrorHandlerWebApi
    {
        static public Task Handle(StatusCodeContext Context)
        {
            int HttpStatus = Context.HttpContext.Response.StatusCode;

            if (HttpStatus >= 400 && HttpStatus <= 599)
            {
                DataResult Result = new();
                Result.ErrorResult(HttpStatus);
                string JsonText = JsonSerializer.Serialize(Result);
                Context.HttpContext.Response.WriteAsync(JsonText);
            }

            return Task.CompletedTask;
        }
    }
}
