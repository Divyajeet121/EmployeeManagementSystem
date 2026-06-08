USE EmployeeManagementDb;
GO

IF OBJECT_ID(N'dbo.usp_Employee_Create', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_Employee_Create;
GO
CREATE PROCEDURE dbo.usp_Employee_Create
    @EmployeeCode NVARCHAR(20),
    @FirstName    NVARCHAR(100),
    @LastName     NVARCHAR(100),
    @Email        NVARCHAR(150),
    @Department   NVARCHAR(100),
    @Designation  NVARCHAR(100),
    @JoiningDate  DATETIME2,
    @IsActive     BIT,
    @CreatedBy    INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Employees WHERE EmployeeCode = @EmployeeCode OR Email = @Email)
    BEGIN
        RAISERROR('An employee with the same code or email already exists.', 16, 1);
        RETURN;
    END

    INSERT INTO dbo.Employees
        (EmployeeCode, FirstName, LastName, Email, Department, Designation, JoiningDate, IsActive, CreatedBy, CreatedOn)
    VALUES
        (@EmployeeCode, @FirstName, @LastName, @Email, @Department, @Designation, @JoiningDate, @IsActive, @CreatedBy, SYSUTCDATETIME());

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS EmployeeId;
END
GO

IF OBJECT_ID(N'dbo.usp_Employee_Update', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_Employee_Update;
GO
CREATE PROCEDURE dbo.usp_Employee_Update
    @EmployeeId   INT,
    @EmployeeCode NVARCHAR(20),
    @FirstName    NVARCHAR(100),
    @LastName     NVARCHAR(100),
    @Email        NVARCHAR(150),
    @Department   NVARCHAR(100),
    @Designation  NVARCHAR(100),
    @JoiningDate  DATETIME2,
    @IsActive     BIT,
    @ModifiedBy   INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Employees WHERE EmployeeId = @EmployeeId)
    BEGIN
        RAISERROR('Employee not found.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM dbo.Employees WHERE (EmployeeCode = @EmployeeCode OR Email = @Email) AND EmployeeId <> @EmployeeId)
    BEGIN
        RAISERROR('Another employee with the same code or email already exists.', 16, 1);
        RETURN;
    END

    UPDATE dbo.Employees
    SET EmployeeCode = @EmployeeCode,
        FirstName    = @FirstName,
        LastName     = @LastName,
        Email        = @Email,
        Department   = @Department,
        Designation  = @Designation,
        JoiningDate  = @JoiningDate,
        IsActive     = @IsActive,
        ModifiedBy   = @ModifiedBy,
        ModifiedOn   = SYSUTCDATETIME()
    WHERE EmployeeId = @EmployeeId;
END
GO

IF OBJECT_ID(N'dbo.usp_Employee_Delete', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_Employee_Delete;
GO
CREATE PROCEDURE dbo.usp_Employee_Delete
    @EmployeeId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Employees WHERE EmployeeId = @EmployeeId)
    BEGIN
        RAISERROR('Employee not found.', 16, 1);
        RETURN;
    END

    DELETE FROM dbo.Employees WHERE EmployeeId = @EmployeeId;
END
GO

IF OBJECT_ID(N'dbo.usp_Employee_GetById', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_Employee_GetById;
GO
CREATE PROCEDURE dbo.usp_Employee_GetById
    @EmployeeId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT EmployeeId, EmployeeCode, FirstName, LastName, Email, Department, Designation,
           JoiningDate, IsActive, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn
    FROM dbo.Employees
    WHERE EmployeeId = @EmployeeId;
END
GO
