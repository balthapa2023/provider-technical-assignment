# Dashboard Troubleshooting & Verification

## Quick Verification Checklist

Run through these steps to verify your dashboard is working correctly:

### ✅ Step 1: Start the Application
```bash
dotnet run
```

Expected: Application starts without errors

### ✅ Step 2: Verify Menu Item Appears
- Navigate to http://localhost:5000 (or appropriate port)
- Check that **Dashboard** appears in the navigation menu
- Expected: Menu shows: Home | Dashboard | Providers | New Provider | Audit Log | Privacy

### ✅ Step 3: Click Dashboard Link
- Click on "Dashboard" in the navigation menu
- Expected: Page loads with "Dashboard - Provider and License Insights" heading
- Expected: See 4 summary cards appearing

### ✅ Step 4: Verify API Endpoint
Open browser Developer Tools (F12):

**Console Tab - Run:**
```javascript
fetch('/api/dashboard/health')
  .then(r => r.json())
  .then(data => console.log('API Health:', data))
  .catch(e => console.error('API Error:', e))
```

**Expected Output:**
```
API Health: {
  status: 'healthy',
  timestamp: '2024-12-06T...',
  providerCount: 8,
  licenseCount: 16
}
```

If you see `providerCount: 0` or `licenseCount: 0`, the database wasn't seeded properly.

### ✅ Step 5: Verify Dashboard Data
**Console Tab - Run:**
```javascript
fetch('/api/dashboard/all')
  .then(r => r.json())
  .then(data => {
	console.log('Summary:', data.summary);
	console.log('Providers by Status:', data.providersByStatus);
	console.log('Providers Expiring Soon:', data.providersExpiringSoon);
  })
  .catch(e => console.error('Error:', e))
```

**Expected:** See data objects with provider and license counts

### ✅ Step 6: Verify Summary Cards Display
Look for 4 cards:
- **Total Providers** with building icon
- **Total Licenses** with ticket icon  
- **Expiring in 30 Days** with warning icon
- **Expired Licenses** with x icon

### ✅ Step 7: Verify Charts Display
Look for 3 chart sections:
- **Providers by Status** (horizontal bar chart)
- **License Status Distribution** (doughnut chart)
- **Active Licenses per Provider** (horizontal bar chart)

### ✅ Step 8: Verify Data Table
Look for **"Providers with Licenses Expiring Within 30 Days"** table with columns:
- Provider Name
- County
- Status
- Expiring Soon


## Common Issues & Solutions

### Issue 1: Dashboard Page Loads but Shows "Loading..." Forever

**Cause:** API call failing silently

**Solution:**
1. Open DevTools Console (F12)
2. Look for error messages in red
3. Check Network tab → look for failed requests to `/api/dashboard/all`
4. Verify backend is running and accessible

**Debug Commands:**
```javascript
// Check if React is loaded
console.log(typeof React)  // Should print "object"

// Check if Chart.js is loaded
console.log(typeof Chart)  // Should print "function"

// Try manual API call
fetch('/api/dashboard/all')
  .then(r => {
	console.log('Status:', r.status);
	return r.json();
  })
  .then(data => console.log('Data:', data))
  .catch(e => console.error('Error:', e))
```

### Issue 2: Summary Cards Show 0 or Incorrect Numbers

**Cause:** Database wasn't seeded with sample data

**Solution:**
1. Delete database files:
   ```bash
   rm Data/provider_assignment.db*
   ```
2. Restart application
3. Database will be recreated and seeded automatically

**Verify Seeding:**
```javascript
fetch('/api/dashboard/health')
  .then(r => r.json())
  .then(console.log)
```
Should show `providerCount: 8` and `licenseCount: 16`

### Issue 3: Charts Don't Display (Just Canvas Elements Visible)

**Cause 1:** Chart.js library didn't load from CDN

**Debug:**
```javascript
console.log(typeof Chart)  // Should be "function"
console.log(window.Chart)  // Should exist
```

**Solution:**
- Verify internet connection (Chart.js loads from CDN)
- Check browser console for CORS errors
- Try refreshing the page
- Clear browser cache (Ctrl+Shift+Delete)

