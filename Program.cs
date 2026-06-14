using ProviderAssignmentStarter.Data;
using ProviderAssignmentStarter.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register business logic services
builder.Services.AddScoped<IProviderService, ProviderService>();

builder.Services.AddControllersWithViews();
builder.Services.AddCors();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Database initialization
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        // Apply migrations - this will create tables if they don't exist
        dbContext.Database.Migrate();
        System.Diagnostics.Debug.WriteLine("Database migrations applied successfully.");

        // Initialize database with sample data
        DbInitializer.Initialize(dbContext);
        System.Diagnostics.Debug.WriteLine("Database initialized with sample data.");
    }
    catch (Microsoft.Data.Sqlite.SqliteException ex) when (ex.SqliteErrorCode == 1)
    {
        // Handle SQLite Error 1: "table already exists"
        // This occurs when the database has tables but migration history is missing or corrupted.
        // Solution: Delete the corrupted database file and recreate it cleanly.
        System.Diagnostics.Debug.WriteLine($"Migration conflict detected: {ex.Message}. Recreating database...");

        try
        {
            // Get the database file path from the connection string
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            var dbPath = connectionString?.Replace("Data Source=", "").Replace(";", "").Trim() ?? "provider_assignment.db";

            // Dispose the database context to release all connections immediately
            dbContext.Dispose();
            scope.Dispose();

            // Helper method to delete file with retry logic
            void DeleteFileWithRetry(string path, int maxAttempts = 5, int delayMs = 1000)
            {
                for (int attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    try
                    {
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                            System.Diagnostics.Debug.WriteLine($"Successfully deleted: {path}");
                            return;
                        }
                    }
                    catch (IOException) when (attempt < maxAttempts)
                    {
                        System.Diagnostics.Debug.WriteLine($"Attempt {attempt} failed. File is locked. Triggering GC and retrying in {delayMs}ms...");
                        // Force garbage collection to release file handles
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        System.Threading.Thread.Sleep(delayMs);
                    }
                }

                // Final attempt - throw if it fails
                if (File.Exists(path))
                    File.Delete(path);
            }

            // Delete the corrupted database file(s) with retry logic
            DeleteFileWithRetry(dbPath);

            // Also delete SQLite WAL and SHM files if they exist
            if (File.Exists($"{dbPath}-shm")) DeleteFileWithRetry($"{dbPath}-shm");
            if (File.Exists($"{dbPath}-wal")) DeleteFileWithRetry($"{dbPath}-wal");

            // Recreate the scope with a fresh context to apply migrations
            var newScope = app.Services.CreateScope();
            var newDbContext = newScope.ServiceProvider.GetRequiredService<AppDbContext>();
            newDbContext.Database.Migrate();
            System.Diagnostics.Debug.WriteLine("Database recreated and migrations applied successfully.");

            // Initialize database with sample data
            DbInitializer.Initialize(newDbContext);
            System.Diagnostics.Debug.WriteLine("Database initialized with sample data.");

            newDbContext.Dispose();
            newScope.Dispose();
        }
        catch (Exception cleanupEx)
        {
            System.Diagnostics.Debug.WriteLine($"Error during database recovery: {cleanupEx.Message}");
            throw new InvalidOperationException("Failed to recover database. Please manually delete the database file and restart the application.", cleanupEx);
        }
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
