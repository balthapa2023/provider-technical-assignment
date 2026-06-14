# Soft Delete Display Updates - Dashboard Implementation

## Overview
The dashboard has been successfully updated to display soft-deleted records alongside active records. All dashboard views now include soft-deleted providers and licenses with clear visual indicators.

## Changes Made

### 1. Backend API Updates (`Controllers/DashboardController.cs`)

All endpoint queries have been modified to use `IgnoreQueryFilters()` to bypass the global soft-delete filter:

#### Modified Endpoints:

1. **GET `/api/dashboard/providers-by-status`**
   - Now includes soft-deleted providers in the status grouping
   - Uses: `.IgnoreQueryFilters()` on Providers
   - Label: "Providers by Status (Include Deleted)"

2. **GET `/api/dashboard/licenses-per-provider`**
   - Now includes soft-deleted providers and their licenses
   - Providers with "(Deleted)" suffix in labels for soft-deleted records
   - Uses: `.IgnoreQueryFilters()` on Providers
   - Label: "All Licenses per Provider (Include Deleted)"

3. **GET `/api/dashboard/license-status`**
   - Now includes soft-deleted licenses in active/expired counts
   - Uses: `.IgnoreQueryFilters()` on Licenses
   - Label: "License Status Distribution"

4. **GET `/api/dashboard/summary`**
   - Returns new fields:
	 - `deletedProviders` - count of soft-deleted providers
	 - `deletedLicenses` - count of soft-deleted licenses
   - Uses: `.IgnoreQueryFilters()` on both Providers and Licenses

5. **GET `/api/dashboard/providers-expiring-soon`**
   - Now includes soft-deleted providers with expiring licenses
   - Returns new field: `isDeleted` flag for each provider
   - Uses: `.IgnoreQueryFilters()` on Providers
   - Label: "Providers with Licenses Expiring Within 30 Days (Include Deleted)"

6. **GET `/api/dashboard/all`**
   - Comprehensive endpoint returning all dashboard data
   - Includes deleted record counts in summary
   - Uses: `.IgnoreQueryFilters()` throughout

### 2. Frontend React App Updates (`wwwroot/js/dashboard-app.js`)

#### SummaryCard Component Enhancement
- Added optional `subtext` parameter to display deleted record counts
- Format: "(X deleted)" displayed below main metric value
- Updated signature:
  ```javascript
  function SummaryCard({ icon, title, value, color, subtext })
  ```

#### Summary Cards Section
- Total Providers card shows deleted provider count as subtext
- Total Licenses card shows deleted license count as subtext
- Example display: "Total Providers: 8 (2 deleted)"

#### Soft-Delete Indicators in Tables
Providers expiring soon table now displays:
- 🗑️ Trash icon in red for soft-deleted providers
- "(Deleted)" text suffix after provider name
- Gray row background for soft-deleted records

#### Updated Chart Labels
All charts now clearly indicate soft-deleted record inclusion:
- "Providers by Status (Include Deleted)"
- "License Status Distribution"
- "All Licenses per Provider (Top 10, Include Deleted)"
- "Providers with Licenses Expiring Within 30 Days (Include Deleted)"

#### Legend Section
Added informational legend explaining:
- Trash icon indicates soft-deleted records
- Gray row background indicates soft-deleted provider
- Soft-deleted records are kept for audit compliance

### 3. API Response Structure

#### Providers by Status
```json
{
  "labels": ["Active", "Inactive", "Pending"],
  "datasets": [{
	"label": "Providers by Status (Include Deleted)",
	"data": [5, 1, 2]
  }]
}
```

#### Summary
```json
{
  "totalProviders": 8,
  "totalLicenses": 16,
  "expiringIn30Days": 3,
  "expiredLicenses": 2,
  "deletedProviders": 2,
  "deletedLicenses": 4
}
```

#### Providers Expiring Soon
```json
[
  {
	"providerName": "Provider Name",
	"county": "County Name",
	"status": "Active",
	"isDeleted": false,
	"expiringLicenseCount": 1
  },
  {
	"providerName": "Deleted Provider",
	"county": "County Name",
	"status": "Active",
	"isDeleted": true,
	"expiringLicenseCount": 2
  }
]
```

## User Interface Features

### Visual Indicators
- **Trash Icon** (🗑️ in red): Indicates soft-deleted records
- **Gray Row**: Soft-deleted provider rows in tables
- **(Deleted)** suffix: Added to provider names in charts and tables
- **Colored Badges**: Status badges (Active/Inactive/Pending) remain unchanged

### Dashboard Sections

