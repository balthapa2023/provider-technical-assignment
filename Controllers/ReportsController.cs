using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProviderAssignmentStarter.Data;
using ProviderAssignmentStarter.Models;

namespace ProviderAssignmentStarter.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ReportsController(AppDbContext db) => _db = db;

    private static DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);

    [HttpGet("active-providers-active-licenses")]
    public Task<List<ProviderLicenseScenarioDto>> ActiveWithActive() =>
        ScenarioQuery(expired: false);
    [HttpGet("active-providers-expired-licenses")]
    public Task<List<ProviderLicenseScenarioDto>> ActiveWithExpired() =>
        ScenarioQuery(expired: true);

    private Task<List<ProviderLicenseScenarioDto>> ScenarioQuery(bool expired)
    {
        var today = Today;
        return _db.Licenses.AsNoTracking()
            .Where(l => l.Provider.Status == ProviderStatus.Active)
            .Where(l => expired ? l.ExpirationDate < today : l.ExpirationDate >= today)
            .OrderBy(l => l.Provider.ProviderName).ThenBy(l => l.ExpirationDate)
            .Select(l => new ProviderLicenseScenarioDto(
                l.ProviderId, l.Provider.ProviderName, l.Provider.County, l.Provider.Status.ToString(),
                l.LicenseId, l.LicenseNumber, l.LicenseStatus.ToString(), l.ExpirationDate, l.ExpirationDate < today))
            .ToListAsync();
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardDto>> Dashboard()
    {
        var today = Today;
        var in30 = today.AddDays(30);

        var providers = await _db.Providers.AsNoTracking().Select(p => new { p.ProviderId, p.ProviderName, p.Status }).ToListAsync();
        var licenses = await _db.Licenses.AsNoTracking().Select(l => new { l.ProviderId, l.ExpirationDate, l.Provider.ProviderName }).ToListAsync();
        var deletedCount = await _db.Providers.IgnoreQueryFilters().CountAsync(p => p.IsDeleted);

        var dto = new DashboardDto(
            TotalProviders: providers.Count,
            ActiveProviders: providers.Count(p => p.Status == ProviderStatus.Active),
            DeletedProviders: deletedCount,
            TotalLicenses: licenses.Count,
            ActiveLicenses: licenses.Count(l => l.ExpirationDate >= today),
            ExpiredLicenses: licenses.Count(l => l.ExpirationDate < today),
            ExpiringIn30Days: licenses.Count(l => l.ExpirationDate >= today && l.ExpirationDate <= in30),
            ProvidersByStatus: providers.GroupBy(p => p.Status.ToString()).Select(g => new KeyValuePair<string, int>(g.Key, g.Count())),
            LicensesPerProvider: providers.Select(p => new KeyValuePair<string, int>(p.ProviderName, licenses.Count(l => l.ProviderId == p.ProviderId))),
            ProvidersWithExpiredLicenses: licenses.Where(l => l.ExpirationDate < today)
                .GroupBy(l => l.ProviderName).Select(g => new KeyValuePair<string, int>(g.Key, g.Count())));

        return Ok(dto);
    }
}
