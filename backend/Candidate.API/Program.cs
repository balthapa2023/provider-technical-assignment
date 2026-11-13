using Candidate.API.Data;
using Candidate.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<IDashboardService, DashboardService>();

var databaseDirectory = Path.Combine(
    builder.Environment.ContentRootPath,
    "Database",
    "Data");
Directory.CreateDirectory(databaseDirectory);

var databasePath = Path.Combine(databaseDirectory, "providers.db");
builder.Services.AddDbContext<CandidateDbContext>(options =>
    options.UseSqlite($"Data Source={databasePath}"));

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var database = scope.ServiceProvider.GetRequiredService<CandidateDbContext>();
    await database.Database.EnsureCreatedAsync();
    await DatabaseSeeder.SeedAsync(database);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program
{
}
