using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IEmployeeStorage, EmployeeStorage>();
builder.Services.AddDbContext<Mycontext>(X => X.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.MapControllers();
app.Urls.Add("https://localhost:5236");
app.UseMiddleware<LogRequestMiddleware>();

app.Run();

public class Employee{
    public Guid Id { get; set;}
    public string Name { get; set;}

    public Employee(Guid id, string name) {
        Id = id;
        Name = name;
    }
}

public class Mycontext : DbContext
{
    public Mycontext(DbContextOptions<Mycontext> x) : base(x) { }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Dept> Depts { get; set; }
    public DbSet<Proj> Projs { get; set; }
    public DbSet<EmpProj> EmpProjs { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Proj>().HasKey(_ => _.Name);
        modelBuilder.Entity<EmpProj>().HasKey(t => new { t.EmpId, t.ProjName });
    }
}

[Route("api/employees")]
public class EmployeeController : Controller{
    private readonly IEmployeeStorage _employeeStorage;

    public EmployeeController(IEmployeeStorage employeeStorage)	
    {
        _employeeStorage = employeeStorage;
    }

    [HttpPost("")]
    public async Task<IActionResult> PostEmployee([FromBody] Employee employee)
    {
        await _employeeStorage.Add(employee);
        return Ok();
    }
    [AdminAuthorizationFilter("Admin")]
    [HttpGet("")]
    public async Task<IActionResult> GetEmployee([FromQuery] Guid? id = null)
    {
        if (id == null)
        {
            return Ok(await _employeeStorage.GetAll());
        }
        else
        {
            var employee = await _employeeStorage.GetbyId((Guid)id);
            return employee == null ? NotFound($"employee with id {id} not found") : Ok(employee);
        }
    }
}

public interface IEmployeeStorage{
    Task Add(Employee employee);
    Task<bool> Update(Guid id, Employee employee);
    Task<bool> Delete(Guid id);
    Task<List<Employee>> GetAll();
    Task<Employee> GetbyId(Guid id);
}

public class EmployeeStorage : IEmployeeStorage{
    private readonly Mycontext _context;
    public EmployeeStorage(Mycontext context){
        _context = context;
    }


    public async Task Add(Employee employee){
        if (employee != null)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }
    }
    public async Task<Employee> GetbyId(Guid id){
        return await _context.Employees.FirstOrDefaultAsync(_ => _.Id == id);
    }

    public async Task<List<Employee>> GetAll()
    {
        return await _context.Employees.ToListAsync();
    }

    public async Task<bool> Delete(Guid id){
        var Emp = await _context.Employees.FirstOrDefaultAsync(_ => _.Id == id);
        if (Emp == null) return false;
        _context.Employees.Remove(Emp);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Update(Guid id, Employee emp){
        var Emp = await _context.Employees.FirstOrDefaultAsync(_ => _.Id == id);
        if (Emp == null) return false;
        Emp.Id = emp.Id;
        Emp.Name = emp.Name;
        await _context.SaveChangesAsync();
        return true;
    }
}

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

    await _next(context);

    File.AppendAllText("log.txt", $"\nRequest at Route: {context.Request.Path}\n");
    File.AppendAllText("log.txt", $"Request method used: {context.Request.Method}\n");
    File.AppendAllText("log.txt", $"Response status code: {context.Response.StatusCode}\n");
  }
}

public class AdminAuthorizationFilter : Attribute, IAuthorizationFilter
{
    private readonly string _adminToken;

    public AdminAuthorizationFilter(string adminToken)
    {
        _adminToken = "Admin";
    }

    public void OnAuthorization(AuthorizationFilterContext con)
    {
        var authHeader = con.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
        Console.WriteLine(authHeader);
        if (authHeader == null || authHeader != "Admin"){
            con.Result = new UnauthorizedResult();
        }
    }
}