using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProviderAssignmentStarter.Data;
using ProviderAssignmentStarter.Models;

namespace ProviderAssignmentStarter.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProvidersController : ControllerBase
{
    private readonly AppDbContext _db;
    public ProvidersController(AppDbContext db) => _db = db;

    private static DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);

    // GET /api/providers?status=Active&county=Fulton&search=sun
    // Soft-deleted providers are excluded automatically by the global query filter.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProviderDto>>> List(
        [FromQuery] string? status, [FromQuery] string? county, [FromQuery] string? search)
    {
        var q = _db.Providers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<ProviderStatus>(status, true, out var s))
                return BadRequest(Problem("Invalid status filter."));
            q = q.Where(p => p.Status == s);
        }
        if (!string.IsNullOrWhiteSpace(county)) q = q.Where(p => p.County == county);
        if (!string.IsNullOrWhiteSpace(search)) q = q.Where(p => EF.Functions.Like(p.ProviderName, $"%{search}%"));

        var today = Today;
        var result = await q.OrderBy(p => p.ProviderName)
            .Select(p => new ProviderDto(
                p.ProviderId, p.ProviderName, p.County, p.Status.ToString(),
                p.CreatedDate, p.ModifiedDate,
                p.Licenses.Count,
                p.Licenses.Count(l => l.ExpirationDate >= today),
                p.Licenses.Count(l => l.ExpirationDate < today)))
            .ToListAsync();

        return Ok(result);
    }

    // GET /api/providers/5  (provider + its licenses)
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProviderDetailDto>> Get(int id)
    {
        var p = await _db.Providers.AsNoTracking()
            .Include(x => x.Licenses)
            .FirstOrDefaultAsync(x => x.ProviderId == id);

        return p is null ? NotFound() : Ok(ToDetail(p));
    }

    // POST /api/providers
    [HttpPost]
    public async Task<ActionResult<ProviderDetailDto>> Create(ProviderUpsertDto dto)
    {
        var status = Enum.Parse<ProviderStatus>(dto.Status, true);

        // Business rule: no duplicate active provider name within the same county.
        if (await _db.Providers.AnyAsync(p => p.ProviderName == dto.ProviderName.Trim() && p.County == dto.County.Trim()))
            return Conflict(Problem($"A provider named '{dto.ProviderName}' already exists in {dto.County}."));

        var p = new Provider { ProviderName = dto.ProviderName.Trim(), County = dto.County.Trim(), Status = status };
        _db.Providers.Add(p);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = p.ProviderId }, ToDetail(p));
    }

    // PUT /api/providers/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProviderDetailDto>> Update(int id, ProviderUpsertDto dto)
    {
        var p = await _db.Providers.Include(x => x.Licenses).FirstOrDefaultAsync(x => x.ProviderId == id);
        if (p is null) return NotFound(); // includes soft-deleted — they can't be edited

        if (await _db.Providers.AnyAsync(x => x.ProviderId != id && x.ProviderName == dto.ProviderName.Trim() && x.County == dto.County.Trim()))
            return Conflict(Problem($"A provider named '{dto.ProviderName}' already exists in {dto.County}."));

        p.ProviderName = dto.ProviderName.Trim();
        p.County = dto.County.Trim();
        p.Status = Enum.Parse<ProviderStatus>(dto.Status, true);
        await _db.SaveChangesAsync();

        return Ok(ToDetail(p));
    }

    // DELETE /api/providers/5  -> SOFT DELETE ONLY
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> SoftDelete(int id)
    {
        var p = await _db.Providers.FirstOrDefaultAsync(x => x.ProviderId == id);
        if (p is null) return NotFound();

        p.IsDeleted = true;
        p.DeletedDate = DateTime.UtcNow;
        p.DeletedBy = User.Identity?.Name ?? "api-user"; // no auth in scope; placeholder for audit
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // ---------- AUDIT / TROUBLESHOOTING (explicitly opts out of the filter) ----------

    // GET /api/providers/audit/deleted
    [HttpGet("audit/deleted")]
    public async Task<ActionResult<IEnumerable<ProviderDetailDto>>> Deleted()
    {
        var list = await _db.Providers.IgnoreQueryFilters().AsNoTracking()
            .Include(p => p.Licenses)
            .Where(p => p.IsDeleted)
            .OrderByDescending(p => p.DeletedDate)
            .ToListAsync();
        return Ok(list.Select(ToDetail));
    }

    // POST /api/providers/5/restore
    [HttpPost("{id:int}/restore")]
    public async Task<ActionResult<ProviderDetailDto>> Restore(int id)
    {
        var p = await _db.Providers.IgnoreQueryFilters().Include(x => x.Licenses)
            .FirstOrDefaultAsync(x => x.ProviderId == id && x.IsDeleted);
        if (p is null) return NotFound();

        p.IsDeleted = false; p.DeletedDate = null; p.DeletedBy = null;
        await _db.SaveChangesAsync();
        return Ok(ToDetail(p));
    }

    // ---------- helpers ----------
    private static ProviderDetailDto ToDetail(Provider p) => new(
        p.ProviderId, p.ProviderName, p.County, p.Status.ToString(),
        p.CreatedDate, p.ModifiedDate, p.IsDeleted, p.DeletedDate, p.DeletedBy,
        p.Licenses.OrderBy(l => l.ExpirationDate).Select(l => LicensesController.ToDto(l, Today)));

    private static ProblemDetails Problem(string detail) => new() { Title = "Business rule violation", Detail = detail };
}
