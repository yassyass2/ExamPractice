using Microsoft.AspNetCore.Mvc;

[Route("Employees")]
public class EmployeesController : Controller
{
    private readonly IEmployeeStorage employeeStorage;

    public EmployeesController(IEmployeeStorage employeeStorage)
    {
        this.employeeStorage = employeeStorage;
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateEmployee([FromBody] Employee employee)
    {
        return Ok(await employeeStorage.Add(employee));
    }

    [HttpGet("")]
    public async Task<IActionResult> GetEmployee([FromQuery] Guid? id)
    {
        if (id == null)
        {
            return Ok(await employeeStorage.GetAll());
        }
        else
        {
            return Ok(await employeeStorage.GetByID((Guid)id));
        }
    }
    // dotnet-ef migrations add m1
    // dotnet-ef database update
}