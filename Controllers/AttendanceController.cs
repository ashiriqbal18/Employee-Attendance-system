using final.Models;
using final.Repository;
using Microsoft.AspNetCore.Mvc;

namespace final.Controllers
{
    public class AttendanceController : BaseController
    {
        private readonly IAttendanceRepository _attendanceRepository;

        public AttendanceController(IAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAll()
        {
            var records = _attendanceRepository.GetAll();
            return Json(new { success = true, data = records });
        }

        [HttpGet]
        public JsonResult GetByEmployeeId(int empId)
        {
            var records = _attendanceRepository.GetByEmployeeId(empId);
            return Json(new { success = true, data = records });
        }

        [HttpPost]
        [Route("Attendance/Add")]
        public JsonResult Add([FromBody] Attendance attendance)
        {
            if (attendance == null)
                return Json(new { success = false, message = "Attendance object is null" });

            if (attendance.EmployeeId == 0)
                return Json(new { success = false, message = "EmployeeId is missing or zero" });

            if (string.IsNullOrWhiteSpace(attendance.AttendanceType))
                return Json(new { success = false, message = "AttendanceType is missing" });

            if (attendance.Date == DateTime.MinValue)
                return Json(new { success = false, message = "Date is invalid" });

            try
            {
                _attendanceRepository.Add(attendance);
                return Json(new { success = true, message = "Attendance added successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet]
        public JsonResult SearchAttendance(string? search)
        {
            var result = _attendanceRepository.SearchAttendance(search);
            return Json(new { success = true, data = result });
        }
        [HttpGet]
        public JsonResult GetAllNamedAttendence()
        {
            var data = _attendanceRepository.GetAllEmployeeAttendance();
            return Json(new { success = true, data = data });
        }
    }
}
