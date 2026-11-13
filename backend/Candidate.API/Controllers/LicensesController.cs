using Candidate.API.Contracts;
using Candidate.API.Data;
using Candidate.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Candidate.API.Controllers;

[ApiController]
[Route("api/providers/{providerId:guid}/licenses")]
public sealed class LicensesController(CandidateDbContext database) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<LicenseResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<LicenseResponse>>> GetLicenses(
        Guid providerId,
        CancellationToken cancellationToken)
    {
        var providerExists = await database.Providers
            .AnyAsync(provider => provider.Id == providerId, cancellationToken);

        if (!providerExists)
        {
            return NotFound();
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var licenses = await database.Licenses
            .AsNoTracking()
            .Where(license => license.ProviderId == providerId)
            .OrderBy(license => license.ExpirationDate)
            .ToListAsync(cancellationToken);

        return Ok(licenses.Select(license => ToResponse(license, today)).ToList());
    }

    [HttpGet("deleted")]
    [ProducesResponseType<IReadOnlyList<DeletedLicenseResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<DeletedLicenseResponse>>> GetDeletedLicenses(
        Guid providerId,
        CancellationToken cancellationToken)
    {
        var providerExists = await database.Providers
            .AnyAsync(provider => provider.Id == providerId, cancellationToken);

        if (!providerExists)
        {
            return NotFound();
        }

        var deletedLicenses = await database.Licenses
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(license => license.ProviderId == providerId && license.IsDeleted)
            .OrderBy(license => license.Number)
            .Select(license => new DeletedLicenseResponse(
                license.Id,
                license.ProviderId,
                license.Number,
                license.Status,
                license.ExpirationDate,
                license.CreatedAtUtc,
                license.UpdatedAtUtc,
                license.DeletedAtUtc))
            .ToListAsync(cancellationToken);

        return Ok(deletedLicenses);
    }

    [HttpPost]
    [ProducesResponseType<LicenseResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LicenseResponse>> CreateLicense(
        Guid providerId,
        LicenseRequest request,
        CancellationToken cancellationToken)
    {
        if (!NormalizeAndValidate(request))
        {
            return ValidationProblem(ModelState);
        }

        if (request.ExpirationDate is not DateOnly expirationDate)
        {
            ModelState.AddModelError(
                nameof(request.ExpirationDate),
                "Expiration date is required.");
            return ValidationProblem(ModelState);
        }

        var provider = await database.Providers
            .SingleOrDefaultAsync(item => item.Id == providerId, cancellationToken);

        if (provider is null)
        {
            return NotFound();
        }

        var numberInUse = await database.Licenses
            .AnyAsync(license =>
                license.ProviderId == providerId &&
                license.Number == request.Number,
                cancellationToken);

        if (numberInUse)
        {
            return Conflict(new ProblemDetails
            {
                Title = "License number already exists",
                Detail = "A non-deleted license already uses this number for the provider.",
                Status = StatusCodes.Status409Conflict
            });
        }

        var now = DateTimeOffset.UtcNow;
        var license = new License
        {
            Id = Guid.NewGuid(),
            ProviderId = providerId,
            Number = request.Number,
            Status = request.Status,
            ExpirationDate = expirationDate,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        database.Licenses.Add(license);
        await database.SaveChangesAsync(cancellationToken);

        var response = ToResponse(license, DateOnly.FromDateTime(DateTime.UtcNow));
        return CreatedAtAction(
            nameof(GetLicenses),
            new { providerId },
            response);
    }

    [HttpPut("{licenseId:guid}")]
    [ProducesResponseType<LicenseResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LicenseResponse>> UpdateLicense(
        Guid providerId,
        Guid licenseId,
        LicenseRequest request,
        CancellationToken cancellationToken)
    {
        if (!NormalizeAndValidate(request))
        {
            return ValidationProblem(ModelState);
        }

        if (request.ExpirationDate is not DateOnly expirationDate)
        {
            ModelState.AddModelError(
                nameof(request.ExpirationDate),
                "Expiration date is required.");
            return ValidationProblem(ModelState);
        }

        var license = await database.Licenses
            .SingleOrDefaultAsync(item =>
                item.Id == licenseId &&
                item.ProviderId == providerId,
                cancellationToken);

        if (license is null)
        {
            return NotFound();
        }

        var numberInUse = await database.Licenses
            .AnyAsync(item =>
                item.ProviderId == providerId &&
                item.Id != licenseId &&
                item.Number == request.Number,
                cancellationToken);

        if (numberInUse)
        {
            return Conflict(new ProblemDetails
            {
                Title = "License number already exists",
                Detail = "A non-deleted license already uses this number for the provider.",
                Status = StatusCodes.Status409Conflict
            });
        }

        license.Number = request.Number;
        license.Status = request.Status;
        license.ExpirationDate = expirationDate;
        license.UpdatedAtUtc = DateTimeOffset.UtcNow;
        await database.SaveChangesAsync(cancellationToken);

        var response = ToResponse(license, DateOnly.FromDateTime(DateTime.UtcNow));
        return Ok(response);
    }

    [HttpDelete("{licenseId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SoftDeleteLicense(
        Guid providerId,
        Guid licenseId,
        CancellationToken cancellationToken)
    {
        var license = await database.Licenses
            .SingleOrDefaultAsync(item =>
                item.Id == licenseId &&
                item.ProviderId == providerId,
                cancellationToken);

        if (license is null)
        {
            return NotFound();
        }

        var deletedAt = DateTimeOffset.UtcNow;
        license.IsDeleted = true;
        license.DeletedAtUtc = deletedAt;
        license.UpdatedAtUtc = deletedAt;
        await database.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private bool NormalizeAndValidate(LicenseRequest request)
    {
        request.Normalize();
        ModelState.Clear();
        return TryValidateModel(request);
    }

    private static LicenseResponse ToResponse(License license, DateOnly today)
    {
        return new LicenseResponse(
            license.Id,
            license.Number,
            ProviderResponseMapper.GetEffectiveLicenseStatus(license, today),
            license.Status,
            license.ExpirationDate,
            license.UpdatedAtUtc,
            license.IsDeleted,
            license.DeletedAtUtc);
    }
}
