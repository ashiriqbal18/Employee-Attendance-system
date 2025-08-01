using final.Models;
using System.Collections.Generic;
namespace final.Repository
{
    public interface IEmployeeRepository
    {
        Employee GetByEmail(string email);
        Employee GetById(int id);
        Task<bool> Add(Employee employee);

        public bool Update(Employee employee);
        public bool Delete(int id);
        List<Employee> GetAll();
        List<Employee> SearchEmployees(string? search = null);
        List<EmployeeAttendanceViewModel> GetAttendanceWithNameByManagerId(int managerId);
        List<Employee> GetEmployeesByManagerId(int managerId);
        List<Employee> SearchEmployeesUnderManager(int managerId, string? search = null);
    }
}
