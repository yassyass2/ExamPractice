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
    [RequireCustomHeaderFilter()]
    public async Task<IActionResult> PostEmployee([FromBody] Employee employee)
    {
        await _employeeStorage.Add(employee);
        return Ok();
    }
    [AdminAuthorizationFilter("Admin")]
    [ResponseModifyFilter()]
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

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee([FromBody] Employee emp, [FromRoute] Guid? id = null){
        if (id == null || emp == null) return BadRequest("you didn't pass an ID or body");
        return await _employeeStorage.Update((Guid)id, emp) ? Ok("updated succesfully") : BadRequest("no id or body given, or employee not existing.");
    }

    [HttpDelete("")]
    public async Task<IActionResult> DeleteEmployee([FromQuery] Guid? id = null){
        if (id == null) return BadRequest("no id given");
        return await _employeeStorage.Delete((Guid)id) ? Ok("employee deleted") : BadRequest("employee didn't exist anyway");
    }
}

public interface IEmployeeStorage{
    Task Add(Employee employee);
    Task<bool> Update(Guid id, Employee employee);
    Task<bool> Delete(Guid id);
    Task<IEnumerable<Employee>> GetAll();
    Task<Employee> GetbyId(Guid id);
}

public class EmployeeStorage : IEmployeeStorage{
    private readonly Mycontext _context;
    public EmployeeStorage(Mycontext context){
        _context = context;
    }


    public async Task Add(Employee employee){
        if (employee != null && _context.Employees.FirstOrDefault(x => x.Id == employee.Id) == null)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }
    }
    public async Task<Employee> GetbyId(Guid id){
        return await _context.Employees.FirstOrDefaultAsync(_ => _.Id == id);
    }

    public async Task<IEnumerable<Employee>> GetAll()
    {
        return await _context.Employees.ToListAsync();
    }

    public async Task<bool> Delete(Guid id){
        var Emp = await _context.Employees.FirstOrDefaultAsync(_ => _.Id == id);
        if (Emp == null) return false;
        _context.Employees.Remove(Emp);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> Update(Guid id, Employee emp){
        if (id == null || emp == null) return false;
        var Emp = await _context.Employees.FirstOrDefaultAsync(_ => _.Id == id);
        if (Emp == null) return false;
        Emp.Id = emp.Id;
        Emp.Name = emp.Name;
        return await _context.SaveChangesAsync() > 0;
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

public class ResponseModifyFilter : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next){
        await next();
        context.HttpContext.Response.Headers.Add("added-header", "nice");
    }
}

public class RequireCustomHeaderFilter : Attribute, IAsyncActionFilter{
    public async Task OnActionExecutionAsync (ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.HttpContext.Request.Headers["POST_KEY"].FirstOrDefault() == null)
        {
            context.HttpContext.Response.Headers.Add("NoPostKey","next time add header POST_KEY");
            context.Result = new BadRequestObjectResult("next time add header POST_KEY");
        }
        else if (context.HttpContext.Request.Headers["POST_KEY"] != "lol1"){
            context.HttpContext.Response.Headers.Add("WrongPostKey","wrong header POST_KEY");
            context.Result = new UnauthorizedObjectResult("wrong PostKey");
        }
        next();
    }
}