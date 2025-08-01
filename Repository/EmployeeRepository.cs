using Dapper;
using final.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
namespace final.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly SqlConnection _connection;

        public EmployeeRepository(SqlConnection connection)
        {
            _connection = connection;
        }

        public Employee GetByEmail(string email)
        {
            string sql = "SELECT * FROM Employees WHERE Email = @Email;";

            return _connection.QueryFirstOrDefault<Employee>(sql, new { Email = email });

        }

        public Employee GetById(int id)
        {
            string sql = "SELECT * FROM Employees WHERE Id = @Id";
            return _connection.QueryFirstOrDefault<Employee>(sql, new { Id = id });
        }

        public async Task<bool> Add(Employee employee)
        {
            var sql = @"INSERT INTO Employees (Name, Email, Password, Role, ManagerId, Position, Address, Salary, ImagePath)
                    VALUES (@Name, @Email, @Password, @Role, @ManagerId, @Position, @Address, @Salary, @ImagePath)";

                try
                {
                    var rows = await _connection.ExecuteAsync(sql, employee);
                    return rows > 0;
                }
                catch (Exception ex)
                {
                    // Log error if needed
                    return false;
                }
            
        }


        public bool Update(Employee employee)
        {
            string sql = @"UPDATE Employees 
                   SET Name = @Name, 
                       Email = @Email, 
                       Password = @Password, 
                       Role = @Role, 
                       ManagerId = @ManagerId,
                       Position = @Position,
                       Address = @Address,
                       Salary = @Salary,
                       ImagePath = @ImagePath
                   WHERE Id = @Id";

            int rowsAffected = _connection.Execute(sql, employee);
            return rowsAffected > 0;
        }

        public bool Delete(int id)
        {
            string sql = "DELETE FROM Employees WHERE Id = @Id";
            int rowsAffected = _connection.Execute(sql, new { Id = id });
            return rowsAffected > 0;
        }


        public List<Employee> GetAll()
        {
            string sql = "SELECT * FROM Employees";
            return _connection.Query<Employee>(sql).ToList();
        }

        public List<Employee> SearchEmployees(string? search = null)
        {
            string sql = @"
                SELECT * FROM Employees
                WHERE @Search IS NULL 
                   OR Name LIKE @Search 
                   OR Email LIKE @Search
                   OR Role LIKE @Search";

            return _connection.Query<Employee>(
                sql, new { Search = $"%{search}%" }).AsList();
        }

        public List<Employee> GetEmployeesByManagerId(int managerId)
        {
            var sql = "SELECT * FROM Employees WHERE ManagerId = @ManagerId";
            return _connection.Query<Employee>(sql, new { ManagerId = managerId }).ToList();
        }

        public List<EmployeeAttendanceViewModel> GetAttendanceWithNameByManagerId(int managerId)
        {
            string sql = @"
                        SELECT e.Name AS EmployeeName, a.Date, a.AttendanceType
                        FROM Employees e
                        INNER JOIN Attendance a ON e.Id = a.EmployeeId
                        WHERE e.ManagerId = @ManagerId";

            return _connection.Query<EmployeeAttendanceViewModel>(sql, new { ManagerId = managerId }).ToList();
        }

        public List<Employee> SearchEmployeesUnderManager(int managerId, string? search = null)
        {
            string sql = @"
        SELECT * FROM Employees
        WHERE ManagerId = @ManagerId AND
              (@Search IS NULL 
               OR Name LIKE '%' + @Search + '%' 
               OR Email LIKE '%' + @Search + '%' 
               OR Role LIKE '%' + @Search + '%')";

            return _connection.Query<Employee>(sql, new { ManagerId = managerId, Search = search }).AsList();
        }

    }
}
