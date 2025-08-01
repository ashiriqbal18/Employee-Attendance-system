using final.Models;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Collections.Generic;
namespace final.Repository
{

    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly SqlConnection _connection;

        public AttendanceRepository(SqlConnection connection)
        {
            _connection = connection;
        }

        public List<Attendance> GetAll()
        {
            string sql = @"
                SELECT A.*, E.Name AS EmployeeName
                FROM Attendance A
                JOIN Employees E ON A.EmployeeId = E.Id";

            return _connection.Query<Attendance>(sql).AsList();
        }

        public List<Attendance> GetByEmployeeId(int empId)
        {
            string sql = @"
                SELECT A.*, E.Name AS EmployeeName
                FROM Attendance A
                JOIN Employees E ON A.EmployeeId = E.Id
                WHERE A.EmployeeId = @empId";

            return _connection.Query<Attendance>(sql, new { empId }).AsList();
        }

        public void Add(Attendance att)
        {
            string sql = @"
                INSERT INTO Attendance (EmployeeId, Date, AttendanceType )
                VALUES (@EmployeeId, @Date, @AttendanceType )";

            _connection.Execute(sql, att);
        }

        public void Update(Attendance att)
        {
            string sql = @"
                UPDATE Attendance
                SET EmployeeId = @EmployeeId,
                    Date = @Date,
                    AttendanceType = @AttendanceType
                WHERE Id = @Id";

            _connection.Execute(sql, att);
        }
        public List<EmployeeAttendanceViewModel> SearchAttendance(string? search = null)
        {
            string sql = @"
        SELECT E.Name AS EmployeeName, A.Date, A.AttendanceType
        FROM Attendance A
        JOIN Employees E ON A.EmployeeId = E.Id
        WHERE @Search IS NULL
           OR E.Name LIKE @Search
           OR A.AttendanceType LIKE @Search
           OR CONVERT(VARCHAR, A.Date, 23) LIKE @Search";

            return _connection.Query<EmployeeAttendanceViewModel>(sql, new { Search = $"%{search}%" }).ToList();
        }

        public List<EmployeeAttendanceViewModel> GetAllEmployeeAttendance()
        {
            string query = @"
            SELECT 
                e.Name AS EmployeeName,
                a.Date,
                a.AttendanceType
            FROM 
                Attendance a
            INNER JOIN 
                Employees e ON a.EmployeeId = e.Id
            ORDER BY 
                a.Date DESC";

            var result = _connection.Query<EmployeeAttendanceViewModel>(query).ToList();
            return result;
        }

    }
}