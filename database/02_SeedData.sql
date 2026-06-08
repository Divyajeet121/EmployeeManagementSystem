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
