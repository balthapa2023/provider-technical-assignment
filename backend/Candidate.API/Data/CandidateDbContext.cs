using Candidate.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Candidate.API.Data;

public sealed class CandidateDbContext(DbContextOptions<CandidateDbContext> options)
    : DbContext(options)
{
    public DbSet<Provider> Providers => Set<Provider>();

    public DbSet<License> Licenses => Set<License>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Provider>(entity =>
        {
            entity.HasKey(provider => provider.Id);
            entity.Property(provider => provider.Name).HasMaxLength(120).IsRequired();
            entity.Property(provider => provider.County).HasMaxLength(80).IsRequired();
            entity.Property(provider => provider.Status).HasMaxLength(20).IsRequired();
            entity.HasIndex(provider => provider.Name);
            entity.HasQueryFilter(provider => !provider.IsDeleted);
        });

        modelBuilder.Entity<License>(entity =>
        {
            entity.HasKey(license => license.Id);
            entity.Property(license => license.Number)
                .HasMaxLength(40)
                .UseCollation("NOCASE")
                .IsRequired();
            entity.Property(license => license.Status).HasMaxLength(20).IsRequired();
            entity.HasIndex(license => new { license.ProviderId, license.Number })
                .IsUnique()
                .HasFilter("\"IsDeleted\" = 0");
            entity.HasOne(license => license.Provider)
                .WithMany(provider => provider.Licenses)
                .HasForeignKey(license => license.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasQueryFilter(license =>
                !license.IsDeleted &&
                !license.Provider.IsDeleted);
        });
    }
}
