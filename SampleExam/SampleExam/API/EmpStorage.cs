using Microsoft.EntityFrameworkCore;

public class EmpStorage : IEmpStorage
{
    MyContext myContext;
    public EmpStorage(MyContext myContext){
        this.myContext = myContext;
    }
    public async Task<bool> Add(Emp emp)
    {
        Emp? x = myContext.Emps.FirstOrDefault(_=>_.Id==emp.Id);
        if(x!=null) return false;
        await myContext.Emps.AddAsync(emp);
        int n = await myContext.SaveChangesAsync();
        return n>0;
    }

    public async Task<bool> Delete(Guid id)
    {
        Emp? x = await myContext.Emps.FirstOrDefaultAsync<Emp>(_=>_.Id==id);
        if(x==null) return false;
        myContext.Emps.Remove(x);
        int n = await myContext.SaveChangesAsync();
        return n>0; 
    }

    public async Task<IEnumerable<Emp>> GetAll()
    {
        return await myContext.Emps.ToListAsync<Emp>();
    }

    public async Task<Emp?> GetbyId(Guid id)
    {
        return await myContext.Emps.FirstOrDefaultAsync<Emp>(_=>_.Id==id);
    }

    public async Task<bool> Update(Emp emp)
    {
        throw new NotImplementedException();
    }
}