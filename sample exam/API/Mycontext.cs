// shadow foreign key = nor accessible
// context class

using Microsoft.EntityFrameworkCore;
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