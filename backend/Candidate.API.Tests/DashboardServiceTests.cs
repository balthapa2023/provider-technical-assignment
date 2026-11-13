using Candidate.API.Contracts;
using Candidate.API.Data;
using Candidate.API.Models;
using Candidate.API.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Candidate.API.Tests;

public sealed class DashboardServiceTests
{
    private static readonly DateOnly Today = new(2026, 11, 13);

    [Fact]
    public async Task GetAsync_AggregatesCurrentRecordsAndInclusiveRenewalBoundaries()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var database = CreateDatabase(connection);
        await database.Database.EnsureCreatedAsync();

        var active = CreateProvider("Alpha", ProviderStatuses.Active);
        active.Licenses.AddRange([
            CreateLicense("expired", -1),
            CreateLicense("today", 0),
            CreateLicense("cutoff", 30),
            CreateLicense("outside-window", 31),
            CreateLicense("suspended-past", -1, LicenseStatuses.Suspended),
            CreateLicense("suspended-future", 1, LicenseStatuses.Suspended),
            CreateLicense("deleted-license", 1, isDeleted: true)
        ]);
        var inactive = CreateProvider("Beta", ProviderStatuses.Inactive);
        inactive.Licenses.Add(CreateLicense("inactive-provider-renewal", 2));
        var pending = CreateProvider("Gamma", ProviderStatuses.Pending);
        var deleted = CreateProvider("Deleted", ProviderStatuses.Active);
        deleted.IsDeleted = true;
        deleted.Licenses.Add(CreateLicense("deleted-provider-license", 1));
        database.Providers.AddRange(active, inactive, pending, deleted);
        await database.SaveChangesAsync();
        database.ChangeTracker.Clear();

        var result = await new DashboardService(database, new FixedTimeProvider())
            .GetAsync(CancellationToken.None);

        Assert.Equal(Today, result.AsOfDate);
        Assert.Equal(3, result.ProviderCount);
        Assert.Equal(7, result.LicenseCount);
        Assert.Equal(1, result.ExpiredLicenseCount);
        Assert.Equal([
            new StatusCountResponse(ProviderStatuses.Active, 1),
            new StatusCountResponse(ProviderStatuses.Inactive, 1),
            new StatusCountResponse(ProviderStatuses.Pending, 1)
        ], result.ProvidersByStatus);
        Assert.Equal([
            new StatusCountResponse(LicenseStatuses.Active, 4),
            new StatusCountResponse(LicenseStatuses.Expired, 1),
            new StatusCountResponse(LicenseStatuses.Suspended, 2)
        ], result.LicensesByStatus);
        Assert.Equal([
            new ProviderLicenseCountResponse(active.Id, active.Name, 6),
            new ProviderLicenseCountResponse(inactive.Id, inactive.Name, 1),
            new ProviderLicenseCountResponse(pending.Id, pending.Name, 0)
        ], result.LicensesPerProvider);
        Assert.Equal(2, result.ProvidersWithExpiringLicensesCount);
        Assert.Equal([
            new ExpiringProviderResponse(active.Id, active.Name, active.County, 2, Today),
            new ExpiringProviderResponse(inactive.Id, inactive.Name, inactive.County, 1, Today.AddDays(2))
        ], result.ProvidersWithExpiringLicenses);
        Assert.Empty(database.ChangeTracker.Entries());
    }

    [Fact]
    public async Task GetAsync_EmptyRegistryReturnsZeroCountsAndEmptyProviderLists()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var database = CreateDatabase(connection);
        await database.Database.EnsureCreatedAsync();

        var result = await new DashboardService(database, new FixedTimeProvider())
            .GetAsync(CancellationToken.None);

        Assert.Equal(0, result.ProviderCount);
        Assert.Equal(0, result.LicenseCount);
        Assert.Equal(0, result.ExpiredLicenseCount);
        Assert.Equal(0, result.ProvidersWithExpiringLicensesCount);
        Assert.All(result.ProvidersByStatus, item => Assert.Equal(0, item.Count));
        Assert.All(result.LicensesByStatus, item => Assert.Equal(0, item.Count));
        Assert.Empty(result.LicensesPerProvider);
        Assert.Empty(result.ProvidersWithExpiringLicenses);
    }

    private static CandidateDbContext CreateDatabase(SqliteConnection connection)
    {
        return new CandidateDbContext(
            new DbContextOptionsBuilder<CandidateDbContext>().UseSqlite(connection).Options);
    }

    private static Provider CreateProvider(string name, string status)
    {
        return new Provider { Id = Guid.NewGuid(), Name = name, County = "Test county", Status = status };
    }

    private static License CreateLicense(
        string number, int daysAhead, string status = LicenseStatuses.Active, bool isDeleted = false)
    {
        return new License
        {
            Id = Guid.NewGuid(),
            Number = number,
            Status = status,
            ExpirationDate = Today.AddDays(daysAhead),
            IsDeleted = isDeleted
        };
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => new(2026, 11, 13, 23, 59, 0, TimeSpan.Zero);
    }
}
