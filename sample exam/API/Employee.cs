// model class/entity class
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
public class Employee
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public Employee(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}

public class Dept
{
    [Key]
    public int No { get; set; }
    public string? Name { get; set; }
    public ICollection<Employee> Employees { get; set; }
}

public class Proj()
{
    [Key]
    public string Name { get; set; }
}

public class EmpProj
{
    // composite primary key
    public Employee Emp { get; set; } = null!;
    public Guid EmpId { get; set; }
    public string ProjName { get; set; }
    public string Name { get; set; }
}

