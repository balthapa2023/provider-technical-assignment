# Provider Technical Assignment

This project is an ASP.NET Core MVC application for managing healthcare providers and their licenses.

The solution uses ASP.NET Core MVC, Entity Framework Core, and SQLite.

## Technologies Used

- .NET 8
- ASP.NET Core MVC
- Razor Views
- Entity Framework Core
- SQLite

## Running the Application

### Prerequisites

Install the .NET 8 SDK.

### 1. Restore dependencies

From the project directory, run:

dotnet restore

### 2. Apply the database migrations

Run:

dotnet ef database update

This creates or updates the SQLite database using the Entity Framework Core migrations included with the project.

### 3. Run the application

Run:

dotnet run

The application will display the local URL in the terminal.

For example:

http://localhost:5236

Navigate to:

http://localhost:5236/Providers

to access the Provider management page.

## Database Design

The application contains two main entities: Provider and License.

### Provider

A Provider contains:

- ProviderId - Primary key
- Name
- County
- Status
- IsDeleted
- CreatedDate
- DeletedDate

`IsDeleted` and `DeletedDate` support soft deletion.

### License

A License contains:

- LicenseId - Primary key
- ProviderId - Foreign key to Provider
- LicenseNumber
- Status
- ExpirationDate

A Provider can have zero or more Licenses.

Each License belongs to one Provider.

The combination of ProviderId and LicenseNumber is unique. This prevents the same license number from being entered more than once for the same provider.

The Provider-to-License foreign key uses restricted delete behavior to prevent accidental cascading physical deletion of license records.

## Provider Functionality

The application supports:

- Listing providers
- Creating providers
- Viewing provider details
- Editing providers
- Soft deleting providers
- Viewing licenses belonging to a provider
- Adding licenses to a provider

## Soft Delete Design

Providers are never physically deleted through the application.

When a provider is deleted:

- `IsDeleted` is set to `true`
- `DeletedDate` is set to the current UTC date/time
- The database record remains stored

Entity Framework Core uses a global query filter so soft-deleted providers are automatically excluded from normal application queries.

Licenses associated with soft-deleted providers are also excluded from normal queries.

Deleted providers can still be queried directly for audit and troubleshooting purposes.

Example audit queries are included in:

Data/database.sql

## License Status and Expiration

License status and license expiration are treated as separate concepts.

For example, a license may have:

Status: Active

but have an expiration date that has already passed.

The application determines whether a license is expired by comparing `ExpirationDate` with the current date rather than relying only on the license Status field.

This allows scenarios such as:

- Active provider with an active, non-expired license
- Active provider with an active but expired license

## SQL Scripts

`Data/database.sql` contains SQL queries that demonstrate:

- Standard non-deleted provider queries
- Soft-deleted provider audit queries
- Provider/license relationships
- Active providers with active licenses
- Active providers with expired licenses
- Queries including soft-deleted providers for troubleshooting

The database schema itself is managed through Entity Framework Core migrations.

## Assumptions and Tradeoffs

### Provider and License Status

Status values are stored as strings for simplicity.

In a larger production system, these could be represented using enums or lookup/reference tables to provide stronger validation and consistency.

### License Management

The assignment focuses primarily on Provider management and the Provider/License relationship.

The application supports adding and viewing licenses. Full license edit/delete functionality was not added in order to keep the implementation focused on the required scenarios.

### Soft Delete

Soft delete is implemented for Providers because audit-safe persistence is a requirement.

A global EF Core query filter keeps deleted providers out of standard application operations while preserving their database records.

### User Interface

The UI intentionally uses standard Razor views and Bootstrap styling.

The focus of the implementation is backend behavior, database design, maintainability, and correctness rather than extensive visual styling.

## Possible Future Improvements

Given additional time, I would consider:

- Adding edit functionality for licenses
- Adding license-level soft deletion if required by future business rules
- Replacing string status fields with enums or lookup tables
- Adding automated unit and integration tests
- Adding search, filtering, sorting, and pagination to the Provider list
- Adding an audit/history view for authorized users
- Adding more detailed validation and user-friendly error handling
- Moving Provider and License operations into service classes if application complexity grows

## Project Structure

- `Controllers/` - MVC controllers and request handling
- `Models/` - Provider and License domain models
- `Data/` - EF Core database context and SQL reference script
- `Migrations/` - Entity Framework Core database migrations
- `Views/Providers/` - Provider and License Razor views
- `providerassignment.db` - SQLite database

## Notes

The project intentionally uses server-rendered Razor views. React was considered optional for this assignment, so the implementation prioritizes the required ASP.NET Core MVC, EF Core, SQLite, Provider management, license scenarios, and soft-delete behavior.
