using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public DashboardController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary()
    {
        var summary = await _employeeService.GetDashboardSummaryAsync();
        return Ok(summary);
    }
}
