using Microsoft.EntityFrameworkCore;

//Context class
//dotnet add package Microsoft.EntityFrameworkCore
//                                                 .Design
//                                                 .Tools
//                                                 .Sqlite
public class MyContext :DbContext{
  public MyContext(DbContextOptions<MyContext> x):base(x){}
  public DbSet<Emp> Emps{get;set;}
  
}