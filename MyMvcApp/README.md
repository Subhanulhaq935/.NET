# SkillHub - Service Booking Platform

A comprehensive web application connecting customers with local service providers, built with ASP.NET Core MVC.

## Features

### Customer Features
- Browse and search services by category and location
- Book services with date/time selection
- View booking history
- Rate and review completed services

### Provider Features
- Create and manage service listings
- Set availability calendar
- Accept/reject booking requests
- View customer ratings and feedback

### Admin Features
- Approve/disable service providers
- Manage service categories
- View analytics dashboard with charts
- Export reports to Excel and PDF

## Tech Stack

- **Backend**: ASP.NET Core MVC (.NET 9.0)
- **Authentication**: ASP.NET Identity with Role-Based Access Control
- **Database**: SQL Server with Entity Framework Core (Code-First)
- **Frontend**: Bootstrap 5
- **Charts**: Chart.js
- **Export**: EPPlus (Excel), QuestPDF (PDF)

## SQL Server Setup Instructions

### Option 1: LocalDB (Recommended for Development)

LocalDB is included with Visual Studio and SQL Server Express. No additional setup needed!

The connection string in `appsettings.json` is already configured for LocalDB:
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SkillHubDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
```

### Option 2: SQL Server Express/Full SQL Server

1. **Install SQL Server** (if not already installed):
   - Download SQL Server Express (free): https://www.microsoft.com/sql-server/sql-server-downloads
   - Or use SQL Server Developer Edition (free for development)

2. **Update Connection String** in `appsettings.json`:
   ```json
   "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=SkillHubDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
   ```
   - Replace `localhost\\SQLEXPRESS` with your SQL Server instance name
   - For default instance: `Server=localhost;Database=SkillHubDb;...`

3. **Create Database** (optional - EF Core will create it automatically):
   ```sql
   CREATE DATABASE SkillHubDb;
   ```

### Option 3: SQL Server in Docker

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
```

Update connection string:
```json
"DefaultConnection": "Server=localhost,1433;Database=SkillHubDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true"
```

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- Visual Studio 2022 or VS Code
- SQL Server (LocalDB, Express, or Full)

### Installation Steps

1. **Clone or navigate to the project directory**

2. **Restore NuGet packages**:
   ```bash
   dotnet restore
   ```

3. **Update database connection string** in `appsettings.json` if needed (see SQL Setup above)

4. **Create and apply database migrations**:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

   If you don't have EF Core tools installed:
   ```bash
   dotnet tool install --global dotnet-ef
   ```

5. **Run the application**:
   ```bash
   dotnet run
   ```

6. **Access the application**:
   - Open browser to `https://localhost:5001` or `http://localhost:5000`

### Default Admin Account

After running the application for the first time, an admin account is automatically created:

- **Email**: `admin@skillhub.com`
- **Password**: `Admin@123`

**Important**: Change this password after first login!

## Database Schema

The application uses the following main tables:

- **Users** (via Identity) - User accounts with roles
- **Categories** - Service categories (Plumbing, Tutoring, etc.)
- **Services** - Services offered by providers
- **Bookings** - Customer bookings
- **Reviews** - Customer reviews and ratings
- **Availability** - Provider availability slots
- **Notifications** - User notifications

## User Roles

1. **Admin**: Full system access
   - Manage providers
   - Manage categories
   - View analytics
   - Export reports

2. **Provider**: Service provider account
   - Add/edit services
   - Manage bookings
   - Set availability
   - View reviews

3. **Customer**: Regular user account
   - Browse services
   - Book services
   - Leave reviews

## Project Structure

```
MyMvcApp/
├── Controllers/          # MVC Controllers
│   ├── AccountController.cs
│   ├── AdminController.cs
│   ├── CustomerController.cs
│   ├── HomeController.cs
│   └── ProviderController.cs
├── Data/                 # Database context
│   └── ApplicationDbContext.cs
├── Models/               # Data models
│   ├── ApplicationUser.cs
│   ├── Booking.cs
│   ├── Category.cs
│   ├── Review.cs
│   ├── Service.cs
│   └── ...
├── Views/                # Razor views
│   ├── Account/
│   ├── Admin/
│   ├── Customer/
│   ├── Home/
│   └── Provider/
└── wwwroot/              # Static files
```

## Features in Detail

### Booking Flow
1. Customer browses services
2. Customer selects service and date/time
3. Booking status: Pending → Provider accepts/rejects
4. After completion, customer can leave review

### Provider Approval
- New providers register with `IsApproved = false`
- Admin must approve before provider can offer services
- Customers only see services from approved providers

### Reports Export
- **Excel Export**: All bookings with details
- **PDF Export**: Formatted booking report

## Troubleshooting

### Database Connection Issues

1. **LocalDB not found**:
   - Install SQL Server Express or use full SQL Server
   - Update connection string accordingly

2. **Cannot create database**:
   - Ensure SQL Server is running
   - Check user permissions
   - Verify connection string

3. **Migration errors**:
   - Delete `Migrations` folder
   - Run `dotnet ef migrations add InitialCreate` again
   - Ensure database doesn't exist or drop it first

### Common Issues

- **"No database provider configured"**: Check `appsettings.json` connection string
- **"Login failed"**: Verify SQL Server authentication settings
- **"Migration pending"**: Run `dotnet ef database update`

## Development Notes

- The application seeds initial categories automatically
- Admin user is created on first run
- Providers need admin approval before their services are visible
- All dates/times are stored in UTC

## License

This project is for educational/demonstration purposes.

## Support

For issues or questions, please check:
- Entity Framework Core documentation
- ASP.NET Core Identity documentation
- SQL Server documentation