**Cause 2:** Canvas IDs don't match initialization code

**Solution:**
- Open DevTools → Elements tab
- Search for `<canvas id="providersByStatusChart">`
- Verify IDs match in `wwwroot/js/dashboard-app.js`

### Issue 4: "No providers with licenses expiring within 30 days" Message

**Cause:** No sample data meets the expiration criteria

**This is NOT an error** - it means:
- All licenses either already expired or expiring after 30 days
- The dashboard is working correctly
- The table is just empty

**To Create Data That Shows:**
- Add a provider manually via "New Provider"
- Add a license via database or API
- Set license expiration to 15 days from today

### Issue 5: API Returns 404 or 500 Error

**Debug:**
1. Check DevTools Network tab for `/api/dashboard/all`
2. Click the request to see Response tab
3. Look for error message

**Common Causes:**
- Route not registered
- DbContext not initialized
- Exception in controller code

**Solution:**
- Rebuild solution: `dotnet build`
- Restart application: `dotnet run`
- Check application logs for stack trace

## Database Verification

### Check if Database is Seeded

**Using SQLite Command Line:**
```bash
sqlite3 Data/provider_assignment.db
```

**Then run:**
```sql
SELECT COUNT(*) FROM Providers;
SELECT COUNT(*) FROM Licenses;
```

Expected: `8` and `16` respectively

### Manually Insert Test Data

```sql
INSERT INTO Providers (ProviderName, County, Status, CreatedDate, IsDeleted)
VALUES ('Test Provider', 'Fulton', 'Active', datetime('now'), 0);

INSERT INTO Licenses (ProviderId, LicenseNumber, LicenseStatus, ExpirationDate, CreatedDate, IsDeleted)
VALUES (1, 'TEST-001', 'Active', date('now', '+20 days'), datetime('now'), 0);
```

## Network Inspection

### Verify API Responses

**In Browser DevTools:**
1. Open Network tab (F12 → Network)
2. Navigate to Dashboard
3. Look for request to `all` (from `/api/dashboard/all`)
4. Click it and check Response tab
5. Verify JSON structure matches expected format

### Check Response Headers

Look for:
```
Content-Type: application/json
Content-Length: [reasonable size, not 0]
Status: 200 OK
```

## React Component Debugging

### Check React Error Boundary

If component crashes, you'll see error message in Dashboard.

**To debug:**
```javascript
// Check if app component mounted
console.log(document.getElementById('root').children.length)  // Should be > 0

// Check React fiber tree
console.log(document.getElementById('root')._reactRootContainer)
```

## Chart.js Debugging

### Verify Chart Instances

```javascript
// After dashboard loads, check if charts initialized
console.log(Chart.helpers.getCanvas(document.getElementById('providersByStatusChart')))

// Get current chart instance
const canvases = document.querySelectorAll('canvas');
console.log(`Number of charts rendered: ${canvases.length}`);  // Should be 3
```

## Performance Metrics

### Check Load Time

```javascript
// Measure API call duration
const start = performance.now();
fetch('/api/dashboard/all')
  .then(r => r.json())
  .then(() => {
	const end = performance.now();
	console.log(`API call took ${(end - start).toFixed(2)}ms`);
  });
```

Expected: < 500ms for 8 providers and 16 licenses

## Browser Compatibility

Dashboard verified to work on:
- ✅ Chrome 120+
- ✅ Firefox 121+
- ✅ Safari 17+
- ✅ Edge 120+

## Still Having Issues?

1. **Collect Information:**
   - Browser and version
   - Error messages from console
   - Screenshots
   - Steps to reproduce

2. **Check Logs:**
   ```bash
   # Application shows errors during startup
   dotnet run
   ```

3. **Verify Files Exist:**
   ```bash
   # Check dashboard files
   ls Controllers/DashboardController.cs
   ls Views/Dashboard/Index.cshtml
   ls wwwroot/js/dashboard-app.js
   ```

4. **Rebuild Everything:**
   ```bash
   dotnet clean
   dotnet build
   dotnet run
   ```

5. **Reset Database:**
   ```bash
   rm Data/provider_assignment.db*
   # Restart app - database will be recreated
   ```
