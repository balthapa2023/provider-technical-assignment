using ProviderAssignmentStarter.Models;

namespace ProviderAssignmentStarter.Data
{
    /// <summary>
    /// Database initialization with sample data
    /// </summary>
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            try
            {
                // Check if we already have dummy data seeded
                if (context.Providers.Any(p => p.ProviderName == "Sunny Days Child Care"))
                {
                    System.Diagnostics.Debug.WriteLine("Database already seeded. Skipping initialization.");
                    return;
                }

                System.Diagnostics.Debug.WriteLine("Starting fresh database seeding...");

                // Create 6 sample providers with 1 license each
                var providers = new Provider[]
                {
                    new Provider
                    {
                        ProviderName = "Sunny Days Child Care",
                        County = "Fulton",
                        Status = "Active",
                        CreatedDate = DateTime.UtcNow.AddMonths(-6),
                        IsDeleted = false
                    },
                    new Provider
                    {
                        ProviderName = "Little Stars Academy",
                        County = "DeKalb",
                        Status = "Active",
                        CreatedDate = DateTime.UtcNow.AddMonths(-4),
                        IsDeleted = false
                    },
                    new Provider
                    {
                        ProviderName = "Rainbow Kids Care",
                        County = "Cobb",
                        Status = "Active",
                        CreatedDate = DateTime.UtcNow.AddMonths(-3),
                        IsDeleted = false
                    },
                    new Provider
                    {
                        ProviderName = "Golden Hour Preschool",
                        County = "Henry",
                        Status = "Active",
                        CreatedDate = DateTime.UtcNow.AddMonths(-2),
                        IsDeleted = false
                    },
                    new Provider
                    {
                        ProviderName = "Bright Futures Learning Center",
                        County = "Gwinnett",
                        Status = "Active",
                        CreatedDate = DateTime.UtcNow.AddMonths(-1),
                        IsDeleted = false
                    },
                    new Provider
                    {
                        ProviderName = "Happy Beginnings Daycare",
                        County = "Clayton",
                        Status = "Inactive",
                        CreatedDate = DateTime.UtcNow.AddMonths(-8),
                        IsDeleted = false
                    }
                };

                foreach (Provider p in providers)
                {
                    context.Providers.Add(p);
                }
                context.SaveChanges();
                System.Diagnostics.Debug.WriteLine($"Added {providers.Length} providers to database.");

                // Create exactly 1 license per provider
                var today = DateTime.UtcNow.Date;
                var licenses = new License[]
                {
                    new License
                    {
                        ProviderId = 1,
                        LicenseNumber = "CC-FUL-2024-001",
                        LicenseStatus = "Active",
                        ExpirationDate = today.AddYears(2),
                        CreatedDate = DateTime.UtcNow.AddMonths(-6),
                        IsDeleted = false
                    },
                    new License
                    {
                        ProviderId = 2,
                        LicenseNumber = "CC-DEK-2024-001",
                        LicenseStatus = "Active",
                        ExpirationDate = today.AddYears(1),
                        CreatedDate = DateTime.UtcNow.AddMonths(-4),
                        IsDeleted = false
                    },
                    new License
                    {
                        ProviderId = 3,
                        LicenseNumber = "CC-COB-2024-001",
                        LicenseStatus = "Active",
                        ExpirationDate = today.AddYears(2).AddMonths(3),
                        CreatedDate = DateTime.UtcNow.AddMonths(-3),
                        IsDeleted = false
                    },
                    new License
                    {
                        ProviderId = 4,
                        LicenseNumber = "CC-HEN-2024-001",
                        LicenseStatus = "Active",
                        ExpirationDate = today.AddYears(1).AddMonths(6),
                        CreatedDate = DateTime.UtcNow.AddMonths(-2),
                        IsDeleted = false
                    },
                    new License
                    {
                        ProviderId = 5,
                        LicenseNumber = "CC-GWI-2024-001",
                        LicenseStatus = "Active",
                        ExpirationDate = today.AddMonths(18),
                        CreatedDate = DateTime.UtcNow.AddMonths(-1),
                        IsDeleted = false
                    },
                    new License
                    {
                        ProviderId = 6,
                        LicenseNumber = "CC-CLA-2024-001",
                        LicenseStatus = "Active",
                        ExpirationDate = today.AddDays(45),
                        CreatedDate = DateTime.UtcNow.AddMonths(-8),
                        IsDeleted = false
                    }
                };

                foreach (License l in licenses)
                {
                    context.Licenses.Add(l);
                }
                context.SaveChanges();
                System.Diagnostics.Debug.WriteLine($"Added {licenses.Length} licenses to database.");
                System.Diagnostics.Debug.WriteLine("Database seeding completed successfully - 6 providers, 1 license each.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during seeding: {ex.Message}");
                throw;
            }
        }
    }
}
