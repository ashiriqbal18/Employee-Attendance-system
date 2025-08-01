using final.Models;
using final.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace final.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeApiController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeApiController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        // GET: api/EmployeeApi
        [HttpGet]
        public IActionResult GetAll()
        {
            var employees = _employeeRepository.GetAll();
            return Ok(employees);
        }

        // GET: api/EmployeeApi/1
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var emp = _employeeRepository.GetById(id);
            if (emp == null) return NotFound("Employee not found");
            return Ok(emp);
        }

        // POST: api/EmployeeApi
        [HttpPost]
        public async Task<IActionResult> Add(Employee employee)
        {
            var result = await _employeeRepository.Add(employee);
            if (!result) return BadRequest("Failed to add employee.");
            return Ok(employee);
        }

        // PUT: api/EmployeeApi/1
        [HttpPut("{id}")]
        public IActionResult Update(int id, Employee updated)
        {
            var existing = _employeeRepository.GetById(id);
            if (existing == null) return NotFound("Employee not found");

            updated.Id = id;
            var success = _employeeRepository.Update(updated);
            if (!success) return BadRequest("Update failed");
            return Ok(updated);
        }

        // DELETE: api/EmployeeApi/1
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existing = _employeeRepository.GetById(id);
            if (existing == null) return NotFound("Employee not found");

            var success = _employeeRepository.Delete(id);
            if (!success) return BadRequest("Delete failed");
            return Ok("Deleted");
        }
    }
}
