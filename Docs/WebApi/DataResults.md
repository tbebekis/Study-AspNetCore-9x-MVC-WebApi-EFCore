# Api Data Results

> This text is part of a group of texts describing [Asp.Net Core](../Index.md).

A WebApi application should return consistent responses.

## Problem description

Let us consider a WebApi application which registers an exception handler in the application startup code in order to trap unhandled exceptions. 

```
public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args); 
    builder.Services.AddControllers();      

    var app = builder.Build();

    //if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler(AppBuilder => {
            AppBuilder.Run(async HttpContext =>
            {
                HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                HttpContext.Response.ContentType = Text.Plain;

                string Message = "Unknown error";

                var ExceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
                if (ExceptionHandlerFeature != null && ExceptionHandlerFeature.Error != null)
                    Message = ExceptionHandlerFeature.Error.Message;

                await HttpContext.Response.WriteAsync(Message);
            });
        });

        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
```

Furthermore the application contains a controller, something like the following, which returns a list of products.

```
[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    [HttpGet("list")]
    public IEnumerable<ProductModel> List()
    {
        List<ProductModel> products = new List<ProductModel>()
        {
            new Product() { Name = "Product 1", Price = 12.34M },
            new Product() { Name = "Product 2", Price = 56.78M }
        };

        return products;
    }
}
```

The model used is the following.

```
public class ProductModel
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```

Opening a browser and calling the Url `https://localhost:7134/product/list` results in the following `JSON` text.

```
[
  {
    "name": "Product 1",
    "price": 12.34
  },
  {
    "name": "Product 2",
    "price": 56.78
  }
]
```

The client application expects a `JSON` array of products in success. 

But in order to deal with a possible failure of its request it has to **first** check the [HTTP Status](https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Status) of the response, before attempting to deserialize the `JSON` data to a product array. 

And it has to perform this check in every request.

There are ways to have a WebApi application returning structured results even when an exception is thrown.

## Api Data Result class hierarchy.

Consider the following hierarchy of result (response) classes.

