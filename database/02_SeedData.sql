USE EmployeeManagementDb;
GO

 Default credentials (change after first login):
  admin / Admin@123  (Role: Admin)
  Divyajeets/ User@123 (Role: User)


IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = 'admin')
BEGIN
    INSERT INTO dbo.Users (Username, Email, PasswordHash, Role, IsActive)
    VALUES ('admin', 'admin@ems.com', 'AQAAAAIAAYagAAAAEHitgFQx32QUto+3+0loKvlpN62dV/CO7jrWjVpNQG/l75n+zbfh5hk0r2GGs9dMgg==', 'Admin', 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = 'john.user')
BEGIN
    INSERT INTO dbo.Users (Username, Email, PasswordHash, Role, IsActive)
    VALUES ('Divyajeets', 'divyajeet@test.com', 'AQAAAAIAAYagAAAAECsfdHBf+1Arxx0BxK/XmjkWqxwAK3GKi5TEEI5u2ZXgVJfl00ggFCfyYMniLI+pew==', 'User', 1);
END
GO

DECLARE @AdminUserId INT = (SELECT TOP (1) UserId FROM dbo.Users WHERE Username = 'admin');

IF NOT EXISTS (SELECT 1 FROM dbo.Employees WHERE EmployeeCode = 'EMP-2001')
BEGIN
    INSERT INTO dbo.Employees (EmployeeCode, FirstName, LastName, Email, Department, Designation, JoiningDate, IsActive, CreatedBy, CreatedOn)
    VALUES ('EMP-2001', 'Aarav', 'Sharma', 'aarav.sharma@ems.com', 'Engineering', 'Software Engineer', '2024-02-12', 1, @AdminUserId, SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Employees WHERE EmployeeCode = 'EMP-2002')
BEGIN
    INSERT INTO dbo.Employees (EmployeeCode, FirstName, LastName, Email, Department, Designation, JoiningDate, IsActive, CreatedBy, CreatedOn)
    VALUES ('EMP-2002', 'Priya', 'Verma', 'priya.verma@ems.com', 'Human Resources', 'HR Executive', '2023-07-04', 1, @AdminUserId, SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Employees WHERE EmployeeCode = 'EMP-2003')
BEGIN
    INSERT INTO dbo.Employees (EmployeeCode, FirstName, LastName, Email, Department, Designation, JoiningDate, IsActive, CreatedBy, CreatedOn)
    VALUES ('EMP-2003', 'Rohan', 'Mehta', 'rohan.mehta@ems.com', 'Sales', 'Sales Manager', '2022-11-21', 1, @AdminUserId, SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Employees WHERE EmployeeCode = 'EMP-2004')
BEGIN
    INSERT INTO dbo.Employees (EmployeeCode, FirstName, LastName, Email, Department, Designation, JoiningDate, IsActive, CreatedBy, CreatedOn)
    VALUES ('EMP-2004', 'Kavya', 'Iyer', 'kavya.iyer@ems.com', 'Finance', 'Accountant', '2025-03-18', 1, @AdminUserId, SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Employees WHERE EmployeeCode = 'EMP-2005')
BEGIN
    INSERT INTO dbo.Employees (EmployeeCode, FirstName, LastName, Email, Department, Designation, JoiningDate, IsActive, CreatedBy, CreatedOn)
    VALUES ('EMP-2005', 'Vikram', 'Nair', 'vikram.nair@ems.com', 'Engineering', 'QA Engineer', '2021-09-09', 0, @AdminUserId, SYSUTCDATETIME());
END
GO
