# Database Verification Commands

## Using SQLite CLI

### Check if Database Exists
```powershell
cd "C:\Users\yida\Divya2026\provider-technical-assignment"
ls -Name "*.db"
```

### Connect to Database
```powershell
sqlite3 provider_assignment.db
```

### Inside SQLite Prompt - Run These Commands

#### 1. List All Tables
```sql
.tables
```
Expected output: `AuditLog License Provider`

#### 2. Count Providers
```sql
SELECT COUNT(*) as ProviderCount FROM Provider WHERE IsDeleted = 0;
```
Expected: `4`

#### 3. Show All Active Providers
```sql
SELECT ProviderId, ProviderName, County, Status FROM Provider WHERE IsDeleted = 0 ORDER BY ProviderId;
```
Expected:
```
1|Sunny Days Child Care|Fulton|Active
2|Little Stars Academy|DeKalb|Active
3|Rainbow Kids Care|Cobb|Active
4|Golden Hour Preschool|Henry|Active
```

#### 4. Count Total Licenses
```sql
SELECT COUNT(*) as LicenseCount FROM License WHERE IsDeleted = 0;
```
Expected: `8`

#### 5. Show All Licenses
```sql
SELECT LicenseId, ProviderId, LicenseNumber, LicenseStatus, ExpirationDate FROM License WHERE IsDeleted = 0 ORDER BY ProviderId, LicenseId;
```
Expected:
```
1|1|CC-FUL-2024-001|Active|[2026-01-XX]
2|1|CC-FUL-2024-002|Active|[2025-07-XX]
3|2|CC-DEK-2024-001|Active|[2025-01-XX]
4|2|CC-DEK-2024-002|Active|[2025-01-XX]
5|3|CC-COB-2024-001|Active|[2027-01-XX]
6|3|CC-COB-2024-002|Active|[2025-10-XX]
7|4|CC-HEN-2024-001|Active|[2025-07-XX]
8|4|CC-HEN-2024-002|Active|[2025-04-XX]
```

#### 6. Show Licenses for Specific Provider (e.g., Provider 1)
```sql
SELECT LicenseNumber, LicenseStatus, ExpirationDate 
FROM License 
WHERE ProviderId = 1 AND IsDeleted = 0;
```

#### 7. Exit SQLite
```
.exit
```

---

## Using Entity Framework Debug Output

### Enable EF Core Query Logging
Add to `Program.cs` in the DbContext configuration:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
		.LogTo(Console.WriteLine, LogLevel.Information)  // Add this line
);
```

Then when you run the app, EF Core SQL queries will show in the console.

---

## Database File Path
```
C:\Users\yida\Divya2026\provider-technical-assignment\provider_assignment.db
```

---

## Expected Schema

### Provider Table
```sql
CREATE TABLE Provider (
	ProviderId INTEGER PRIMARY KEY AUTOINCREMENT,
	ProviderName TEXT NOT NULL,
	County TEXT NOT NULL,
	Status TEXT NOT NULL,
	CreatedDate DATETIME NOT NULL,
	IsDeleted INTEGER NOT NULL DEFAULT 0
);
```

### License Table
```sql
CREATE TABLE License (
	LicenseId INTEGER PRIMARY KEY AUTOINCREMENT,
	ProviderId INTEGER NOT NULL,
	LicenseNumber TEXT NOT NULL,
	LicenseStatus TEXT NOT NULL,
	ExpirationDate DATETIME NOT NULL,
	CreatedDate DATETIME NOT NULL,
	IsDeleted INTEGER NOT NULL DEFAULT 0,
	FOREIGN KEY (ProviderId) REFERENCES Provider(ProviderId)
);
```

---

## Troubleshooting

### Issue: "database is locked"
- Solution: Close the web browser and stop the application, then try again

### Issue: Database file not found
- Solution: Run the application - it will recreate it automatically

### Issue: See 0 providers
- Solution: The database exists but is empty - likely the seeding didn't run
  - Delete `provider_assignment.db`
  - Restart the application
  - Check debug output for seeding messages

---

**Created**: 2026  
**Purpose**: Verify sample data in SQLite database
