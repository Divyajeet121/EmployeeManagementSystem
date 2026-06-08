using System.Data;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EmployeeManagement.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Employee?> GetByIdAsync(int employeeId)
    {
        var employees = await _context.Employees
            .FromSqlInterpolated($"EXEC dbo.usp_Employee_GetById @EmployeeId = {employeeId}")
            .ToListAsync();

        return employees.FirstOrDefault();
    }

    public async Task<(List<Employee> Items, int TotalCount)> GetPagedAsync(EmployeeQueryParameters parameters)
    {
        var query = _context.Employees.AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var term = parameters.SearchTerm.Trim();
            query = query.Where(e =>
                EF.Functions.Like(e.FirstName, $"%{term}%") ||
                EF.Functions.Like(e.LastName, $"%{term}%") ||
                EF.Functions.Like(e.EmployeeCode, $"%{term}%") ||
                EF.Functions.Like(e.Email, $"%{term}%"));
        }

        if (!string.IsNullOrWhiteSpace(parameters.Department))
        {
            query = query.Where(e => e.Department == parameters.Department);
        }

        if (parameters.IsActive.HasValue)
        {
            query = query.Where(e => e.IsActive == parameters.IsActive.Value);
        }

        var totalCount = await query.CountAsync();

        var pageNumber = parameters.PageNumber < 1 ? 1 : parameters.PageNumber;
        var pageSize = parameters.PageSize is < 1 or > 100 ? 10 : parameters.PageSize;

        var items = await query
            .OrderByDescending(e => e.CreatedOn)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<bool> ExistsByCodeOrEmailAsync(string employeeCode, string email, int? excludeEmployeeId = null)
    {
        var query = _context.Employees.Where(e => e.EmployeeCode == employeeCode || e.Email == email);

        if (excludeEmployeeId.HasValue)
        {
            query = query.Where(e => e.EmployeeId != excludeEmployeeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task AddAsync(Employee employee)
    {
        await using var command = CreateCommand("dbo.usp_Employee_Create");

        command.Parameters.Add(new SqlParameter("@EmployeeCode", employee.EmployeeCode));
        command.Parameters.Add(new SqlParameter("@FirstName", employee.FirstName));
        command.Parameters.Add(new SqlParameter("@LastName", employee.LastName));
        command.Parameters.Add(new SqlParameter("@Email", employee.Email));
        command.Parameters.Add(new SqlParameter("@Department", employee.Department));
        command.Parameters.Add(new SqlParameter("@Designation", employee.Designation));
        command.Parameters.Add(new SqlParameter("@JoiningDate", employee.JoiningDate));
        command.Parameters.Add(new SqlParameter("@IsActive", employee.IsActive));
        command.Parameters.Add(new SqlParameter("@CreatedBy", employee.CreatedBy));

        await OpenConnectionAsync();
        var newEmployeeId = await command.ExecuteScalarAsync();
        employee.EmployeeId = Convert.ToInt32(newEmployeeId);
    }

    public async Task UpdateAsync(Employee employee)
    {
        await using var command = CreateCommand("dbo.usp_Employee_Update");

        command.Parameters.Add(new SqlParameter("@EmployeeId", employee.EmployeeId));
        command.Parameters.Add(new SqlParameter("@EmployeeCode", employee.EmployeeCode));
        command.Parameters.Add(new SqlParameter("@FirstName", employee.FirstName));
        command.Parameters.Add(new SqlParameter("@LastName", employee.LastName));
        command.Parameters.Add(new SqlParameter("@Email", employee.Email));
        command.Parameters.Add(new SqlParameter("@Department", employee.Department));
        command.Parameters.Add(new SqlParameter("@Designation", employee.Designation));
        command.Parameters.Add(new SqlParameter("@JoiningDate", employee.JoiningDate));
        command.Parameters.Add(new SqlParameter("@IsActive", employee.IsActive));
        command.Parameters.Add(new SqlParameter("@ModifiedBy", employee.ModifiedBy ?? (object)DBNull.Value));

        await OpenConnectionAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int employeeId)
    {
        await using var command = CreateCommand("dbo.usp_Employee_Delete");

        command.Parameters.Add(new SqlParameter("@EmployeeId", employeeId));

        await OpenConnectionAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
    {
        var total = await _context.Employees.CountAsync();
        var active = await _context.Employees.CountAsync(e => e.IsActive);

        return new DashboardSummaryDto
        {
            TotalEmployees = total,
            ActiveEmployees = active,
            InactiveEmployees = total - active
        };
    }

    private SqlCommand CreateCommand(string procedureName)
    {
        var command = (SqlCommand)_context.Database.GetDbConnection().CreateCommand();
        command.CommandText = procedureName;
        command.CommandType = CommandType.StoredProcedure;

        if (_context.Database.CurrentTransaction is not null)
        {
            command.Transaction = (SqlTransaction)_context.Database.CurrentTransaction.GetDbTransaction();
        }

        return command;
    }

    private async Task OpenConnectionAsync()
    {
        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }
    }
}
