using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Employee?> GetByIdAsync(int employeeId)
    {
        return _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
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
        await _context.Employees.AddAsync(employee);
    }

    public void Update(Employee employee)
    {
        _context.Employees.Update(employee);
    }

    public void Remove(Employee employee)
    {
        _context.Employees.Remove(employee);
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

    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
