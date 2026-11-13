using Candidate.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Candidate.API.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(CandidateDbContext database)
    {
        if (await database.Providers.AnyAsync())
        {
            return;
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var now = DateTimeOffset.UtcNow;

        var lakeview = new Provider
        {
            Id = Guid.NewGuid(),
            Name = "Lakeview Community Care",
            County = "Fulton",
            Status = ProviderStatuses.Active,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };
        var familyServices = new Provider
        {
            Id = Guid.NewGuid(),
            Name = "Family Services of Fulton",
            County = "Fulton",
            Status = ProviderStatuses.Active,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };
        var homeSupport = new Provider
        {
            Id = Guid.NewGuid(),
            Name = "DeKalb Home Support",
            County = "DeKalb",
            Status = ProviderStatuses.Inactive,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };
        var archived = new Provider
        {
            Id = Guid.NewGuid(),
            Name = "Archived Example Provider",
            County = "Cobb",
            Status = ProviderStatuses.Active,
            IsDeleted = true,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
            DeletedAtUtc = now
        };

        database.Providers.AddRange(lakeview, familyServices, homeSupport, archived);
        database.Licenses.AddRange(
            CreateLicense(lakeview, "FULTON-1024", LicenseStatuses.Active, today.AddDays(365), now),
            CreateLicense(lakeview, "FULTON-1025", LicenseStatuses.Suspended, today.AddDays(180), now),
            CreateLicense(familyServices, "FULTON-2048", LicenseStatuses.Active, today.AddDays(-30), now),
            CreateLicense(homeSupport, "DEKALB-4096", LicenseStatuses.Active, today.AddDays(240), now),
            CreateLicense(archived, "COBB-8192", LicenseStatuses.Active, today.AddDays(90), now));

        await database.SaveChangesAsync();
    }

    private static License CreateLicense(
        Provider provider,
        string number,
        string status,
        DateOnly expirationDate,
        DateTimeOffset createdAtUtc)
    {
        return new License
        {
            Id = Guid.NewGuid(),
            Provider = provider,
            Number = number,
            Status = status,
            ExpirationDate = expirationDate,
            CreatedAtUtc = createdAtUtc,
            UpdatedAtUtc = createdAtUtc
        };
    }
}
