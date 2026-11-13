using Candidate.API.Contracts;
using Candidate.API.Data;
using Candidate.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Candidate.API.Services;

public sealed class DashboardService(CandidateDbContext database, TimeProvider timeProvider)
    : IDashboardService
{
    public async Task<DashboardResponse> GetAsync(CancellationToken cancellationToken)
    {
        const int expiringWithinDays = 30;
        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        var cutoff = today.AddDays(expiringWithinDays);
        var providers = await database.Providers
            .AsNoTracking()
            .Include(provider => provider.Licenses)
            .OrderBy(provider => provider.Name)
            .ThenBy(provider => provider.Id)
            .ToListAsync(cancellationToken);

        var licenses = providers.SelectMany(provider => provider.Licenses).ToList();
        var providerStatuses = new[]
        {
            ProviderStatuses.Active, ProviderStatuses.Inactive, ProviderStatuses.Pending
        };
        var licenseStatuses = new[]
        {
            LicenseStatuses.Active, LicenseStatuses.Expired, LicenseStatuses.Suspended
        };
        var licensesByStatus = licenseStatuses.Select(status => new StatusCountResponse(
            status,
            licenses.Count(license =>
                ProviderResponseMapper.GetEffectiveLicenseStatus(license, today) == status)))
            .ToList();

        var expiringProviders = providers.Select(provider =>
        {
            var expiringLicenses = provider.Licenses.Where(license =>
                ProviderResponseMapper.GetEffectiveLicenseStatus(license, today) == LicenseStatuses.Active &&
                license.ExpirationDate <= cutoff).ToList();

            return expiringLicenses.Count == 0
                ? null
                : new ExpiringProviderResponse(
                    provider.Id,
                    provider.Name,
                    provider.County,
                    expiringLicenses.Count,
                    expiringLicenses.Min(license => license.ExpirationDate));
        })
            .OfType<ExpiringProviderResponse>()
            .OrderBy(provider => provider.NextExpirationDate)
            .ThenBy(provider => provider.ProviderName)
            .ThenBy(provider => provider.ProviderId)
            .ToList();

        return new DashboardResponse(
            today,
            expiringWithinDays,
            providers.Count,
            licenses.Count,
            licensesByStatus.Single(item => item.Status == LicenseStatuses.Expired).Count,
            expiringProviders.Count,
            providerStatuses.Select(status => new StatusCountResponse(
                status, providers.Count(provider => provider.Status == status))).ToList(),
            licensesByStatus,
            providers.OrderByDescending(provider => provider.Licenses.Count)
                .ThenBy(provider => provider.Name)
                .ThenBy(provider => provider.Id)
                .Select(provider => new ProviderLicenseCountResponse(
                    provider.Id, provider.Name, provider.Licenses.Count)).ToList(),
            expiringProviders);
    }
}
