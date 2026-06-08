USE EmployeeManagementDb;
GO

-- Default credentials (change after first login):
--   admin / Admin@123  (Role: Admin)
--   john.user / User@123 (Role: User)
-- Password hashes below are generated with ASP.NET Core Identity's PasswordHasher<T> (PBKDF2/HMAC-SHA256).

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = 'admin')
BEGIN
    INSERT INTO dbo.Users (Username, PasswordHash, Role, IsActive)
    VALUES ('admin', 'AQAAAAIAAYagAAAAEHitgFQx32QUto+3+0loKvlpN62dV/CO7jrWjVpNQG/l75n+zbfh5hk0r2GGs9dMgg==', 'Admin', 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = 'john.user')
BEGIN
    INSERT INTO dbo.Users (Username, PasswordHash, Role, IsActive)
    VALUES ('john.user', 'AQAAAAIAAYagAAAAECsfdHBf+1Arxx0BxK/XmjkWqxwAK3GKi5TEEI5u2ZXgVJfl00ggFCfyYMniLI+pew==', 'User', 1);
END
GO

DECLARE @AdminId INT = (SELECT UserId FROM dbo.Users WHERE Username = 'admin');

IF NOT EXISTS (SELECT 1 FROM dbo.Employees WHERE EmployeeCode = 'EMP-1001')
BEGIN
    INSERT INTO dbo.Employees (EmployeeCode, FirstName, LastName, Email, Department, Designation, JoiningDate, IsActive, CreatedBy, CreatedOn)
    VALUES
    ('EMP-1001', 'Aditi',  'Sharma', 'aditi.sharma@example.com',  'Engineering', 'Software Engineer',     '2022-03-14', 1, @AdminId, SYSUTCDATETIME()),
    ('EMP-1002', 'Rohan',  'Mehta',  'rohan.mehta@example.com',   'Engineering', 'Senior Software Engineer', '2021-07-01', 1, @AdminId, SYSUTCDATETIME()),
    ('EMP-1003', 'Neha',   'Verma',  'neha.verma@example.com',    'Human Resources', 'HR Executive',      '2023-01-10', 1, @AdminId, SYSUTCDATETIME()),
    ('EMP-1004', 'Karan',  'Gupta',  'karan.gupta@example.com',   'Finance',     'Accountant',            '2020-11-23', 0, @AdminId, SYSUTCDATETIME()),
    ('EMP-1005', 'Priya',  'Nair',   'priya.nair@example.com',    'Engineering', 'QA Engineer',           '2022-09-05', 1, @AdminId, SYSUTCDATETIME());
END
GO
