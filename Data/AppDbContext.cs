using Microsoft.EntityFrameworkCore;
using ProviderAssignmentStarter.Models;
using System.Reflection.Emit;

namespace ProviderAssignmentStarter.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Provider> Providers => Set<Provider>();
    public DbSet<License> Licenses => Set<License>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Provider>(e =>
        {
            e.ToTable("Providers");
            e.HasKey(p => p.ProviderId);
            e.Property(p => p.ProviderName).IsRequired().HasMaxLength(200);
            e.Property(p => p.County).IsRequired().HasMaxLength(100);
            e.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
            e.Property(p => p.DeletedBy).HasMaxLength(100);
            e.HasIndex(p => p.IsDeleted);
            e.HasIndex(p => new { p.ProviderName, p.County });

            e.HasQueryFilter(p => !p.IsDeleted);
            e.HasQueryFilter(p => !p.IsDeleted);
        });

        mb.Entity<License>(e =>
        {
            e.ToTable("Licenses");
            e.HasKey(l => l.LicenseId);
            e.Property(l => l.LicenseNumber).IsRequired().HasMaxLength(50);
            e.Property(l => l.LicenseStatus).HasConversion<string>().HasMaxLength(20);
            e.HasIndex(l => new { l.ProviderId, l.LicenseNumber }).IsUnique();

            e.HasOne(l => l.Provider)
             .WithMany(p => p.Licenses)
             .HasForeignKey(l => l.ProviderId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasQueryFilter(l => !l.Provider.IsDeleted);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries<ISoftDeletable>()
                     .Where(e => e.State == EntityState.Deleted))
        {
            entry.State = EntityState.Modified;
            entry.Entity.IsDeleted = true;
            entry.Entity.DeletedDate = DateTime.UtcNow;
            entry.Entity.DeletedBy ??= "system";
        }
        foreach (var entry in ChangeTracker.Entries<ISoftDeletable>()
                     .Where(e => e.State == EntityState.Deleted))
        {
            entry.State = EntityState.Modified;
            entry.Entity.IsDeleted = true;
            entry.Entity.DeletedDate = DateTime.UtcNow;
            entry.Entity.DeletedBy ??= "system";
        }

        foreach (var entry in ChangeTracker.Entries<Provider>()
                     .Where(e => e.State == EntityState.Modified))
            entry.Entity.ModifiedDate = DateTime.UtcNow;

        return base.SaveChangesAsync(ct);
    }
}
