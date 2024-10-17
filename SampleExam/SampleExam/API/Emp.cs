//Entity class // Domain class // Model class 


public class Emp{
  public Guid Id { get; set; }
  public string? Name{get;set;}

    public override string ToString() => $"Id: {Id}, Name: {Name}";
}
