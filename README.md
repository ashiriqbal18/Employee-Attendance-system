**Employee Attendance Management System**

This is a web-based Employee Attendance Management System built using ASP.NET Core MVC, Dapper, and SQL Server. 

It provides role-based access for Admins, Managers, and Employees to manage and monitor employee data and attendance records.

 1) Features
    
  .) Authentication & Authorization
     - Session-based login system
     - Role-based access: Admin, Manager, Employee
     - Secure password encryption using AES
  .) Employee Management
     - CRUD operations for employees
     -  Assign managers to employees
     -  View profile and position info
     -  Upload and display employee images
     
  .) Attendance Management
     - Mark attendance with type (Present, Absent, Leave)
     - Filter by date, employee, and attendance type
     - View attendance summary
    
  .) Manager Access
     - View own profile and attendance
     - View team members' information and attendance
     
  .) Admin Access
     - View all employees
     - View all attendance records
     - Assign Manager to employees
     - Manage employee records
     
 2) Tech Stack
   - Backend: ASP.NET Core 8.0 MVC
   - Database: SQL Server, Dapper ORM
   - Frontend: Bootstrap, jQuery, DataTables
   - Authentication: Session-based with role checks
   - Encryption: AES (Advanced Encryption Standard)
