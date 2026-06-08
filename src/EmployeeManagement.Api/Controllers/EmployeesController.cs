using System.Security.Claims;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<EmployeeDto>>> GetEmployees([FromQuery] EmployeeQueryParameters parameters)
    {
        var result = await _employeeService.GetPagedAsync(parameters);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
    {
        var employee = await _employeeService.GetByIdAsync(id);
        return Ok(employee);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EmployeeDto>> CreateEmployee([FromBody] CreateEmployeeDto dto)
    {
        var employee = await _employeeService.CreateAsync(dto, GetCurrentUserId());
        return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmployeeId }, employee);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EmployeeDto>> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto dto)
    {
        var employee = await _employeeService.UpdateAsync(id, dto, GetCurrentUserId());
        return Ok(employee);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        await _employeeService.DeleteAsync(id);
        return NoContent();
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.Parse(userIdClaim!);
    }
}
