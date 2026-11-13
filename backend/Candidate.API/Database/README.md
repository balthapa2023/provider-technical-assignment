# SQLite database

The API stores its local database at `Data/providers.db`. The checked-in database is seeded with examples for active, expired, suspended, inactive, and soft-deleted provider/license scenarios.

## Schema and setup

Entity Framework Core defines the `Providers` and `Licenses` tables and creates them on API startup with `EnsureCreated`. A new database is seeded when it has no providers. Both providers and licenses use `IsDeleted` and `DeletedAtUtc`; global query filters keep deleted records and licenses belonging to deleted providers out of standard results. License rows retain their provider foreign key after either record is soft-deleted.

No standalone SQL scripts are used: the EF Core model is the schema definition. The root [README](../../../README.md) documents the model and local run instructions. `EnsureCreated` is suitable for this exercise but does not apply schema upgrades; use versioned EF Core migrations for ongoing production changes.
