using EmployeeManagement.Application.DTOs;

namespace EmployeeManagement.Application.Interfaces;

public interface IEmployeeService
{
    Task<EmployeeDto> GetByIdAsync(int employeeId);
    Task<PagedResultDto<EmployeeDto>> GetPagedAsync(EmployeeQueryParameters parameters);
    Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto, int currentUserId);
    Task<EmployeeDto> UpdateAsync(int employeeId, UpdateEmployeeDto dto, int currentUserId);
    Task DeleteAsync(int employeeId);
    Task<DashboardSummaryDto> GetDashboardSummaryAsync();
}
