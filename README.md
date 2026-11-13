# Provider Registry

A small provider and license registry built with ASP.NET Core Web API, Entity Framework Core, SQLite, React, and Vite. Provider and license records are loaded from the API; the frontend contains no business sample data.

## Requirements

- .NET 8 SDK
- Node.js and npm

## Run locally

Start the API from the repository root:

```powershell
dotnet restore CandidateAssessment.sln
dotnet run --no-launch-profile --urls http://localhost:5000 --project backend\Candidate.API
```

In another terminal, start the React app:

```powershell
cd frontend\React.UI
npm ci
npm run dev
```

Open the address printed by Vite (normally `http://localhost:5173`). Vite forwards `/api` requests to `http://localhost:5000`; keep the API on that address while using the development server. Swagger is available at `http://localhost:5000/swagger` in the Development environment.

Build the solution/API with `dotnet build CandidateAssessment.sln`. Build the React production bundle separately with `npm run build` from `frontend\React.UI`.

## Data model and database

The SQLite database is [providers.db](backend/Candidate.API/Database/Data/providers.db). Entity Framework Core creates the schema on API startup and stores the file in `backend/Candidate.API/Database/Data/`. The provider table includes a GUID key, name, county, lifecycle status, creation/update timestamps, and soft-delete fields. Each license has a required provider foreign key, license number, editable status, expiration date, creation/update timestamps, and soft-delete fields. A case-insensitive partial unique index prevents duplicate active license numbers per provider while allowing a deleted number to be reissued. Provider deletion does not cascade to licenses.

The API uses global EF query filters to exclude deleted providers and licenses from normal queries. Deletion sets `IsDeleted`, `DeletedAtUtc`, and `UpdatedAtUtc`; it never removes a provider, license, or provider/license association. The `GET /api/providers/deleted` and nested deleted-license endpoints intentionally bypass those filters for audit and maintenance queries. They are included for this assignment and should be protected by authorization in a deployed system.

The database is seeded only when it contains no providers. Its examples demonstrate an active provider with a valid active license, an active provider with an expired license, an inactive provider with a valid license, a suspended license, and a soft-deleted provider whose data remains available through the audit endpoint. License validity is calculated from `ExpirationDate` using the current UTC date; a suspended license remains suspended even when its expiration date has passed. Provider status and license validity are evaluated separately.

`EnsureCreated` is used to make the sample database easy to run without a migration step. It does not update an existing schema; a production system should replace it with versioned EF Core migrations. The schema is defined in the EF model, so no standalone SQL scripts are currently required. See [Database/README.md](backend/Candidate.API/Database/README.md) for the database artifact location.

## API

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/providers` | List non-deleted providers |
| `GET` | `/api/dashboard` | Read-only registry-wide provider and license insights |
| `GET` | `/api/providers?search=Fulton` | Search provider names and counties |
| `GET` | `/api/providers?scenario=active-with-active-licenses` | Active providers with at least one currently active license |
| `GET` | `/api/providers?scenario=active-with-expired-licenses` | Active providers with at least one expired license |
| `GET` | `/api/providers/{id}` | Provider detail and associated licenses |
| `GET` | `/api/providers/deleted` | Soft-deleted providers for audit |
| `POST` | `/api/providers` | Create a provider |
| `PUT` | `/api/providers/{id}` | Update a provider |
| `DELETE` | `/api/providers/{id}` | Soft-delete a provider |
| `GET` | `/api/providers/{providerId}/licenses` | List a provider's active license records |
| `GET` | `/api/providers/{providerId}/licenses/deleted` | List the provider's soft-deleted licenses for audit |
| `POST` | `/api/providers/{providerId}/licenses` | Add a license linked to the provider |
| `PUT` | `/api/providers/{providerId}/licenses/{licenseId}` | Edit a linked license |
| `DELETE` | `/api/providers/{providerId}/licenses/{licenseId}` | Soft-delete a linked license |

Provider status must be `Active`, `Inactive`, or `Pending`. API validation enforces required names and counties, field lengths, and allowed statuses. A license's effective status is returned as `Active`, `Expired`, or `Suspended`; expiration is derived by the API rather than inferred from provider status.

## React workflows

The directory loads providers from `GET /api/providers`, supports name/county search and the two license scenarios, and provides create, edit, and confirmed soft-delete workflows. Selecting a provider loads its license details from the API; the detail panel supports adding, editing, and confirmed soft deletion of licenses linked to that provider. Loading, empty, validation, and request-error states are shown in the UI. The Vite development proxy keeps browser requests on `/api`; a deployment should route that path to the API or configure an equivalent same-origin reverse proxy.

## Assumptions and trade-offs

- Providers can have multiple licenses. A license's provider is assigned by the nested API route and cannot be changed through the license edit request.
- License stored status is `Active` or `Suspended`; `Expired` is a derived status based on the expiration date.
- License numbers are unique per provider among non-deleted licenses. A deleted number can be reused; the deleted record remains available in the audit endpoint.
- The sample audit endpoint is unauthenticated to keep the local exercise easy to inspect. Production access must be restricted.
- There is no pagination or concurrency token because the included dataset and workflows are intentionally small.
- The database file is included as a deliverable. The API seeds a new/empty database; it will not overwrite existing data.

## Further improvements

With more time, I would add EF Core migrations, broader API and UI automated tests, authentication and role-based audit access, pagination, and operational logging/health checks.

## Read-only provider and license dashboard

Choose **Dashboard** in the navigation, or open `http://localhost:5173/#dashboard`. The existing **Directory** remains available for management; the dashboard has no create, edit, or delete actions. Refresh reloads current backend data. Loading, empty, and request-error states are explicit, with a retry action after failures.

