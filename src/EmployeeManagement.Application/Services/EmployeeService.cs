using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Exceptions;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<EmployeeDto> GetByIdAsync(int employeeId)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId)
            ?? throw new NotFoundException($"Employee with id {employeeId} was not found.");

        return MapToDto(employee);
    }

    public async Task<PagedResultDto<EmployeeDto>> GetPagedAsync(EmployeeQueryParameters parameters)
    {
        var (items, totalCount) = await _employeeRepository.GetPagedAsync(parameters);

        return new PagedResultDto<EmployeeDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto, int currentUserId)
    {
        if (await _employeeRepository.ExistsByCodeOrEmailAsync(dto.EmployeeCode, dto.Email))
        {
            throw new ValidationException("An employee with the same code or email already exists.");
        }

        var employee = new Employee
        {
            EmployeeCode = dto.EmployeeCode.Trim(),
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.Trim(),
            Department = dto.Department.Trim(),
            Designation = dto.Designation.Trim(),
            JoiningDate = dto.JoiningDate,
            IsActive = dto.IsActive,
            CreatedBy = currentUserId,
            CreatedOn = DateTime.UtcNow
        };

        await _employeeRepository.AddAsync(employee);

        return MapToDto(employee);
    }

    public async Task<EmployeeDto> UpdateAsync(int employeeId, UpdateEmployeeDto dto, int currentUserId)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId)
            ?? throw new NotFoundException($"Employee with id {employeeId} was not found.");

        if (await _employeeRepository.ExistsByCodeOrEmailAsync(dto.EmployeeCode, dto.Email, employeeId))
        {
            throw new ValidationException("Another employee with the same code or email already exists.");
        }

        employee.EmployeeCode = dto.EmployeeCode.Trim();
        employee.FirstName = dto.FirstName.Trim();
        employee.LastName = dto.LastName.Trim();
        employee.Email = dto.Email.Trim();
        employee.Department = dto.Department.Trim();
        employee.Designation = dto.Designation.Trim();
        employee.JoiningDate = dto.JoiningDate;
        employee.IsActive = dto.IsActive;
        employee.ModifiedBy = currentUserId;
        employee.ModifiedOn = DateTime.UtcNow;

        await _employeeRepository.UpdateAsync(employee);

        return MapToDto(employee);
    }

    public async Task DeleteAsync(int employeeId)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId)
            ?? throw new NotFoundException($"Employee with id {employeeId} was not found.");

        await _employeeRepository.DeleteAsync(employee.EmployeeId);
    }

    public Task<DashboardSummaryDto> GetDashboardSummaryAsync()
    {
        return _employeeRepository.GetDashboardSummaryAsync();
    }

    private static EmployeeDto MapToDto(Employee employee)
    {
        return new EmployeeDto
        {
            EmployeeId = employee.EmployeeId,
            EmployeeCode = employee.EmployeeCode,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Department = employee.Department,
            Designation = employee.Designation,
            JoiningDate = employee.JoiningDate,
            IsActive = employee.IsActive,
            CreatedBy = employee.CreatedBy.ToString(),
            CreatedOn = employee.CreatedOn,
            ModifiedBy = employee.ModifiedBy?.ToString(),
            ModifiedOn = employee.ModifiedOn
        };
    }
}
