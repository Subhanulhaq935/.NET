# Quick Setup Guide

## Step-by-Step Setup

### 1. Install Prerequisites

- **.NET 9.0 SDK**: Download from https://dotnet.microsoft.com/download
- **SQL Server**: Use LocalDB (comes with Visual Studio) or install SQL Server Express

### 2. Restore Packages

```bash
dotnet restore
```

### 3. Install EF Core Tools (if not already installed)

```bash
dotnet tool install --global dotnet-ef
```

### 4. Create Database Migration

```bash
dotnet ef migrations add InitialCreate
```

This creates the migration files in the `Migrations` folder.

### 5. Apply Migration to Create Database

```bash
dotnet ef database update
```

This will:
- Create the database (if it doesn't exist)
- Create all tables
- Seed initial categories
- Create admin user

### 6. Run the Application

```bash
dotnet run
```

Or in Visual Studio: Press F5

### 7. Access the Application

- Open browser to: `https://localhost:5001` or `http://localhost:5000`
- Login with admin account:
  - Email: `admin@skillhub.com`
  - Password: `Admin@123`

## SQL Server Connection String Options

### LocalDB (Default - No Setup Needed)
```json
"Server=(localdb)\\mssqllocaldb;Database=SkillHubDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
```

### SQL Server Express
```json
"Server=localhost\\SQLEXPRESS;Database=SkillHubDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
```

### Full SQL Server
```json
"Server=localhost;Database=SkillHubDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
```

### SQL Server with Username/Password
```json
"Server=localhost;Database=SkillHubDb;User Id=sa;Password=YourPassword;TrustServerCertificate=true"
```

## Troubleshooting

### Migration Errors

If you get errors during migration:

1. **Delete existing database** (if it exists):
   ```sql
   DROP DATABASE SkillHubDb;
   ```

2. **Delete Migrations folder** (if needed)

3. **Recreate migration**:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

### Connection String Issues

- Verify SQL Server is running
- Check instance name (use `localhost` for default instance)
- For LocalDB, ensure it's installed (comes with Visual Studio)

### Port Already in Use

If port 5000/5001 is in use, update `Properties/launchSettings.json`:

```json
"applicationUrl": "http://localhost:5002;https://localhost:5003"
```

## Next Steps After Setup

1. **Login as Admin** and approve some providers
2. **Register as Provider** and add services
3. **Register as Customer** and book services
4. **Test the booking flow** end-to-end

## Database Reset

To reset the database completely:

```bash
dotnet ef database drop
dotnet ef database update
```

This will delete all data and recreate the database with seed data.





