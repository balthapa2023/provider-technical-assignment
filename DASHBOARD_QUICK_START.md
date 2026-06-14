# Dashboard Quick Start Guide

## 🚀 Get Started in 3 Minutes

### Step 1: Start the Application
```bash
cd C:\Users\yida\Divya2026\provider-technical-assignment\
dotnet run
```

**What happens:**
- Application starts
- Database is created (if it doesn't exist)
- Sample data is seeded (8 providers, 16 licenses)
- Application listens on https://localhost:5001

### Step 2: Open in Browser
Navigate to:
```
https://localhost:5001
```

### Step 3: Click Dashboard
Look for **Dashboard** in the navigation menu (second item after Home)

---

## ✅ What You Should See

### Immediately upon navigation to Dashboard:

**Summary Cards** (4 colored boxes):
- 🏢 Total Providers: **8**
- 🎫 Total Licenses: **16**
- ⚠️ Expiring in 30 Days: **4**
- ❌ Expired Licenses: **2**

**Charts** (3 visualizations):
1. Horizontal bar chart labeled "Providers by Status"
2. Doughnut/pie chart labeled "License Status Distribution"
3. Horizontal bar chart labeled "Active Licenses per Provider"

**Table**:
"Providers with Licenses Expiring Within 30 Days" (3 rows of data)

**Button**:
"🔄 Refresh Data" to update manually

---

## 🎯 What Each Part Shows

| Component | Shows | Example |
|-----------|-------|---------|
| **Summary Cards** | Key metrics at a glance | 8 active providers |
| **Status Chart** | Provider status breakdown | 5 Active, 1 Inactive, 2 Pending |
| **License Chart** | Active vs Expired split | 14 Active (green), 2 Expired (red) |
| **Provider Chart** | Top providers by count | Better Care: 3 licenses |
| **Expiring Table** | Needs renewal attention | Acme Health: 1 license expiring |

---

## 🔍 Verify Everything Works

### Check 1: Menu Item Visible
✓ See "Dashboard" in top menu between Home and Providers

### Check 2: Summary Cards Display
✓ 4 cards visible with numbers
✓ Should see: 8, 16, 4, 2

### Check 3: Charts Render
✓ Three distinct chart sections
✓ Charts have labels and legends
✓ Charts contain colored elements

### Check 4: Table Shows Data
✓ "Providers with Licenses Expiring..." section visible
✓ Table contains provider names and counties
✓ Red badges show expiring count

### ✅ If All Four Checks Pass
**Congratulations!** Your dashboard is working perfectly.

---

## 🆘 If Something Seems Wrong

### Dashboard Page Shows "Loading..." Forever
```
1. Open browser Developer Tools (F12)
2. Go to Console tab
3. Look for red error messages
4. Check Network tab for failed requests to /api/dashboard/all
```

### Summary Cards Show 0 Values
```
1. Delete database file:
   rm Data/provider_assignment.db*
2. Restart application
3. Database will be recreated with sample data
```

### Charts Don't Display
```
1. Check internet connection (Chart.js loads from CDN)
2. Open DevTools → F12
3. Check for CORS errors in console
4. Try clearing cache (Ctrl+Shift+Delete)
```

### Still Having Issues?
See: `Data/DASHBOARD_TROUBLESHOOTING.md` for detailed debugging

---

## 📖 Learn More

| Document | Purpose |
|----------|---------|
| `DASHBOARD_IMPLEMENTATION_SUMMARY.md` | What was built and how |
| `DASHBOARD_VISUAL_GUIDE.md` | What everything looks like |
| `Data/DASHBOARD_README.md` | Features, API, customization |
| `Data/DASHBOARD_TROUBLESHOOTING.md` | Debug and troubleshoot |

---

## 🎮 Try These Actions

### 1. Refresh Dashboard
Click the [🔄 Refresh Data] button at the bottom
- Updates all charts and cards
- Takes 1-2 seconds

### 2. Check Mobile View
Press F12 → Toggle Device Toolbar (Ctrl+Shift+M)
- Dashboard adapts to mobile/tablet sizes
- Cards stack vertically on small screens
- Charts remain readable

### 3: Create New Provider
Click "New Provider" in menu, add a provider
- Navigate back to Dashboard
- "Total Providers" card increases to 9
- Click Refresh to see updated data

### 4. Add License (Advanced)
- View provider details
- Add license with expiration date 15 days from now
- Return to Dashboard
- License appears in expiring soon metrics

---

## 💡 Key Features to Know

✓ **Read-Only** - Dashboard shows data, doesn't modify it  
✓ **Live Data** - Uses real database, not sample data  
✓ **Soft-Delete Aware** - Excludes deleted records automatically  
✓ **Responsive** - Works on phone, tablet, desktop  
✓ **Fast** - Summary cards load in < 1 second  
✓ **Reliable** - Error handling with retry button  

---

## 🔧 Under the Hood

**Backend**: ASP.NET Core API  
**Frontend**: React 18 with Chart.js  
**Database**: SQLite with 16 licenses  
**Data Updated**: When you click Refresh or reload page  

---

## 📊 Sample Data Details

The application comes pre-loaded with:

**8 Providers:**
- 5 with Active status
- 1 with Inactive status
- 2 with Pending status
- Distributed across GA counties

**16 Licenses:**
- 14 currently active
- 2 already expired
- 4 expiring within next 30 days
- Various expiration dates spread across 2024-2025

**Perfect for testing all dashboard features immediately!**

---

## ✨ What's Next?

### To Customize Dashboard
See: `Data/DASHBOARD_README.md` → Customization section

### To Add More Data
Click "New Provider" → Add Providers  
View Provider Details → Add Licenses

### To Understand Architecture
See: `DASHBOARD_IMPLEMENTATION_SUMMARY.md` → Implementation Details

### To Deploy to Production
Ensure database file is included in deployment  
Set `appsettings.json` connection string appropriately  

---

## 🎓 Dashboard API (For Developers)

### Main Endpoint
```
GET /api/dashboard/all
```

Returns all dashboard data in one call

### Other Endpoints
```
GET /api/dashboard/health                    ← Check if API works
GET /api/dashboard/providers-by-status       ← Status breakdown
GET /api/dashboard/licenses-per-provider     ← Top 10 providers
GET /api/dashboard/license-status            ← Active vs Expired
GET /api/dashboard/summary                   ← Metrics only
GET /api/dashboard/providers-expiring-soon   ← Expiring list
```

Test any endpoint in browser:
```javascript
// Open DevTools Console (F12)
fetch('/api/dashboard/health')
  .then(r => r.json())
  .then(console.log)
```

---

## ✅ Success Criteria

Your dashboard is working correctly when:

- [x] Dashboard menu item visible and clickable
- [x] Page loads without hanging
- [x] 4 summary cards display with numbers
- [x] 3 charts render with data
- [x] Table shows providers expiring soon
- [x] Refresh button updates data
- [x] No error messages in console

**All criteria met? You're done! 🎉**

---

## 📞 Quick Reference

| Need | Action |
|------|--------|
| Start app | `dotnet run` |
| Open dashboard | Click "Dashboard" menu |
| Update data | Click "Refresh Data" button |
| Remove old DB | `rm Data/provider_assignment.db*` |
| Check console (F12) | Look for errors |
| Test API | Run fetch in DevTools |
| Clear cache | Ctrl+Shift+Delete |
| Mobile view | F12 → Toggle device toolbar |
| Full page | F11 to exit fullscreen |

---

## 🏁 You're All Set!

Your Provider License Management System now has a fully functional dashboard with:
- 4 summary metric cards
- 3 interactive charts
- 1 data table
- Real-time API integration
- responsive design
- Complete documentation

**Start the app and navigate to Dashboard to see it in action!**

Questions? Check the troubleshooting guide: `Data/DASHBOARD_TROUBLESHOOTING.md`
