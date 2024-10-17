using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// builder.Services.AddScoped<EmployeeStorage>();
builder.Services.AddDbContext<Mycontext>(X => X.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapControllers();
app.Urls.Add("https://localhost:5236");

app.Run();
