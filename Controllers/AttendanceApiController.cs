using final.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace final.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceApiController : ControllerBase
    {
        private readonly IAttendanceRepository _attendanceRepository;

        public AttendanceApiController(IAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        // GET: api/AttendanceApi
        [HttpGet]
        public IActionResult GetAll()
        {
            var attendances = _attendanceRepository.GetAllEmployeeAttendance();
            if (attendances == null) return NotFound("No attendence found");
            return Ok(attendances);
        }

        // GET: api/AttendanceApi/employee/5
        [HttpGet("employee/{empId}")]
        public IActionResult GetByEmployeeId(int empId)
        {
            var records = _attendanceRepository.GetByEmployeeId(empId);
            if (records == null || !records.Any()) return NotFound("No attendance records found for this employee");
            return Ok(records);
        }

        // POST: api/AttendanceApi
        [HttpPost]
        public IActionResult Add([FromBody] Attendance attendance)
        {
            if (attendance == null)
                return BadRequest("Invalid attendance data.");

            _attendanceRepository.Add(attendance);
            return Ok(attendance);
        }
    }
}
