using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProviderAssignmentStarter.Data;
using ProviderAssignmentStarter.Models;

namespace ProviderAssignmentStarter.Controllers;

[ApiController]
[Route("api/providers/{providerId:int}/licenses")]
public class LicensesController : ControllerBase
{
    private readonly AppDbContext _db;
    public LicensesController(AppDbContext db) => _db = db;

    private static DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LicenseDto>>> List(int providerId)
    {
        if (!await _db.Providers.AnyAsync(p => p.ProviderId == providerId)) return NotFound();

        var today = Today;
        var list = await _db.Licenses.AsNoTracking()
            .Where(l => l.ProviderId == providerId)
            .OrderBy(l => l.ExpirationDate)
            .ToListAsync();

        return Ok(list.Select(l => ToDto(l, today)));
    }

    [HttpPost]
    public async Task<ActionResult<LicenseDto>> Create(int providerId, LicenseUpsertDto dto)
    {
        if (!await _db.Providers.AnyAsync(p => p.ProviderId == providerId))
            return NotFound(new ProblemDetails { Title = "Provider not found or deleted." });

        var number = dto.LicenseNumber.Trim().ToUpperInvariant();
        if (await _db.Licenses.AnyAsync(l => l.ProviderId == providerId && l.LicenseNumber == number))
            return Conflict(new ProblemDetails { Title = "Duplicate license", Detail = $"License {number} already exists for this provider." });

        var l = new License
        {
            ProviderId = providerId,
            LicenseNumber = number,
            LicenseStatus = Enum.Parse<LicenseStatus>(dto.LicenseStatus, true),
            ExpirationDate = dto.ExpirationDate
        };
        _db.Licenses.Add(l);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(List), new { providerId }, ToDto(l, Today));
    }

    [HttpPut("{licenseId:int}")]
    public async Task<ActionResult<LicenseDto>> Update(int providerId, int licenseId, LicenseUpsertDto dto)
    {
        var l = await _db.Licenses.FirstOrDefaultAsync(x => x.LicenseId == licenseId && x.ProviderId == providerId);
        if (l is null) return NotFound();

        var number = dto.LicenseNumber.Trim().ToUpperInvariant();
        if (await _db.Licenses.AnyAsync(x => x.ProviderId == providerId && x.LicenseId != licenseId && x.LicenseNumber == number))
            return Conflict(new ProblemDetails { Title = "Duplicate license", Detail = $"License {number} already exists for this provider." });

        l.LicenseNumber = number;
        l.LicenseStatus = Enum.Parse<LicenseStatus>(dto.LicenseStatus, true);
        l.ExpirationDate = dto.ExpirationDate;
        await _db.SaveChangesAsync();

        return Ok(ToDto(l, Today));
    }

    public static LicenseDto ToDto(License l, DateOnly today) => new(
        l.LicenseId, l.ProviderId, l.LicenseNumber, l.LicenseStatus.ToString(),
        l.ExpirationDate, l.IsExpired(today), l.ExpirationDate.DayNumber - today.DayNumber);
}
