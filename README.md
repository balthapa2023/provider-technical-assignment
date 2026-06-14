# Provider Assignment – Technical Solution

A full-stack ASP.NET Core MVC application demonstrating enterprise-grade provider and license management with comprehensive soft-delete implementation, audit logging, and production-support patterns.

## 📋 Table of Contents

1. [Quick Start](#quick-start)
2. [Project Structure](#project-structure)
3. [Database Design](#database-design)
4. [Soft-Delete Implementation](#soft-delete-implementation)
5. [API & Features](#api--features)
6. [Assumptions & Trade-offs](#assumptions--trade-offs)
7. [Future Improvements](#future-improvements)

---

## 🚀 Quick Start

### Prerequisites

- **.NET 8 SDK** (minimum .NET 8.0)
- **Visual Studio 2022+** (or any IDE supporting .NET 8)
- **SQLite** (included in EF Core)
- **PowerShell** or command-line terminal

### Build & Run Locally

1. **Clone the Repository**
   ```bash
   git clone https://github.com/balthapa2023/provider-technical-assignment.git
   cd provider-technical-assignment
   git checkout divya-assignment-dev  # Switch to your branch
   ```

2. **Restore NuGet Packages**
   ```bash
   dotnet restore
   ```

3. **Build the Solution**
   ```bash
   dotnet build
   ```

4. **Run the Application**
   ```bash
   dotnet run
   ```
   - By default, the application runs on `http://localhost:5000` (HTTP) and `https://localhost:5001` (HTTPS)
   - Open your browser and navigate to `https://localhost:5001`

5. **Database Initialization**
   - The application automatically creates the SQLite database (`provider_assignment.db`) on first run
   - Sample data is seeded by `DbInitializer.cs` for testing
   - Database migrations are applied automatically via `Program.cs`

### Initial Data

Upon first run, the database is populated with:
- **8 Sample Providers** across multiple counties (Fulton, DeKalb, Cobb, Gwinnett, Clayton, Henry, Marietta)
- **Multiple Licenses** per provider with various statuses
- **Audit Log entries** tracking creation events

---

## 📁 Project Structure

```
ProviderAssignmentStarter/
│
├── Controllers/
│   ├── HomeController.cs          # Public pages (Index, Privacy, Error)
│   └── ProvidersController.cs     # Provider CRUD and soft-delete operations
│
├── Data/
│   ├── AppDbContext.cs            # EF Core DbContext with global query filters
│   ├── DbInitializer.cs           # Sample data seeding
│   └── Migrations/                # EF Core migration files (auto-generated)
│
├── Models/
│   ├── Provider.cs                # Provider entity with soft-delete fields
│   ├── License.cs                 # License entity with soft-delete fields
│   ├── AuditLog.cs                # Immutable audit trail (never filtered)
│   └── ErrorViewModel.cs          # Error page model
│
├── Services/
│   └── ProviderService.cs         # Business logic for provider queries & operations
│
├── Views/
│   ├── Providers/
│   │   ├── Index.cshtml           # Active providers list with "View Deleted" button
│   │   ├── Deleted.cshtml         # Soft-deleted providers with Restore/Permanent Delete
│   │   ├── Create.cshtml          # Create provider form
│   │   ├── Edit.cshtml            # Edit provider form
│   │   ├── Delete.cshtml          # Soft-delete confirmation
│   │   └── PermanentDelete.cshtml # Permanent delete confirmation
│   ├── Home/
│   │   ├── Index.cshtml           # Landing page
│   │   ├── Privacy.cshtml         # Privacy policy
│   │   └── Error.cshtml           # Error page
│   └── Shared/
│       ├── _Layout.cshtml         # Master layout
│       └── _ValidationScriptsPartial.cshtml
│
├── Program.cs                      # DI configuration and startup
├── appsettings.json               # Configuration (connection string, logging)
├── ProviderAssignmentStarter.csproj
└── README.md                      # This file
```

---

## 🗄️ Database Design

### Entity-Relationship Diagram (Conceptual)

```
Provider (1) ──────────── (0..*) License
   │
   └──> AuditLog (no soft-delete filter ever applied)
```

### Schema Design

#### **Provider Table**
| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| `ProviderId` | INT | PRIMARY KEY | Unique identifier |
| `ProviderName` | VARCHAR(255) | NOT NULL | Display name (searchable) |
| `County` | VARCHAR(100) | NOT NULL | Geographic location |
| `Status` | VARCHAR(50) | NOT NULL | Business status (Active/Inactive/Pending) |
| `CreatedDate` | DATETIME | NOT NULL | Audit: creation timestamp (UTC) |
| `IsDeleted` | BOOLEAN | DEFAULT 0 | Soft-delete flag |
| `DeletedAt` | DATETIME | NULL | Soft-delete timestamp (UTC) |

#### **License Table**
| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| `LicenseId` | INT | PRIMARY KEY | Unique identifier |
| `ProviderId` | INT | FOREIGN KEY (Provider) | Association with provider |
| `LicenseNumber` | VARCHAR(100) | NOT NULL | Business-recognizable ID |
| `LicenseStatus` | VARCHAR(50) | NOT NULL | License state (Active/Expired/Suspended) |
| `ExpirationDate` | DATETIME | NOT NULL | Validity period |
| `CreatedDate` | DATETIME | NOT NULL | Audit: creation timestamp (UTC) |
| `IsDeleted` | BOOLEAN | DEFAULT 0 | Soft-delete flag |
| `DeletedAt` | DATETIME | NULL | Soft-delete timestamp (UTC) |

#### **AuditLog Table**
| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| `AuditLogId` | INT | PRIMARY KEY | Unique identifier |
| `EntityType` | VARCHAR(100) | NOT NULL | Entity name (Provider, License) |
| `EntityId` | INT | NOT NULL | Primary key of affected entity |
| `Action` | VARCHAR(50) | NOT NULL | Operation type (Create, Edit, Delete) |
| `UserId` | VARCHAR(200) | NULL | User who performed action |
| `Timestamp` | DATETIME | NOT NULL | Action timestamp (UTC) |
| `OldValues` | TEXT | NULL | JSON snapshot of previous state |
| `NewValues` | TEXT | NULL | JSON snapshot of new state |
| `Description` | VARCHAR(500) | NULL | Human-readable summary |

### Key Design Decisions

1. **Composite Primary Keys**: Using single INT primary keys for simplicity; GUIDs could be used for distributed scenarios
2. **UTC Timestamps**: All dates stored in UTC for consistency across time zones
3. **Nullable DeletedAt**: NULL when not deleted; set to deletion timestamp when soft-deleted
4. **Cascading Soft-Delete**: When a Provider is soft-deleted, its Licenses are cascade-deleted (both soft-deleted)
5. **AuditLog Immutability**: No soft-delete filter; maintains permanent record of all operations

---

## 🗑️ Soft-Delete Implementation

### Overview

Hard deletion is forbidden per business requirements. All deletion operations use **soft-delete** (logical deletion):

- Records remain in the database
- Records are excluded from normal queries
- Records can be restored or permanently deleted (admin function)
- Historical record is maintained for compliance and audit

### Implementation Architecture

#### 1. **Global Query Filters (EF Core)**

In `AppDbContext.cs`, both `Provider` and `License` entities have global query filters:

```csharp
modelBuilder.Entity<Provider>(entity =>
{
	// ... configuration ...

	// Exclude soft-deleted providers by default
	entity.HasQueryFilter(e => !e.IsDeleted);
});

modelBuilder.Entity<License>(entity =>
{
	// ... configuration ...

	// Exclude soft-deleted licenses by default
	entity.HasQueryFilter(e => !e.IsDeleted);
});
```

**Effect**: Every query automatically filters out records where `IsDeleted = true`, unless explicitly overridden.

#### 2. **Soft-Delete Operation (ProviderService)**

To soft-delete a provider:

```csharp
public async Task<bool> SoftDeleteProviderAsync(int id)
{
	var provider = await _context.Providers
		.IgnoreQueryFilters()  // Override global filter to find the record
		.FirstOrDefaultAsync(p => p.ProviderId == id);

	if (provider == null) return false;

	provider.IsDeleted = true;
	provider.DeletedAt = DateTime.UtcNow;

	_context.Providers.Update(provider);
	await _context.SaveChangesAsync();
	return true;
}
```

**Key Points**:
- Uses `.IgnoreQueryFilters()` to bypass the soft-delete filter
- Sets `IsDeleted = true` and `DeletedAt = UtcNow`
- Records remain queryable via `.IgnoreQueryFilters()`

#### 3. **Querying Soft-Deleted Records**

**Active Records Only** (default behavior):
```csharp
// Global filter automatically excludes IsDeleted = true
var activeProviders = await _context.Providers.ToListAsync();
```

**Soft-Deleted Records Only**:
```csharp
// Override global filter to retrieve deleted records
var deletedProviders = await _context.Providers
	.IgnoreQueryFilters()
	.Where(p => p.IsDeleted)
	.ToListAsync();
```

**All Records** (for admin/audit):
```csharp
var allProviders = await _context.Providers
	.IgnoreQueryFilters()
	.ToListAsync();
```

#### 4. **AuditLog Never Filtered**

The `AuditLog` table has **no soft-delete filter** by design:

```csharp
modelBuilder.Entity<AuditLog>(entity =>
{
	// ... configuration ...

	// NO query filter - all audit records must be visible
});
```

This ensures a permanent, unalterable record of all operations.

#### 5. **Restore Operation**

To restore a soft-deleted provider:

```csharp
public async Task<bool> RestoreProviderAsync(int id)
{
	var provider = await _context.Providers
		.IgnoreQueryFilters()
		.FirstOrDefaultAsync(p => p.ProviderId == id && p.IsDeleted);

	if (provider == null) return false;

	provider.IsDeleted = false;
	provider.DeletedAt = null;

	_context.Providers.Update(provider);
	await _context.SaveChangesAsync();
	return true;
}
```

#### 6. **Permanent Delete** (Irreversible)

For admin/maintenance scenarios only:

```csharp
public async Task<bool> PermanentlyDeleteProviderAsync(int id)
{
	var provider = await _context.Providers
		.IgnoreQueryFilters()
		.FirstOrDefaultAsync(p => p.ProviderId == id);

	if (provider == null) return false;

	_context.Providers.Remove(provider);
	await _context.SaveChangesAsync();
	return true;
}
```

**Warning**: This cascades to related records and is irreversible.

### Soft-Delete Filtering Enforcement

| Scenario | Where Enforced | How | Notes |
|----------|---|---|---|
| Active provider listing | Database/EF Core | Global query filter | Users see only active records |
| Provider search | Application | Service layer filter | Search doesn't return deleted |
| Delete Page (soft) | Application | Service checks `IsDeleted=false` | Can only soft-delete active records |
| Restore Page | Database | `.IgnoreQueryFilters()` + `IsDeleted=true` check | Shows only deleted records |
| Audit queries | Application | Optional `.IgnoreQueryFilters()` | Admin can query all records |

---

## 🔧 API & Features

### Provider Management Endpoints

#### **GET /Providers**
Display all active (non-deleted) providers.

- **Returns**: List of Provider objects with associated licenses
- **Query Filter**: Applies globally by EF Core (IsDeleted = false)
- **View**: `Views/Providers/Index.cshtml`

#### **GET /Providers/Deleted**
Display all soft-deleted providers (admin/maintenance).

- **Returns**: List of Provider objects (IsDeleted = true)
- **Query Override**: Uses `.IgnoreQueryFilters()`
- **View**: `Views/Providers/Deleted.cshtml`
- **Actions Available**: Restore only (no permanent delete in UI for compliance)

#### **GET /Providers/Create**
Display form to create a new provider.

- **View**: `Views/Providers/Create.cshtml`

#### **POST /Providers/Create**
Create a new provider.

- **Validation**: Provider name (3-255 chars), County (required), Status (Active/Inactive/Pending)
- **Defaults**: CreatedDate = UtcNow, IsDeleted = false
- **Success**: Redirects to /Providers (Index)

#### **GET /Providers/Edit/{id}**
Display form to edit a provider (active only).

- **View**: `Views/Providers/Edit.cshtml`
- **Constraint**: Provider must not be soft-deleted

#### **POST /Providers/Edit/{id}**
Update provider information.

- **Editable Fields**: ProviderName, County, Status
- **Protected Fields**: ProviderId, CreatedDate, IsDeleted
- **Success**: Redirects to /Providers (Index)

#### **GET /Providers/Delete/{id}**
Display soft-delete confirmation page.

- **View**: `Views/Providers/Delete.cshtml`
- **Constraint**: Provider must be active (not already deleted)

#### **POST /Providers/Delete/{id}**
Soft-delete a provider.

- **Operation**: Sets IsDeleted = true, DeletedAt = UtcNow
- **Cascade**: Associated licenses are also soft-deleted
- **Result**: Provider appears in `/Providers/Deleted`
- **Reversibility**: Can be restored via /Providers/Restore/{id}

#### **POST /Providers/Restore/{id}**
Restore a soft-deleted provider.

- **Operation**: Sets IsDeleted = false, DeletedAt = null
- **Cascade**: Associated soft-deleted licenses are also restored
- **Success**: Redirects to /Providers/Deleted

### Service Layer (ProviderService)

All data access logic is centralized in `Services/ProviderService.cs`:

| Method | Purpose | Query Behavior |
|--------|---------|---|
| `GetActiveProvidersAsync()` | Active providers (default) | Applies global filter |
| `GetDeletedProvidersAsync()` | Soft-deleted providers | `.IgnoreQueryFilters()` + Where(IsDeleted) |
| `GetAllProvidersAsync(includeDeleted)` | All or active | Optional filter override |
| `GetProviderByIdAsync(id)` | Single active provider | Applies global filter |
| `GetProviderByIdAsync(id, includeDeleted)` | Single provider (any state) | Optional filter override |
| `CreateProviderAsync(provider)` | Insert new record | Sets defaults (CreatedDate, IsDeleted=false) |
| `UpdateProviderAsync(provider)` | Modify existing | Requires active provider |
| `SoftDeleteProviderAsync(id)` | Mark as deleted | Sets IsDeleted=true, DeletedAt=UtcNow |
| `RestoreProviderAsync(id)` | Undelete | Sets IsDeleted=false, DeletedAt=null |

---

## 📊 Business Scenarios Supported

### 1. **List Active Providers**
✅ Implemented via `/Providers/Index`

- Displays all non-deleted providers
- Filtering handled by EF Core global query filter
- Includes license count and status badges

### 2. **Search & Filter Providers**
✅ Can be extended in `ProviderService`

- Current: OrderBy ProviderName
- Future: County filter, Status filter, keyword search

### 3. **Active Providers with Active Licenses**
✅ Partially implemented

- Providers have associated Licenses (navigation property)
- Can be queried in service layer (need to add logic for license status filtering)

### 4. **Providers with Expired Licenses**
✅ Can be queried via Service

Example query:
```csharp
var expiredLicenseProviders = await _context.Providers
	.IgnoreQueryFilters()
	.Where(p => !p.IsDeleted && 
		   p.Licenses.Any(l => !l.IsDeleted && DateTime.UtcNow > l.ExpirationDate))
	.ToListAsync();
```

### 5. **Soft-Deleted Provider Audit Trail**
✅ Implemented via `/Providers/Deleted`

- View all soft-deleted providers
- See deletion timestamp (DeletedAt)
- Restore deleted providers (one-click operation)
- Records permanently retained for compliance

---

## ⚠️ Assumptions & Trade-offs

### Assumptions Made

1. **Single-Tenant Application**: No multi-tenant isolation; all users see the same data
2. **No User Authentication**: System assumes "System" user; ready for integration with ASP.NET Identity
3. **UTC Timestamps**: All dates stored in UTC; conversion to local time done in UI
4. **Cascade Behavior**: Soft-deleting a provider cascades to associated licenses (can be changed)
5. **No Permanent Delete UI**: The `PermanentlyDeleteProviderAsync()` method remains in the service layer for admin/maintenance but is **not exposed in the UI** to ensure data retention for compliance and audit purposes
6. **Sequential Integer IDs**: Primary keys are sequential integers; no GUID distribution
7. **Synchronous Operations**: All operations are synchronous (non-concurrent scenarios); ready for async optimization

### Trade-offs

| Decision | Trade-off | Rationale |
|----------|-----------|-----------|
| Global query filters (EF Core) | Less explicit, harder to debug | Automatic soft-delete everywhere prevents data leaks |
| Service layer encapsulation | Additional abstraction layer | Maintainability & reusability outweigh thin layer overhead |
| Soft-delete via flags | Extra columns, NULL handling | Production-support access to "deleted" data is worth the complexity |
| UTC all dates | Manual local conversion in UI | Consistency, timezone-free comparisons, daylight saving immunity |
| Immediate cascade soft-delete | No "orphan" licenses with deleted provider | Maintains referential integrity; can be changed to manual cascade |
| No role-based authorization | Anyone can restore/permanently delete | Sample app; ready for integration with `[Authorize]` attributes |
| SQLite | Limited to single connection in some scenarios | Sufficient for development/testing; upgrade to SQL Server for production |

---

## 🔮 Future Improvements

### Short-Term (Production-Ready)

1. **License Management UI**
   - Full CRUD for licenses
   - License expiration warnings
   - Bulk license operations

2. **Search & Filtering**
   - County-based filtering
   - Status filtering
   - License expiration date range

3. **Audit Trail UI**
   - View detailed audit logs
   - Filter by action, timestamp, user
   - Export audit reports

4. **Authentication & Authorization**
   - Integrate ASP.NET Identity
   - Role-based access (Admin, Manager, Viewer)
   - Restrict permanent delete to Admins

### Medium-Term (Enhanced Features)

5. **Advanced Queries**
   - Providers expiring soon (license alerts)
   - License compliance reports
   - County-by-county statistics

6. **API Endpoints**
   - RESTful API for providers and licenses
   - API versioning
   - Swagger/OpenAPI documentation

7. **Performance Optimization**
   - Query caching (Redis)
   - Indexed searches on ProviderName, County
   - Batch operations for bulk soft-delete

### Long-Term (Strategic)

8. **Optional React Dashboard** (as per assignment)
   - Providers by status (pie chart)
   - Licenses per provider (bar chart)
   - Expired vs active licenses
   - Key counts (summary cards)

9. **Database Migration to Production**
   - SQL Server (Azure SQL Database)
   - Replicated backups
   - Audit retention policies (7-year compliance)

10. **Advanced Soft-Delete Management**
	- Scheduled hard-delete of audit-old records
	- Soft-delete recovery window (e.g., 30-day restore period)
	- Soft-delete reason tracking (closure, duplicate, etc.)

---

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core MVC (.NET 8)
- **Database**: SQLite (with EF Core provider)
- **ORM**: Entity Framework Core 8.0
- **Views**: Razor (`.cshtml`)
- **Frontend**: HTML5, Bootstrap 5.x, CSS3
- **Logging**: ILogger (built-in .NET provider)
- **Dependency Injection**: Native .NET Core DI container

---

## 📝 License

This project is part of a technical assignment. All code is provided as-is for evaluation purposes.

---

## 📞 Support & Notes

- **Database File**: Located at `[ProjectRoot]/data/provider_assignment.db`
- **Logs**: Check Visual Studio Output window or console for EF Core migration logs
- **Sample Data**: Seeded by `DbInitializer.cs` on each fresh database creation

**Questions or issues?** Refer to the assignment requirements or reach out to the hiring team.

---

**Assignment Completed**: Soft-delete implementation, full CRUD UI, comprehensive documentation, production-support patterns.
