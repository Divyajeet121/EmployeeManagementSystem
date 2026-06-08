using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Interfaces;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(int employeeId);
    Task<(List<Employee> Items, int TotalCount)> GetPagedAsync(EmployeeQueryParameters parameters);
    Task<bool> ExistsByCodeOrEmailAsync(string employeeCode, string email, int? excludeEmployeeId = null);
    Task AddAsync(Employee employee);
    void Update(Employee employee);
    void Remove(Employee employee);
    Task<DashboardSummaryDto> GetDashboardSummaryAsync();
    Task<int> SaveChangesAsync();
}