1. **Summary Cards** (Top Section)
   - Total Providers (with deleted count)
   - Total Licenses (with deleted count)
   - Expiring in 30 Days
   - Expired Licenses

2. **Charts** (Middle Sections)
   - Providers by Status (includes deletions)
   - License Status Distribution (includes deletions)
   - All Licenses per Provider (includes deletions)

3. **Tables** (Bottom Sections)
   - Providers with Licenses Expiring Within 30 Days
   - Displays deleted indicators and counts
   - Editable/clickable provider names

4. **Information Elements**
   - Alert notification: "Dashboard includes soft-deleted records with visual indicators"
   - Legend section explaining indicators
   - Refresh Data button for manual updates

## Technical Implementation Details

### EF Core Method: IgnoreQueryFilters()
- Bypasses global soft-delete filters configured in `DbContext.OnModelCreating()`
- Allows querying both active and soft-deleted records simultaneously
- Applied at query level, not collection level (for performance)

### Query Patterns Used
```csharp
// Query soft-deleted records
var allProviders = await _context.Providers
	.IgnoreQueryFilters()
	.ToListAsync();

// Group including soft-deleted
var grouped = await _context.Providers
	.IgnoreQueryFilters()
	.GroupBy(p => p.Status)
	.ToListAsync();

// Count soft-deleted in navigation properties
var providerWithAllLicenses = providers.Select(p => new {
	licenseCount = p.Licenses.Count() // Includes soft-deleted licenses
});
```

## Database Configuration (Unchanged)
- Soft-delete implementation via `OnModelCreating()` in `AppDbContext.cs`
- Global query filter: `modelBuilder.Entity<Provider>().HasQueryFilter(p => !p.IsDeleted)`
- Bypassed with `.IgnoreQueryFilters()` in dashboard endpoints

## Build & Deployment

### Build Status: ✅ SUCCESSFUL
- No compilation errors
- All endpoints compile correctly
- React component updates verified

### Testing Endpoints
Test these API endpoints to verify soft-deleted record inclusion:

1. **Summary Metrics:**
   ```
   GET /api/dashboard/summary
   ```
   - Verify `deletedProviders` and `deletedLicenses` counts

2. **Providers by Status:**
   ```
   GET /api/dashboard/providers-by-status
   ```
   - Verify counts include soft-deleted providers

3. **All Dashboard Data:**
   ```
   GET /api/dashboard/all
   ```
   - Comprehensive test of all data with soft-deleted records

4. **Providers Expiring Soon:**
   ```
   GET /api/dashboard/providers-expiring-soon
   ```
   - Verify `isDeleted` flag present for deleted providers

## Audit & Compliance

- Soft-deleted records are **preserved** for audit compliance
- Dashboard provides **comprehensive visibility** of all records
- **Visual distinction** between active and deleted records
- **Deleted record counts** tracked separately
- **Audit trail** maintained via `DeletedAt` timestamp

## Future Enhancements (Optional)

1. **Toggle View Button:**
   - Switch between "Active Only" and "All Records" views
   - User preference persistence

2. **Advanced Filtering:**
   - Filter by deletion date range
   - Show records deleted in last X days

3. **Restoration UI:**
   - Restore soft-deleted records from dashboard
   - Bulk restoration actions

4. **Audit Detail View:**
   - Show who deleted a record and when
   - Access to DeletedAt timestamp for all records

5. **Export Reports:**
   - Export with soft-deleted records visible
   - CSV/PDF export with deletion indicators

## Support & Troubleshooting

### If Charts Don't Display Deleted Records
1. Verify database has soft-deleted records (IsDeleted = 1)
2. Clear browser cache (Ctrl+Shift+Del)
3. Refresh dashboard page (F5 or Ctrl+R)
4. Check browser console for JavaScript errors (F12)

### If API Returns Unexpected Counts
1. Check database directly for IsDeleted flag values
2. Verify `IgnoreQueryFilters()` is applied correctly
3. Run build again to ensure latest code deployed

### If Soft-Delete Indicators Not Showing
1. Clear browser cache
2. Verify React components loaded correctly (check Network tab in DevTools)
3. Check that provider data includes `isDeleted` field in response

## Related Files
- `Controllers/DashboardController.cs` - API endpoints with soft-delete queries
- `wwwroot/js/dashboard-app.js` - React dashboard UI component
- `Data/AppDbContext.cs` - Entity Framework configuration for soft-delete
- `Views/Dashboard/Index.cshtml` - Razor view hosting React app
- `Data/DbInitializer.cs` - Sample data seeding

## Completed By
AI Assistant (GitHub Copilot)
Date: 2024
Task: Display soft-deleted records in dashboard with visual indicators
