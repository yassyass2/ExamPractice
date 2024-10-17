## Sample Exam

Create a new EmptyFolder named as SampleExam

**Exercise 1**: Create a new minimal api named as API in the exam folder using command `dotnet new web -n API`
- Create a new Controller named as `EmployeesController` able to handel httpGet and Post requests. 
- Post request accepts an Employee object from the Body of http request containing Guid Id, and string Name.
- Get request accepts an optional querystring parameter Id as Guid
- Provide dummy implementation for both end points
-*Optional* write a .rest file to send http requests. You may also required to create a record or class for Employee.

**Exercise 2**: Create a service for `EmployeeStorage` implementing an `IEmployeeStorage` Interface  at the moment the service is *dummy* but register the service and modify the controller that the controller should be able to call the service. 
The service interface should have following methods:
- Add
- Update
- Delete
- GetbyId
- GetAll

https://github.com/hogeschool/webdev-semester/tree/main/24-25


dotnet ef migrations add m1
dotnet ef database update


