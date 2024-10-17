public interface IEmpStorage{
  Task <bool> Add(Emp emp);
  Task <bool> Update(Emp emp);
  Task <bool> Delete(Guid id);
  Task <Emp?> GetbyId(Guid id);
  Task <IEnumerable<Emp>> GetAll();
}
