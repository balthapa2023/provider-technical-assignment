using Candidate.API.Contracts;
using Candidate.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Candidate.API.Controllers;

[ApiController]
[Route("api/dashboard")]
public sealed class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    /// <summary>Returns registry-wide provider, license health, and upcoming renewal metrics.</summary>
    [HttpGet]
    [ProducesResponseType<DashboardResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardResponse>> GetDashboard(
        CancellationToken cancellationToken)
    {
        return Ok(await dashboardService.GetAsync(cancellationToken));
    }
}
