# Provider Assignment – Starter Solution

This is a starter project for the 24‑hour take‑home technical assignment.

- ASP.NET Core MVC application
- Builds and runs successfully
- No business logic implemented
- SQLite and EF Core packages installed

All application logic, database schema, and optional frontend enhancements
are expected to be implemented by the candidate.

Refer to the Assignment document for full requirements.

Setup and run
git clone <repo>

cd <repo>

dotnet restore

dotnet ef database update (or run provided scripts/create_schema.sql to create DB)

dotnet run

Open https://localhost:5001 (or configured URL)

Database
File: App_Data/providers.db (or Data/providers.db)

Schema: show the Provider and License table definitions (copy the SQL DDL).

Why SQLite: lightweight, easy to include DB file in repo for evaluation.

Soft delete design
Implementation: Provider has IsDeleted, DeletedAt, DeletedBy. EF Core global query filter excludes soft‑deleted providers from standard queries. Admin/audit queries use .IgnoreQueryFilters().

Enforcement: All controller/service delete operations call a SoftDeleteProviderAsync method which sets IsDeleted = true and populates DeletedAt/DeletedBy. No code path performs Remove() on Provider.

Required scenarios mapping
Active providers and active licenses: show sample LINQ and SQL queries.

Providers active but licenses expired: show sample LINQ and SQL queries.

Exclusion of soft‑deleted providers: explain global filter and show example.

Assumptions and trade-offs
Licenses are not soft‑deleted by default; can be extended if needed.

IsDeleted boolean chosen for simplicity and compatibility with SQLite.

CreatedBy/UpdatedBy/DeletedBy are strings; in production use a user id.

What I would improve with more time
Add unit/integration tests for soft‑delete behavior.

Add API endpoints and a React dashboard (optional enhancement).

Add role-based admin UI for audit and restore operations.

Add background job to flag expiring licenses and send notifications.