```
/// <summary>
/// Result base class
/// </summary>
public class DataResult
{
    protected const string SCheckErrorList = "Check the Error list";
    List<string> fErrors;

    public DataResult()
    {
    }

    public virtual void AddError(string ErrorText)
    {
        if (!string.IsNullOrWhiteSpace(ErrorText))
        {
            if (fErrors == null)
                fErrors = new List<string>();

            if (!fErrors.Contains(ErrorText))
                fErrors.Add(ErrorText);

            Message = SCheckErrorList;
        }
    }
    public virtual void ClearErrors()
    {
        if (fErrors != null)
            fErrors.Clear();
    }
    public virtual void CopyErrors(DataResult Source)
    {
        this.Status = Source.Status;
        foreach (var error in Source.Errors) 
            this.AddError(error);
    }

    public virtual void SetResult(int Status = -1, string ErrorMessage = null, string Message = null)
    {
        if (Status >= StatusCodes.Status100Continue)
            this.Status = Status;

        if (!string.IsNullOrWhiteSpace(ErrorMessage))
            Errors.Add(ErrorMessage); 

        if (!string.IsNullOrWhiteSpace(Message))
            this.Message = Message;
    }
    public virtual void ErrorResult(int Status, string ErrorMessage = null)
    {
        if (!string.IsNullOrWhiteSpace(ErrorMessage))
            ErrorMessage = $"Error: {ErrorMessage}";
        else if (Status >= 400 && Status <= 599)
            ErrorMessage = Microsoft.AspNetCore.WebUtilities.ReasonPhrases.GetReasonPhrase(Status);
        else if (Status >= 1000)
            ErrorMessage = ApiStatusCodes.StatusCodeToMessage[Status];
        else
            ErrorMessage = $"Error: {Status}";

        SetResult(Status, ErrorMessage, SCheckErrorList);
    }
    public virtual void ExceptionResult(Exception Ex)
    {
        string ErrorMessage = Ex == null? "Unspecified Exception": $"Exception: {Ex.GetType().Name}";
        ErrorResult(ApiStatusCodes.Exception, ErrorMessage);

        if (Ex != null)
        {
            List<string> ExceptionErrorList = Ex.GetErrors();
            Errors.AddRange(ExceptionErrorList);
        }
 
    }
    public virtual void NoDataResult(string Message = null)
    {
        Message = !string.IsNullOrWhiteSpace(Message) ? $"No data: {Message}" : "No data.";
        SetResult(Status: ApiStatusCodes.NoData, Message: Message);
    }


    public int Status { get; set; } = StatusCodes.Status200OK;
    public string Message { get; set; } = "OK";
    public List<string> Errors
    {
        get
        {
            if (fErrors == null)
                fErrors = new List<string>();
            return fErrors;
        }
        set { fErrors = value; }
    }
}

/// <summary>
/// Result for a single item
/// </summary>
public class ItemResult<T> : DataResult
{
    public T Item { get; set; }
}

/// <summary>
/// Result for a list of items
/// </summary>
public class ListResult<T> : DataResult
{
    public List<T> List { get; set; } = new List<T>();
}

/// <summary>
/// Paging information interface
/// </summary>
public interface IPaging
{
    int TotalItems { get; set; }
    int PageSize { get; set; }
    int PageIndex { get; set; }
}

/// <summary>
/// Result for a paged list of items
/// </summary>
public class ListResultPaged<T>: ListResult<T>, IPaging
{
    int IPaging.TotalItems { get => Paging.TotalItems; set => Paging.TotalItems = value; }
    int IPaging.PageSize { get => Paging.PageSize; set => Paging.PageSize = value; }
    int IPaging.PageIndex { get => Paging.TotalItems; set => Paging.PageIndex = value; }

    public ListResultPaged() 
    { 
    }
    public ListResultPaged(IPaging SourcePaging, List<T> SourceList = null)
    {
        SetFrom(SourcePaging, SourceList);
    }


    public void SetFrom(IPaging SourcePaging, List<T> SourceList = null)
    {
        Paging.TotalItems = SourcePaging.TotalItems;
        Paging.PageIndex = SourcePaging.PageIndex;
        Paging.PageSize = SourcePaging.PageSize;

        List = SourceList;
    }

    public Paging Paging { get; set; } = new(); 
}
```

## Using structure results

With the result classes in place the WebApi application can now return consistent responses even in the case of an error.

```
public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args); 
    builder.Services.AddControllers();      

    var app = builder.Build();

    //if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler(AppBuilder => {
            AppBuilder.Run(async HttpContext =>
            {
                string Message = "Unknown error";

                var ExceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
                if (ExceptionHandlerFeature != null && ExceptionHandlerFeature.Error != null)
                    Message = ExceptionHandlerFeature.Error.Message;

                DataResult Result = new();
                Result.ErrorResult(StatusCodes.Status500InternalServerError, Message)
                string JsonText = JsonSerializer.Serialize(Result);

                await HttpContext.Response.WriteAsync(JsonText);
            });
        });

        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
```

Controllers now can return a consistent structured result.

```
[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    [HttpGet("list")]
    public ListResult<Product> List()
    {
        List<ProductModel> products = new List<ProductModel>()
        {
            new Product() { Name = "Product 1", Price = 12.34M },
            new Product() { Name = "Product 2", Price = 56.78M }
        };

        ListResult<Product> ListResult = new();
        ListResult.List = products.

        return ListResult;
    }
}
```

Now the client application knows that it always has to expect at least the following, even in the case of an error, for any reason.

It has just to examine the `Message` string in order to know if its request was a success or a failure.

```
{
  "Status": 200,
  "Message": "OK",
  "Errors": [
    "string"
  ]
}
```

And in a case of success it gets the full list of products.

```
{
  "Status": 200,
  "Message": "OK",
  "Errors": [
    "string"
  ],
  "List": [
    {
      "Id": null,
      "Name": null,
      "Price": 1
    }
  ]
}
```



