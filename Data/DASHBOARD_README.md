# Dashboard Implementation Guide

## Overview
The Dashboard provides read-only analytics and insights for your Provider and License management system. It displays real-time data with interactive charts and summary metrics.

## Features

### 1. Summary Cards
Display key metrics:
- **Total Providers**: Count of all active (non-deleted) providers
- **Total Licenses**: Count of all active (non-deleted) licenses
- **Expiring in 30 Days**: Licenses expiring within the next 30 days
- **Expired Licenses**: Count of licenses that have already expired

### 2. Charts
- **Providers by Status**: Horizontal bar chart showing providers grouped by status (Active, Inactive, Pending)
- **License Status Distribution**: Doughnut chart showing the split between Active and Expired licenses
- **Active Licenses per Provider**: Top 10 providers with the most licenses (horizontal bar chart)

### 3. Providers Expiring Soon Table
Interactive table showing:
- Provider Name
- County
- Status (with color-coded badges)
- Number of licenses expiring within 30 days

## Technology Stack

### Backend
- **ASP.NET Core 8 MVC**
- **Entity Framework Core** with SQLite
- **RESTful API** endpoints serving JSON data

### Frontend
- **React 18** (via CDN - no build process required)
- **Chart.js 4.4** for data visualization
- **Bootstrap 5** for responsive design
- **Bootstrap Icons** for UI elements

## API Endpoints

All endpoints are located under `/api/dashboard/`:

### `/api/dashboard/all` (Recommended)
Returns all dashboard data in a single request for better performance.

**Response Structure:**
```json
{
  "providersByStatus": {
	"labels": ["Active", "Inactive", "Pending"],
	"datasets": [...]
  },
  "licensesPerProvider": {
	"labels": ["Provider A", "Provider B", ...],
	"datasets": [...]
  },
  "licenseStatus": {
	"labels": ["Active", "Expired"],
	"datasets": [...]
  },
  "summary": {
	"totalProviders": 8,
	"totalLicenses": 16,
	"expiringIn30Days": 4,
	"expiredLicenses": 2
  },
  "providersExpiringSoon": [
	{
	  "providerName": "Acme Health Services",
	  "county": "Fulton",
	  "status": "Active",
	  "expiringLicenseCount": 1
	}
  ]
}
```

### Individual Endpoints
- `/api/dashboard/providers-by-status` - Providers grouped by status
- `/api/dashboard/licenses-per-provider` - Top 10 providers by license count
- `/api/dashboard/license-status` - Active vs expired license counts
- `/api/dashboard/summary` - Summary metrics only
- `/api/dashboard/providers-expiring-soon` - Providers with expiring licenses
- `/api/dashboard/health` - Health check endpoint

## How It Works

### Data Flow

1. **Page Load**: User navigates to Dashboard via menu
2. **View Rendered**: `Views/Dashboard/Index.cshtml` loads
3. **React Initialize**: React 18 initializes the dashboard app
4. **API Call**: JavaScript fetches data from `/api/dashboard/all`
5. **Backend Query**: DashboardController queries database
   - Filters out soft-deleted records using EF Core global filters
   - Aggregates data by status, provider, and expiration date
6. **Charts Rendered**: Chart.js renders visualizations
7. **Display Complete**: Dashboard shows all metrics and charts

### Soft-Delete Filtering
All dashboard queries automatically exclude soft-deleted records via EF Core's `HasQueryFilter` configuration on `Provider` and `License` entities.

## Sample Data

The application seeds the database with sample data on first run:
- **8 Providers** with various statuses and counties
- **16 Licenses** with:
  - Licenses already expired
  - Licenses expiring within 30 days
  - Active licenses with future expiration dates

This allows the dashboard to display meaningful data immediately after startup.

### Data Seeding Process
1. `Program.cs` calls `DbInitializer.Initialize()` on startup
2. Check if Providers table exists and has data
3. If empty, insert sample providers and licenses
4. Sample data includes realistic scenarios:
   - Multiple providers per license
   - Mix of active and expired licenses
   - Licenses at various expiration timeframes

## Debugging

### Check API Health
Open your browser console and run:
```javascript
fetch('/api/dashboard/health').then(r => r.json()).then(console.log)
```

Expected response:
```json
{
  "status": "healthy",
  "timestamp": "2024-12-06T...",
  "providerCount": 8,
  "licenseCount": 16
}
```

### Check Full Data
```javascript
fetch('/api/dashboard/all').then(r => r.json()).then(console.log)
```

### Browser Console
Open Developer Tools (F12) → Console tab:
- Look for "Fetching dashboard data" message
- Check for error messages
- Verify API response with "Dashboard data received"

## Customization

### Adding New Charts
1. Create new endpoint in `DashboardController.cs`
2. Add canvas element in dashboard HTML
3. Add chart initialization code in `initializeCharts()` function

### Changing Colors
Modify the `backgroundColor` and `borderColor` arrays in:
- `DashboardController.cs` (API response colors)
- `wwwroot/js/dashboard-app.js` (Chart.js color arrays)

### Adjusting Thresholds
Change the 30-day threshold in `DashboardController.cs`:
```csharp
var thirtyDaysFromNow = today.AddDays(30); // ← Modify this value
```

## Performance Considerations

- **Single API Call**: Uses `/all` endpoint to fetch all data at once (better than 5 separate calls)
- **Database Indexing**: Licenses.ProviderId is indexed for fast queries
- **Async/Await**: All database operations use async patterns
- **Client-Side Rendering**: Charts rendered in browser, not on server

## Known Limitations

- Dashboard is **read-only** (no data modification)
- Charts don't auto-refresh (use Refresh button or manual page reload)
- Provider and License navigation properties need explicit loading for certain queries

## Troubleshooting

### Dashboard Shows "Loading..." Forever
- Check browser console for errors (F12)
- Verify API endpoints are accessible: `/api/dashboard/health`
- Ensure database is initialized with seed data

### Error: "No providers with licenses expiring..."
- This is normal if no licenses are set to expire within 30 days
- Create test licenses with near-future expiration dates

### Charts Not Displaying
- Verify Chart.js CDN is loaded (check Network tab in DevTools)
- Check that canvas elements have IDs: `providersByStatusChart`, `licenseStatusChart`, `licensesPerProviderChart`
- Ensure data contains non-empty datasets

## Files Modified/Created

### New Files
- `Controllers/DashboardController.cs` - API endpoints
- `Controllers/DashboardViewController.cs` - MVC view controller
- `Views/Dashboard/Index.cshtml` - Dashboard view
- `wwwroot/js/dashboard-app.js` - React dashboard app
- `Data/DbInitializer.cs` - Sample data seeding

### Modified Files
- `Program.cs` - Added database initialization
- `Views/Shared/_Layout.cshtml` - Added Dashboard menu item

## Future Enhancements

- [ ] Real-time data updates using SignalR
- [ ] Export dashboard data to PDF/Excel
- [ ] Custom date range filtering
- [ ] Provider-specific license dashboard
- [ ] License renewal reminders
- [ ] Advanced filtering and search
- [ ] User preferences for favorite metrics
- [ ] Mobile-optimized layouts
