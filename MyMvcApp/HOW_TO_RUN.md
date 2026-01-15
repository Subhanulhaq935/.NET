# How to Run SkillHub Application

## Quick Start Guide

### Step 1: Open Terminal/Command Prompt

Navigate to your project directory:
```bash
cd C:\Users\HP\Desktop\MyMvcApp
```

### Step 2: Restore NuGet Packages

```bash
dotnet restore
```

This will download all required packages (Dapper, Identity, etc.)

### Step 3: Install EF Core Tools (if not already installed)

```bash
dotnet tool install --global dotnet-ef
```

**Note**: If you get an error saying it's already installed, that's fine - skip this step.

### Step 4: Create Database Migration

```bash
dotnet ef migrations add InitialCreate
```

This creates the migration files that will set up your database schema.

### Step 5: Create the Database

```bash
dotnet ef database update
```

This will:
- Create the database `SkillHubDB` in LocalDB
- Create all tables (Users, Services, Categories, Bookings, Reviews, etc.)
- Set up Identity tables

### Step 6: Run the Application

```bash
dotnet run
```

Or if you're using Visual Studio:
- Press **F5** or click the **Run** button

### Step 7: Access the Application

Open your browser and go to:
- **HTTPS**: `https://localhost:5001` (or the port shown in terminal)
- **HTTP**: `http://localhost:5000` (or the port shown in terminal)

The terminal will show you the exact URLs when the app starts.

---

## Default Login Credentials

After running, the application automatically seeds these users:

| Role | Email | Password |
|------|-------|----------|
| **Admin** | admin@skillhub.com | Admin@123 |
| **Provider** | provider@skillhub.com | Provider@123 |
| **Customer** | customer@skillhub.com | Customer@123 |

---

## Troubleshooting

### Error: "Cannot open database"

**Solution**: Make sure LocalDB is running. Try:
```bash
sqllocaldb start mssqllocaldb
```

### Error: "dotnet-ef command not found"

**Solution**: Install EF Core tools:
```bash
dotnet tool install --global dotnet-ef
```

Then close and reopen your terminal.

### Error: "Migration already exists"

**Solution**: If you need to recreate migrations:
```bash
# Delete the Migrations folder first
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Error: "Connection string not found"

**Solution**: Check that `appsettings.json` has the connection string. It should be:
```json
"DefaultConnection": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SkillHubDB;..."
```

### Port Already in Use

If port 5000/5001 is in use, the app will automatically use another port. Check the terminal output for the actual URL.

---

## What Happens on First Run?

1. ✅ Database is created automatically
2. ✅ All tables are created
3. ✅ Roles are seeded (Admin, Provider, Customer)
4. ✅ Default users are created
5. ✅ Categories are seeded (8 categories)
6. ✅ Sample services are created (3 services)

---

## Running in Visual Studio

1. Open `MyMvcApp.sln` in Visual Studio
2. Press **F5** or click the green **Run** button
3. The browser will open automatically

---

## Running from Command Line

```bash
# Navigate to project folder
cd C:\Users\HP\Desktop\MyMvcApp

# Restore packages (first time only)
dotnet restore

# Create database (first time only)
dotnet ef migrations add InitialCreate
dotnet ef database update

# Run the application
dotnet run
```

---

## Verify It's Working

1. Open browser to `https://localhost:5001`
2. You should see the SkillHub home page
3. Click "Register" or "Login"
4. Try logging in with: `admin@skillhub.com` / `Admin@123`
5. You should see the Admin Dashboard

---

## Next Steps After Running

1. **Login as Admin**: Manage providers and categories
2. **Login as Provider**: Add services and manage bookings
3. **Login as Customer**: Browse services and make bookings
4. **Test Features**: Try booking a service, leaving reviews, etc.

---

## Need Help?

- Check the terminal/console for error messages
- Verify SQL Server LocalDB is installed
- Make sure .NET 9.0 SDK is installed
- Check `appsettings.json` connection string





