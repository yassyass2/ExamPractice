using Microsoft.AspNetCore.Mvc;

[Route("API")]
public class EmpController:Controller{
  IEmpStorage empStorage;
  public EmpController(IEmpStorage empStorage){
    this.empStorage = empStorage;
  }

  [HttpGet]
  public async Task<IActionResult> Get([FromQuery] Guid? id){
    if(id ==null) return Ok(await empStorage.GetAll());
    var x = await empStorage.GetbyId((Guid)id);
    return x==null ? NotFound($"{id}"): Ok(x);
  }

  [HttpPost]
  public async Task<IActionResult> Post([FromBody] Emp emp){
    bool x = await empStorage.Add(emp);
    return x? Ok("Done"): BadRequest("Not added");
  }

  [HttpDelete]
  public async Task<IActionResult> Del([FromQuery] Guid id){
    return Ok(await empStorage.Delete(id));
  }

}