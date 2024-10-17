using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddTransient<IEmpStorage, EmpStorage>();
builder.Services.AddDbContext<MyContext>
(x => x.UseSqlite(
  builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();
app.MapControllers();
app.Urls.Add("https://localhost:5236");
app.MapGet("/", () => "Hello World!");

app.UseMiddleware<LogRequestMiddleware>();
app.UseRouting();

app.Run();

public class LogRequestMiddleware
{
  private readonly RequestDelegate _next;

  public LogRequestMiddleware(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    context.Request.EnableBuffering();
    string requestBody = "";
    using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
    {
      requestBody = await reader.ReadToEndAsync();
      context.Request.Body.Position = 0;
    }

    var originalBodyStream = context.Response.Body;
    using (var responseBodyStream = new MemoryStream())
    {
      context.Response.Body = responseBodyStream;

      await _next(context);

      context.Response.Body.Seek(0, SeekOrigin.Begin);
      var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
      context.Response.Body.Seek(0, SeekOrigin.Begin);

      string requestHeaders = FormatHeaders(context.Request.Headers);
      string responseHeaders = FormatHeaders(context.Response.Headers);

      var toLog = new Dictionary<string, string>
      {
          { "status code", $"{context.Response.StatusCode}" },
          { "request headers", requestHeaders },
          { "request body", requestBody },
          { "request path", $"{context.Request.Path}" },
          { "response body", responseBody },
          { "response headers", responseHeaders }
      };

      foreach (var key in toLog)
      {
        File.AppendAllText("log.txt", $"{key.Key}: {key.Value}\n");
      }

      await responseBodyStream.CopyToAsync(originalBodyStream);
    }
  }

  private string FormatHeaders(IHeaderDictionary headers)
  {
    var formattedHeaders = new List<string>();
    foreach (var header in headers)
    {
      formattedHeaders.Add($"{header.Key}: {header.Value}");
    }
    return string.Join("\n", formattedHeaders);
  }
}

