# ⚠️ How to See the 4 Providers

## Problem
You may not see the 4 providers in the table because:
- The old database file was deleted
- The application needs to be restarted to recreate the database with new seed data

## Solution: Restart the Application

### Step 1: Stop the Running Application
- Stop the application in Visual Studio
- Or press `Ctrl+Shift+F5` to hot reload
- Or close the browser and stop the debugger

### Step 2: Clean and Rebuild
```powershell
cd C:\Users\yida\Divya2026\provider-technical-assignment
dotnet clean
dotnet build
```

### Step 3: Run the Application
```powershell
dotnet run
# OR press F5 in Visual Studio
```

### Step 4: Navigate to Providers
- Open: `https://localhost:5001/providers` (or the port shown in your console)
- You should now see **4 providers** in the table:

| Provider Name | County | Status | Licenses |
|---|---|---|---|
| Sunny Days Child Care | Fulton | Active | 2 |
| Little Stars Academy | DeKalb | Active | 2 |
| Rainbow Kids Care | Cobb | Active | 2 |
| Golden Hour Preschool | Henry | Active | 2 |

---

## What Happens on App Start

1. ✅ Database migrations run automatically
2. ✅ Tables are created (Provider, License, AuditLog)
3. ✅ DbInitializer.Initialize() seeds 4 providers + 8 licenses
4. ✅ Table displays all active providers

---

## If You Still Don't See Data

### Check 1: Verify Database Was Created
```powershell
Get-ChildItem -Path "C:\Users\yida\Divya2026\provider-technical-assignment" -Name "*.db"
# Should show: provider_assignment.db
```

### Check 2: Check Application Logs
Look in Visual Studio **Output** window for:
- "Database migrations applied successfully."
- "Database initialized with sample data."

### Check 3: Delete Database and Restart Again
```powershell
Remove-Item -Path "C:\Users\yida\Divya2026\provider-technical-assignment\provider_assignment.db" -Force
# Then restart the application
```

### Check 4: Verify Table Not Filtered
- Make sure you're viewing **active** providers (not the "View Deleted Records")
- The Index page shows only `IsDeleted = false` records

---

## Database File Details

**Location**: `C:\Users\yida\Divya2026\provider-technical-assignment\provider_assignment.db`

**Data Inside**:
- **Providers Table**: 4 rows
  - Sunny Days Child Care (Fulton, Active)
  - Little Stars Academy (DeKalb, Active)
  - Rainbow Kids Care (Cobb, Active)
  - Golden Hour Preschool (Henry, Active)

- **Licenses Table**: 8 rows (2 per provider, all Active)
  - Each provider has 2 active licenses with different expiration dates

---

## Next Steps After Data Appears

✅ Click **[View]** on any provider to see details + license sidebar  
✅ Click **[Edit]** to change provider information  
✅ Click **[Delete]** to soft-delete a provider (moves to View Deleted Records)  
✅ Click county dropdown to see all 159 Georgia counties  
✅ Click **[View Deleted Records]** to see soft-deleted items and Restore them  

---

**Status**: Ready to see sample data after restart! 🎉
