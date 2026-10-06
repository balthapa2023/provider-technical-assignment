using Microsoft.EntityFrameworkCore;
using ProviderAssignmentStarter.Models;

namespace ProviderAssignmentStarter.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Represents the Providers table.
    public DbSet<Provider> Providers { get; set; }

    // Represents the Licenses table.
    public DbSet<License> Licenses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // One Provider can have many Licenses.
        modelBuilder.Entity<Provider>()
            .HasMany(p => p.Licenses)
            .WithOne(l => l.Provider)
            .HasForeignKey(l => l.ProviderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Soft-deleted providers are hidden from normal queries.
        modelBuilder.Entity<Provider>()
            .HasQueryFilter(p => !p.IsDeleted);

        // A provider cannot have the same license number twice.
        modelBuilder.Entity<License>()
            .HasQueryFilter(l => !l.Provider.IsDeleted);
        modelBuilder.Entity<License>()
            .HasIndex(l => new { l.ProviderId, l.LicenseNumber })
            .IsUnique();
    }
}