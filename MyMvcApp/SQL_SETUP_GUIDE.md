# SQL Server Setup Guide for SkillHub

## What You Need for SQL Setup

### Option 1: LocalDB (Easiest - Recommended for Development) ✅

**No additional setup required!** LocalDB comes with Visual Studio.

- **Connection String** (already configured in `appsettings.json`):
  ```
  Server=(localdb)\mssqllocaldb;Database=SkillHubDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true
  ```

- **What happens**: When you run `dotnet ef database update`, it automatically creates the database if it doesn't exist.

- **No installation needed** - LocalDB is included with Visual Studio 2022.

### Option 2: SQL Server Express (Free)

1. **Download**: https://www.microsoft.com/sql-server/sql-server-downloads
2. **Install**: Run the installer and choose "Basic" installation
3. **Update Connection String** in `appsettings.json`:
   ```json
   "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=SkillHubDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
   ```
4. **Verify**: SQL Server should be running as a Windows service

### Option 3: Full SQL Server

1. **Install**: SQL Server Developer Edition (free for development)
2. **Update Connection String**:
   ```json
   "DefaultConnection": "Server=localhost;Database=SkillHubDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
   ```
   - For named instance: `Server=localhost\\INSTANCENAME;...`

### Option 4: SQL Server in Docker

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
```

Update connection string:
```json
"DefaultConnection": "Server=localhost,1433;Database=SkillHubDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true"
```

## Database Creation Process

The database is created automatically when you run migrations. You don't need to create it manually!

### Steps:

1. **Install EF Core Tools** (one-time):
   ```bash
   dotnet tool install --global dotnet-ef
   ```

2. **Create Migration**:
   ```bash
   dotnet ef migrations add InitialCreate
   ```

3. **Create Database and Tables**:
   ```bash
   dotnet ef database update
   ```

This command will:
- ✅ Create the database `SkillHubDb` (if it doesn't exist)
- ✅ Create all tables (Users, Services, Categories, Bookings, Reviews, etc.)
- ✅ Seed initial data (5 categories: Plumbing, Tutoring, Cleaning, Electrical, Carpentry)
- ✅ Create admin user (admin@skillhub.com / Admin@123)

## Database Tables Created

The following tables are automatically created:

1. **AspNetUsers** - User accounts (via Identity)
2. **AspNetRoles** - User roles (Admin, Provider, Customer)
3. **Categories** - Service categories
4. **Services** - Services offered by providers
5. **Bookings** - Customer bookings
6. **Reviews** - Customer reviews and ratings
7. **Availabilities** - Provider availability slots
8. **Notifications** - User notifications

## Verification

After running `dotnet ef database update`, you can verify:

1. **Check SQL Server Management Studio (SSMS)**:
   - Connect to your SQL Server instance
   - Expand Databases
   - You should see `SkillHubDb`

2. **Or use command line**:
   ```bash
   sqlcmd -S (localdb)\mssqllocaldb -Q "SELECT name FROM sys.databases"
   ```

## Troubleshooting

### "Cannot open database" Error

- **Solution**: Ensure SQL Server service is running
- **Check**: Services app → SQL Server (MSSQLSERVER or your instance name)

### "Login failed for user" Error

- **Solution**: Use Windows Authentication (Trusted_Connection=true)
- **Or**: Update connection string with SQL authentication:
  ```
  Server=localhost;Database=SkillHubDb;User Id=sa;Password=YourPassword;TrustServerCertificate=true
  ```

### "LocalDB instance doesn't exist" Error

- **Solution**: Install SQL Server Express or use full SQL Server
- **Or**: Create LocalDB instance manually:
  ```bash
  sqllocaldb create mssqllocaldb
  sqllocaldb start mssqllocaldb
  ```

### Migration Errors

If migrations fail:

1. Delete the `Migrations` folder
2. Delete the database (if it exists)
3. Run migrations again:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

## Connection String Format

### Windows Authentication (Recommended)
```
Server=SERVERNAME;Database=SkillHubDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true
```

### SQL Authentication
```
Server=SERVERNAME;Database=SkillHubDb;User Id=USERNAME;Password=PASSWORD;TrustServerCertificate=true
```

### Parameters Explained

- **Server**: SQL Server instance name
  - LocalDB: `(localdb)\mssqllocaldb`
  - Express: `localhost\SQLEXPRESS`
  - Default: `localhost`
  
- **Database**: Database name (SkillHubDb)
- **Trusted_Connection**: Use Windows Authentication
- **MultipleActiveResultSets**: Allow multiple queries
- **TrustServerCertificate**: Trust SSL certificate (for development)

## Quick Start (Recommended Path)

1. **Use LocalDB** (no setup needed)
2. **Run these commands**:
   ```bash
   dotnet restore
   dotnet tool install --global dotnet-ef
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   dotnet run
   ```
3. **Done!** Database is ready.

## Need Help?

- Check `SETUP.md` for detailed setup instructions
- Check `README.md` for full project documentation
- Verify SQL Server is running in Services
- Check connection string in `appsettings.json`





