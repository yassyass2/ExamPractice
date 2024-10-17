/*
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

public interface IEmployeeStorage
{
    Task<bool> Add(Employee employee);
    Task<bool> Delete(Guid id);
    Task<bool> Update(Employee employee);
    Task<IEnumerable<Employee>> GetAll();
    Task<Employee> GetByID(Guid id);
}
public class EmployeeStorage : IEmployeeStorage
{
    Mycontext myContext;

    public EmployeeStorage(Mycontext myContext)
    {
        this.myContext = myContext;
    }

    public async Task<bool> Add(Employee employee)
    {
        Employee? x = myContext.Employees.FirstOrDefault(_ => _.Id == employee.Id);
        if (x != null) return false;
        await myContext.Employees.AddAsync(employee);
        int changes = await myContext.SaveChangesAsync();
        return changes > 0;
    }

    public async Task<bool> Update(Employee employee)
    {
        myContext.Employees.Update(employee);
        int changes = await myContext.SaveChangesAsync();
        return changes > 0;
    }

    public async Task<bool> Delete(Guid id)
    {
        Employee? x = await myContext.Employees.FirstOrDefaultAsync<Employee>(_ => _.Id == id);
        if (x == null) return false;
        myContext.Employees.Remove(x);
        int changes = await myContext.SaveChangesAsync();
        return changes > 0;
    }

    public async Task<IEnumerable<Employee>> GetAll()
    {
        return await myContext.Employees.ToListAsync();
    }

    public async Task<Employee?> GetByID(Guid id)
    {
        return await myContext.Employees.FindAsync(id);
    }
}
*/