### Why these metrics

- **Summary cards** show the total providers, total licenses, expired licenses, and distinct providers needing renewal, giving a quick registry-wide overview independent of directory filters.
- **Providers by status** shows the Active/Inactive/Pending lifecycle mix to highlight onboarding and operational workload.
- **License health** separates Active, Expired, and Suspended licenses. Suspended licenses are not mislabeled as active or expired, so all licenses reconcile to the total.
- **Licenses per provider** highlights license workload and providers with no licenses. All current providers appear, ordered by license count descending; long charts scroll rather than silently dropping providers.
- **Providers with expiring licenses** identifies actionable renewal risks, showing each provider's count and nearest expiration date, ordered by earliest expiration.

### Frontend/backend integration and business rules

React requests `GET /api/dashboard` through the shared API helper and existing Vite `/api` proxy. A dedicated C# controller calls `IDashboardService`, registered with dependency injection. The service reads SQLite through EF Core with `AsNoTracking` and the existing soft-delete query filters, excluding deleted providers and deleted licenses. It returns a dedicated response DTO containing counts, status distributions, per-provider license totals, renewal candidates, the UTC evaluation date, and the renewal window length. The dashboard uses lightweight CSS bar charts with visible labels and numeric counts, avoiding another chart dependency. React only renders API metrics and scales the bars; it does not decide validity, filter renewal candidates, or aggregate business counts.

The backend captures one UTC date per request using `TimeProvider` and reuses `ProviderResponseMapper.GetEffectiveLicenseStatus`. A license expiring today remains active; expiration dates before today are expired unless the stored status is Suspended. The renewal window includes today through 30 days ahead, inclusive, and includes only effectively Active licenses. Provider lifecycle status is independent: even Inactive/Pending providers can have licenses needing renewal. Cards count distinct affected providers, while the renewal table also shows their affected license counts. Date-only fields are returned as `YYYY-MM-DD` and displayed without browser timezone conversion.

No database schema changes or new sample records are required. The endpoint reads existing persisted data; the dashboard reloads when opened or refreshed. As with the directory, production hosting should route `/api` to the C# backend through a same-origin reverse proxy.

### Validation

Run the focused SQLite-backed tests from the repository root:

```powershell
dotnet test backend\Candidate.API.Tests\Candidate.API.Tests.csproj
```

These cover today/30-day/31-day expiration boundaries, suspended licenses, independent provider statuses, zero-license providers, soft-deleted licenses/providers, no tracking, and an empty registry. Build React with `npm run build` from `frontend\React.UI`. [Dashboard.http](backend/Candidate.API/Dashboard.http) provides a manual API request; the endpoint is also listed in development Swagger.
