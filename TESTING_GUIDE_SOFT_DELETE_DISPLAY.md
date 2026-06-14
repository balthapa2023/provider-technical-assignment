# Quick Testing Guide - Soft-Delete Display Feature

## What Was Changed
The dashboard now displays **soft-deleted records** (records where `IsDeleted = true`) alongside active records, with clear visual indicators.

## Visual Changes You'll See

### 1. Summary Cards (Top of Dashboard)
- **Total Providers: 8 (2 deleted)** - Shows deleted count in smaller text below
- **Total Licenses: 16 (4 deleted)** - Shows deleted count in smaller text below
- Expiring in 30 Days: 3
- Expired Licenses: 2

### 2. Alert Box
At the top of the dashboard:
```
ℹ️ Note: Dashboard includes soft-deleted records with visual indicators.
```

### 3. Charts
All chart titles updated to indicate deleted records included:
- "Providers by Status (Include Deleted)"
- "License Status Distribution"
- "All Licenses per Provider (Top 10, Include Deleted)"

### 4. Providers Expiring Soon Table
Shows deleted providers with:
- 🗑️ **Red trash icon** before provider name
- **(Deleted)** text suffix
- **Gray row background**

Example:
```
🗑️ Deleted Provider Name (Deleted)    |  County  |  Active  |  2 licenses
```

### 5. Legend Section
Explains the visual indicators:
- "🗑️ Trash icon indicates soft-deleted records"
- "Gray row background indicates a soft-deleted provider"

## How to Test

### Option 1: Manual Verification in Browser
1. Open application at http://localhost:5236
2. Navigate to **Dashboard** menu
3. **Expected Result:** Three charts showing data + summary cards with deleted counts

### Option 2: Test API Endpoints Directly
Using PowerShell or curl:

```powershell
# Get summary with deleted counts
Invoke-WebRequest -Uri "http://localhost:5236/api/dashboard/summary" | ConvertFrom-Json

# Expected Response:
# {
#   "totalProviders": 8,
#   "totalLicenses": 16,
#   "expiringIn30Days": 3,
#   "expiredLicenses": 2,
#   "deletedProviders": 2,        ← NEW
#   "deletedLicenses": 4           ← NEW
# }
```

```powershell
# Get all dashboard data
Invoke-WebRequest -Uri "http://localhost:5236/api/dashboard/all" | ConvertFrom-Json

# Expected Response: Full dashboard data including providers with isDeleted flag
```

```powershell
# Get providers expiring soon
Invoke-WebRequest -Uri "http://localhost:5236/api/dashboard/providers-expiring-soon" | ConvertFrom-Json

# Expected Response: Providers with expiring licenses including isDeleted field for deleted ones
```

### Option 3: Check Database Directly
Check SQLite database for soft-deleted records:
```sql
-- Count active vs deleted providers
SELECT 
	COUNT(CASE WHEN IsDeleted = 0 THEN 1 END) as ActiveCount,
	COUNT(CASE WHEN IsDeleted = 1 THEN 1 END) as DeletedCount
FROM Providers;

-- View all providers including deleted
SELECT Id, ProviderName, Status, IsDeleted, DeletedAt FROM Providers;
```

## What Didn't Change

✅ Provider CRUD operations (Create, Read, Update, Delete) - unchanged
✅ Audit Log viewer - unchanged
✅ All other pages - unchanged
✅ Database structure - unchanged
✅ Standard filters (that exclude deleted records) - unchanged

## Key Implementation Details

### Backend
- All dashboard API endpoints use `.IgnoreQueryFilters()` to include soft-deleted records
- New fields added to API responses:
  - `deletedProviders` count in summary
  - `deletedLicenses` count in summary
  - `isDeleted` flag on individual providers in tables

### Frontend (React)
- SummaryCard component enhanced with `subtext` parameter
- Table rows styled with gray background if `isDeleted = true`
- Trash icon rendered for deleted providers
- Chart labels updated to indicate "Include Deleted"

### Database Layer
- No database changes needed
- Uses Entity Framework Core's `.IgnoreQueryFilters()` method
- Preserves existing soft-delete audit trail

## Rollback (If Needed)

If you need to hide soft-deleted records again:
1. Remove `.IgnoreQueryFilters()` from each endpoint
2. Add back `.Where(p => !p.IsDeleted)` filters
3. Remove deleted count fields from API responses
4. Remove `isDeleted` flags and visual indicators from React components

## Performance Considerations

- Dashboard now queries **both** active and deleted records
- Slight increase in data returned from API (should be minimal)
- No indexing changes needed
- Recommended: Monitor query performance if dataset grows large

## Browser Compatibility

✅ All modern browsers (Chrome, Firefox, Safari, Edge)
✅ Requires JavaScript enabled
✅ Uses Bootstrap 5 and Bootstrap Icons
✅ Responsive design (works on desktop, tablet, mobile)

## Common Questions

**Q: Will deleted records affect standard views (Provider list, etc.)?**
A: No, only the dashboard is updated. Standard views continue to use default filters.

**Q: Can users restore deleted records from the dashboard?**
A: Not in current version. Users must use direct database restoration or API calls.

**Q: Where are soft-deleted records stored?**
A: In the same database tables with `IsDeleted = true` flag.

**Q: Why keep deleted records instead of removing them?**
A: For audit compliance and data recovery purposes.

**Q: Can I toggle between "show all" and "show active only"?**
A: Not in current version. Future enhancement planned.

## Files Modified

1. **Controllers/DashboardController.cs**
   - Modified: 6 endpoints (GetProvidersByStatus, GetLicensesPerProvider, GetLicenseStatus, GetSummary, GetProvidersExpiringSoon, GetAllDashboardData)
   - Added: `.IgnoreQueryFilters()` to query methods

2. **wwwroot/js/dashboard-app.js**
   - Recreated entire file
   - Updated: SummaryCard component signature
   - Added: Soft-delete visual indicators (trash icon, gray rows)
   - Updated: Chart titles and table rendering
   - Added: Legend explanation section

## Build Status
✅ Build successful - all changes compile without errors

## Next Steps
1. Test dashboard in browser
2. Verify API endpoints return expected data
3. Check that deleted records are visible with indicators
4. Test performance with sample data
5. Consider future enhancements (toggle view, restoration, etc.)
