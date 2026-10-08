using Microsoft.EntityFrameworkCore;
using ProviderAssignmentStarter.Models;

namespace ProviderAssignmentStarter.Data;

public static class DbSeeder
{
    public static async Task InitializeAsync(AppDbContext db, string contentRoot)
    {
        await db.Database.EnsureCreatedAsync();

        var viewsPath = Path.Combine(contentRoot, "Scripts", "views.sql");
        if (File.Exists(viewsPath))
            await db.Database.ExecuteSqlRawAsync(await File.ReadAllTextAsync(viewsPath));

        if (await db.Providers.IgnoreQueryFilters().AnyAsync()) return;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var providers = new List<Provider>
        {
            // Scenario 1: Active provider with active license(s)
            new() { ProviderName = "Sunshine Learning Center", County = "Fulton", Status = ProviderStatus.Active,
                Licenses = { new() { LicenseNumber = "LIC-1001", LicenseStatus = LicenseStatus.Active, ExpirationDate = today.AddMonths(18) },
                             new() { LicenseNumber = "LIC-1002", LicenseStatus = LicenseStatus.Active, ExpirationDate = today.AddDays(20) } } },

            // Scenario 2: Provider LOOKS active but its license has expired
            new() { ProviderName = "Peach State Daycare", County = "DeKalb", Status = ProviderStatus.Active,
                Licenses = { new() { LicenseNumber = "LIC-2001", LicenseStatus = LicenseStatus.Active, ExpirationDate = today.AddMonths(-3) } } },

            // Scenario 2 variant: status already flipped to Expired, also past date
            new() { ProviderName = "Little Steps Academy", County = "Cobb", Status = ProviderStatus.Active,
                Licenses = { new() { LicenseNumber = "LIC-3001", LicenseStatus = LicenseStatus.Expired, ExpirationDate = today.AddYears(-1) },
                             new() { LicenseNumber = "LIC-3002", LicenseStatus = LicenseStatus.Active, ExpirationDate = today.AddMonths(6) } } },

            // Inactive provider with a valid license (status != validity)
            new() { ProviderName = "Riverside Family Care", County = "Gwinnett", Status = ProviderStatus.Inactive,
                Licenses = { new() { LicenseNumber = "LIC-4001", LicenseStatus = LicenseStatus.Suspended, ExpirationDate = today.AddMonths(9) } } },

            // Pending provider, no licenses yet (zero-or-more rule)
            new() { ProviderName = "New Horizons Preschool", County = "Fulton", Status = ProviderStatus.Pending },

            // Scenario 3: SOFT-DELETED provider — stays in DB, hidden from standard views
            new() { ProviderName = "Closed Doors Childcare", County = "Clayton", Status = ProviderStatus.Active,
                IsDeleted = true, DeletedDate = DateTime.UtcNow.AddDays(-10), DeletedBy = "seed",
                Licenses = { new() { LicenseNumber = "LIC-9001", LicenseStatus = LicenseStatus.Active, ExpirationDate = today.AddMonths(12) } } },
        };

        db.Providers.AddRange(providers);
        await db.SaveChangesAsync();
    }
}
