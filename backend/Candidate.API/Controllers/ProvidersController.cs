using Candidate.API.Contracts;
using Candidate.API.Data;
using Candidate.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Candidate.API.Controllers;

[ApiController]
[Route("api/providers")]
public sealed class ProvidersController(CandidateDbContext database) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ProviderListItemResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<ProviderListItemResponse>>> GetProviders(
        [FromQuery] string? search,
        [FromQuery] string? scenario,
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var providers = database.Providers
            .AsNoTracking()
            .Include(provider => provider.Licenses)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.Trim();
            providers = providers.Where(provider =>
                provider.Name.Contains(searchTerm) ||
                provider.County.Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(scenario))
        {
            if (scenario == "active-with-active-licenses")
            {
                providers = providers.Where(provider =>
                    provider.Status == ProviderStatuses.Active &&
                    provider.Licenses.Any(license =>
                        license.Status == LicenseStatuses.Active &&
                        license.ExpirationDate >= today));
            }
            else if (scenario == "active-with-expired-licenses")
            {
                providers = providers.Where(provider =>
                    provider.Status == ProviderStatuses.Active &&
                    provider.Licenses.Any(license =>
                        license.Status == LicenseStatuses.Active &&
                        license.ExpirationDate < today));
            }
            else
            {
                ModelState.AddModelError(
                    nameof(scenario),
                    "Use active-with-active-licenses or active-with-expired-licenses.");
                return ValidationProblem(ModelState);
            }
        }

        var result = await providers
            .OrderBy(provider => provider.Name)
            .ToListAsync(cancellationToken);

        return Ok(result.Select(provider =>
            ProviderResponseMapper.ToListItem(provider, today)).ToList());
    }

    [HttpGet("deleted")]
    [ProducesResponseType<IReadOnlyList<ProviderDetailsResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProviderDetailsResponse>>> GetDeletedProviders(
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var deletedProviders = await database.Providers
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(provider => provider.Licenses)
            .Where(provider => provider.IsDeleted)
            .OrderBy(provider => provider.Name)
            .ToListAsync(cancellationToken);

        return Ok(deletedProviders.Select(provider =>
            ProviderResponseMapper.ToDetails(provider, today)).ToList());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<ProviderDetailsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProviderDetailsResponse>> GetProvider(
        Guid id,
        CancellationToken cancellationToken)
    {
        var provider = await database.Providers
            .AsNoTracking()
            .Include(item => item.Licenses)
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (provider is null)
        {
            return NotFound();
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return Ok(ProviderResponseMapper.ToDetails(provider, today));
    }

    [HttpPost]
    [ProducesResponseType<ProviderDetailsResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProviderDetailsResponse>> CreateProvider(
        ProviderRequest request,
        CancellationToken cancellationToken)
    {
        if (!NormalizeAndValidate(request))
        {
            return ValidationProblem(ModelState);
        }

        var now = DateTimeOffset.UtcNow;
        var provider = new Provider
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            County = request.County,
            Status = request.Status,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        database.Providers.Add(provider);
        await database.SaveChangesAsync(cancellationToken);

        var response = ProviderResponseMapper.ToDetails(
            provider,
            DateOnly.FromDateTime(DateTime.UtcNow));
        return CreatedAtAction(nameof(GetProvider), new { id = provider.Id }, response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<ProviderDetailsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProviderDetailsResponse>> UpdateProvider(
        Guid id,
        ProviderRequest request,
        CancellationToken cancellationToken)
    {
        if (!NormalizeAndValidate(request))
        {
            return ValidationProblem(ModelState);
        }

        var provider = await database.Providers
            .Include(item => item.Licenses)
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (provider is null)
        {
            return NotFound();
        }

        provider.Name = request.Name;
        provider.County = request.County;
        provider.Status = request.Status;
        provider.UpdatedAtUtc = DateTimeOffset.UtcNow;
        await database.SaveChangesAsync(cancellationToken);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return Ok(ProviderResponseMapper.ToDetails(provider, today));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SoftDeleteProvider(
        Guid id,
        CancellationToken cancellationToken)
    {
        var provider = await database.Providers
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (provider is null)
        {
            return NotFound();
        }

        var deletedAt = DateTimeOffset.UtcNow;
        provider.IsDeleted = true;
        provider.DeletedAtUtc = deletedAt;
        provider.UpdatedAtUtc = deletedAt;
        await database.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private bool NormalizeAndValidate(ProviderRequest request)
    {
        request.Normalize();
        ModelState.Clear();
        return TryValidateModel(request);
    }
}
