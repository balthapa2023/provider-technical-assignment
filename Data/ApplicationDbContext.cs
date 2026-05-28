using Microsoft.EntityFrameworkCore;
using ProviderAssignmentStarter.Models;

namespace ProviderAssignmentStarter.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Provider> Providers { get; set; } = null!;
        public DbSet<ProviderLicense> Licenses { get; set; } = null!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -----------------------------
            // GLOBAL QUERY FILTERS
            // -----------------------------
            modelBuilder.Entity<Provider>()
                .HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.Entity<ProviderLicense>()
                .HasQueryFilter(l => !l.IsDeleted);

            // -----------------------------
            // RELATIONSHIPS
            // Provider 1 → Many Licenses
            // -----------------------------
            modelBuilder.Entity<Provider>()
                .HasMany(p => p.Licenses)
                .WithOne(l => l.Provider)
                .HasForeignKey(l => l.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            // -----------------------------
            // INDEXES
            // -----------------------------
            modelBuilder.Entity<Provider>()
                .HasIndex(p => p.ProviderName);

            // -----------------------------
            // DEFAULT VALUES
            // -----------------------------
            modelBuilder.Entity<Provider>()
                .Property(p => p.IsDeleted)
                .HasDefaultValue(false);

            modelBuilder.Entity<ProviderLicense>()
                .Property(l => l.IsDeleted)
                .HasDefaultValue(false);
        }
    }
}
