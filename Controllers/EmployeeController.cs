using final.helper;
using final.Models;
using final.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace final.Controllers
{
    public class EmployeeController : BaseController
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAttendanceRepository _attendanceRepository;

        public EmployeeController(IEmployeeRepository employeeRepository, IAttendanceRepository attendanceRepo)
        {
            _employeeRepository = employeeRepository;
            _attendanceRepository = attendanceRepo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAllEmployees()
        {
            var employees = _employeeRepository.GetAll();
            return Json(new { success = true, data = employees });
        }

        [HttpGet]
        public JsonResult GetEmployeeById(int id)
        {
            var employee = _employeeRepository.GetById(id);
            if (employee == null)
                return Json(new { success = false, message = "Employee not found" });

            return Json(new { success = true, data = employee });
        }

        [HttpPost]
        public async Task<JsonResult> AddEmployee([FromForm] Employee employee)
        {
            // 🔐 Encrypt password
            if (!string.IsNullOrWhiteSpace(employee.Password))
            {
                employee.Password = EncryptionHelper.Encrypt(employee.Password);
            }

            // Step 1: Save employee without image to get the ID
            var result = await _employeeRepository.Add(employee); // returns true or false

            if (!result)
            {
                return Json(new { success = false, message = "Failed to save employee." });
            }

            // Step 2: Now get the saved employee with ID
            var savedEmployee =  _employeeRepository.GetByEmail(employee.Email); // or by name+email

            // Step 3: Save image using employee ID
            var file = employee.ImageFile;
            if (file != null && file.Length > 0 && savedEmployee != null)
            {
                var extension = Path.GetExtension(file.FileName); // .jpg, .png
                var fileName = $"{savedEmployee.Id}{extension}"; // e.g., "7.jpg"
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/image", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Step 4: Update ImagePath
                savedEmployee.ImagePath = "/image/" + fileName;
                 _employeeRepository.Update(savedEmployee);
            }

            return Json(new
            {
                success = true,
                message = "Employee saved successfully."
            });
        }




        [HttpPost]
        public async Task<JsonResult> UpdateEmployee([FromForm] Employee employee)
        {
            try
            {
                if (HttpContext.Session.GetInt32("UserId") == null)
                    return Json(new { success = false, message = "Session expired. Please login again." });

                var existing = _employeeRepository.GetById(employee.Id);
                if (existing == null)
                    return Json(new { success = false, message = "Employee not found." });

                // 🧠 Role change check for manager
                if (existing.Role.ToLower() == "manager" &&
                    employee.Role.ToLower() != "manager")
                {
                    var subordinates = _employeeRepository.GetEmployeesByManagerId(existing.Id);
                    if (subordinates.Any())
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Cannot change role. This manager has employees assigned."
                        });
                    }
                }

                // 🔐 Encrypt password if changed
                if (!string.IsNullOrWhiteSpace(employee.Password) && employee.Password != existing.Password)
                {
                    employee.Password = EncryptionHelper.Encrypt(employee.Password);
                }
                else
                {
                    employee.Password = existing.Password;
                }

                // 📷 Handle image update if a new file was uploaded
                var file = employee.ImageFile;
                if (file != null && file.Length > 0)
                {
                    if (!string.IsNullOrEmpty(existing.ImagePath))
                    {
                        var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existing.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }

                    var fileName = employee.Id + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/image", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    employee.ImagePath = "/image/" + fileName;
                }
                else
                {
                    employee.ImagePath = existing.ImagePath;
                }

                var success = _employeeRepository.Update(employee); // ✅ NOW returns bool

                if (!success)
                    return Json(new { success = false, message = "Update failed. Try again." });

                return Json(new { success = true, message = "Employee updated successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteEmployee(int id)
        {
            try
            {
                if (HttpContext.Session.GetInt32("UserId") == null)
                {
                    return Json(new { success = false, message = "Session expired. Please login again." });
                }

                var employee = _employeeRepository.GetById(id);
                if (employee == null)
                    return Json(new { success = false, message = "Employee not found" });

                var success = _employeeRepository.Delete(id); // ✅ NOW returns bool

                if (!success)
                    return Json(new { success = false, message = "Delete failed. Try again." });

                return Json(new { success = true, message = "Employee deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpGet]
        public JsonResult SearchEmployees(string search)
        {
            var result = _employeeRepository.SearchEmployees(search);
            return Json(new { success = true, data = result });
        }

        public IActionResult Team()
        {
            return View(); 
        }

        [HttpGet]
        public JsonResult GetTeam()
        {
            int? managerId = HttpContext.Session.GetInt32("UserId");
            if (managerId == null)
                return Json(new { success = false, message = "Not logged in" });

            var team = _employeeRepository.GetEmployeesByManagerId(managerId.Value);
            return Json(new { success = true, data = team });
        }

        [HttpGet]
        public JsonResult GetTeamAttendance()
        {
            int? managerId = HttpContext.Session.GetInt32("UserId");
            if (managerId == null)
                return Json(new { success = false, message = "Not logged in" });

            var manager = _employeeRepository.GetById(managerId.Value);

            var managerAttendance = _attendanceRepository.GetByEmployeeId(managerId.Value);

            var teamAttendance = _employeeRepository.GetAttendanceWithNameByManagerId(managerId.Value);

            return Json(new
            {
                success = true,
                manager,
                managerAttendance,
                teamAttendance
            });
        }

        [HttpGet]
        public JsonResult SearchEmployeesUnderManager(string? search)
        {
            int? managerId = HttpContext.Session.GetInt32("UserId");
            if (managerId == null)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var result = _employeeRepository.SearchEmployeesUnderManager(managerId.Value, search);
            return Json(new { success = true, data = result });
        }



        public IActionResult Profile()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetById(int id)
        {
            try
            {
                var emp = _employeeRepository.GetById(id);

                if (emp == null)
                    return new JsonResult(new { success = false, message = "Employee not found." });

                return new JsonResult(new { success = true, data = emp });
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }




    }
}
