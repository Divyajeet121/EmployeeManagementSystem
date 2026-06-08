IF DB_ID(N'EmployeeManagementDb') IS NULL
BEGIN
    CREATE DATABASE EmployeeManagementDb;
END
GO

USE EmployeeManagementDb;
GO

IF OBJECT_ID(N'dbo.Employees', N'U') IS NOT NULL DROP TABLE dbo.Employees;
IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL DROP TABLE dbo.Users;
GO

CREATE TABLE dbo.Users
(
    UserId       INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
    Username     NVARCHAR(50)      NOT NULL,
    Email        NVARCHAR(150)     NOT NULL,
    PasswordHash NVARCHAR(MAX)     NOT NULL,
    Role         NVARCHAR(20)      NOT NULL,
    IsActive     BIT               NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT (1),
    CONSTRAINT UQ_Users_Username UNIQUE (Username)
);
GO

CREATE TABLE dbo.Employees
(
    EmployeeId   INT IDENTITY(1,1)  NOT NULL CONSTRAINT PK_Employees PRIMARY KEY,
    EmployeeCode NVARCHAR(20)       NOT NULL,
    FirstName    NVARCHAR(100)      NOT NULL,
    LastName     NVARCHAR(100)      NOT NULL,
    Email        NVARCHAR(150)      NOT NULL,
    Department   NVARCHAR(100)      NOT NULL,
    Designation  NVARCHAR(100)      NOT NULL,
    JoiningDate  DATETIME2          NOT NULL,
    IsActive     BIT                NOT NULL CONSTRAINT DF_Employees_IsActive DEFAULT (1),
    CreatedBy    INT                NOT NULL,
    CreatedOn    DATETIME2          NOT NULL CONSTRAINT DF_Employees_CreatedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy   INT                NULL,
    ModifiedOn   DATETIME2          NULL,
    CONSTRAINT UQ_Employees_EmployeeCode UNIQUE (EmployeeCode),
    CONSTRAINT UQ_Employees_Email UNIQUE (Email),
    CONSTRAINT FK_Employees_CreatedBy_Users FOREIGN KEY (CreatedBy) REFERENCES dbo.Users (UserId),
    CONSTRAINT FK_Employees_ModifiedBy_Users FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users (UserId)
);
GO

CREATE NONCLUSTERED INDEX IX_Employees_Department ON dbo.Employees (Department);
CREATE NONCLUSTERED INDEX IX_Employees_IsActive ON dbo.Employees (IsActive);
GO
