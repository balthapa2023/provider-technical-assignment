using Microsoft.EntityFrameworkCore;
using ProviderAssignmentStarter.Models;

namespace ProviderAssignmentStarter.Data
{
    /// <summary>
    /// Application database context for SQLite.
    /// Manages all entity mappings, soft-delete filters, and database operations.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// DbSet for Provider entities
        /// </summary>
        public DbSet<Provider> Providers { get; set; } = null!;

        /// <summary>
        /// DbSet for License entities
        /// </summary>
        public DbSet<License> Licenses { get; set; } = null!;

        /// <summary>
        /// DbSet for AuditLog entities (immutable, never filtered)
        /// </summary>
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Provider entity
            modelBuilder.Entity<Provider>(entity =>
            {
                entity.HasKey(e => e.ProviderId);
                entity.Property(e => e.ProviderName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.County).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CreatedDate).IsRequired();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);

                // One-to-Many: Provider -> Licenses
                entity.HasMany(e => e.Licenses)
                    .WithOne(l => l.Provider)
                    .HasForeignKey(l => l.ProviderId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Global query filter: exclude soft-deleted providers by default
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Configure License entity
            modelBuilder.Entity<License>(entity =>
            {
                entity.HasKey(e => e.LicenseId);
                entity.Property(e => e.LicenseNumber).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LicenseStatus).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ExpirationDate).IsRequired();
                entity.Property(e => e.CreatedDate).IsRequired();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);

                // Foreign key constraint
                entity.HasOne(e => e.Provider)
                    .WithMany(p => p.Licenses)
                    .HasForeignKey(e => e.ProviderId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Global query filter: exclude soft-deleted licenses by default
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Configure AuditLog entity (NO soft-delete filter - audit is immutable)
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.AuditLogId);
                entity.Property(e => e.EntityType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
                entity.Property(e => e.UserId).HasMaxLength(200);
                entity.Property(e => e.Timestamp).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);

                // No query filter on AuditLog - all audit records must be visible
            });
        }
    }
}
