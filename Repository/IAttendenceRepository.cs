
using final.Models;
using System.Collections.Generic;

public interface IAttendanceRepository
{
    List<Attendance> GetAll();
    List<Attendance> GetByEmployeeId(int empId);
    void Add(Attendance attend);
    void Update(Attendance attend);
    List<EmployeeAttendanceViewModel> SearchAttendance(string? search = null);
    List<EmployeeAttendanceViewModel> GetAllEmployeeAttendance();
}
