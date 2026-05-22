using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
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

            // Global query filter to exclude soft-deleted providers
            modelBuilder.Entity<Provider>().HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.Entity<Provider>()
                .HasMany(p => p.Licenses)
                .WithOne(l => l.Provider)
                .HasForeignKey(l => l.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Provider>()
                .HasIndex(p => p.ProviderName);

            modelBuilder.Entity<Provider>()
                .Property(p => p.IsDeleted)
                .HasDefaultValue(false);
        }
    }
}