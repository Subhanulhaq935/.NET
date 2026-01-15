# Expense Tracker Web Application

A complete, full-fledged Expense Tracker Web Application built with **ASP.NET Core 8 MVC** and **MongoDB**, featuring cookie-based authentication, clean layered architecture, and a responsive Bootstrap UI.

## 🚀 Technology Stack

- **ASP.NET Core 8 MVC** - Web framework
- **MongoDB** - NoSQL database
- **MongoDB.Driver 2.28.0** - MongoDB client library
- **BCrypt.Net-Next 4.0.3** - Password hashing
- **Bootstrap 5** - UI framework
- **Cookie-based Authentication** - Session management
- **Razor Views** - Server-side rendering

## 📁 Project Structure

```
ExpenseTracker
├── Controllers/          # MVC Controllers (thin layer, no business logic)
│   ├── AuthController.cs
│   ├── ExpensesController.cs
│   ├── HomeController.cs
│   └── ReportsController.cs
├── Models/              # MongoDB entity models
│   ├── User.cs
│   └── Expense.cs
├── DTOs/                # Data Transfer Objects (ViewModels)
│   ├── Auth/
│   ├── Expense/
│   └── Report/
├── Services/            # Business Logic Layer
│   ├── AuthService.cs
│   ├── ExpenseService.cs
│   └── ReportService.cs
├── Repositories/        # Data Access Layer
│   ├── IUserRepository.cs
│   ├── UserRepository.cs
│   ├── IExpenseRepository.cs
│   └── ExpenseRepository.cs
├── Helpers/             # Utility classes
│   └── PasswordHelper.cs
├── Settings/            # Configuration classes
│   └── MongoDbSettings.cs
├── Views/               # Razor Views
│   ├── Auth/
│   ├── Expense/
│   ├── Home/
│   ├── Reports/
│   └── Shared/
└── Program.cs           # Application entry point
```

## 🏗️ Architecture

The application follows **clean layered architecture** with strict separation of concerns:

1. **Controllers Layer**: Handles HTTP requests, returns Views, performs basic validation
2. **Services Layer**: Contains ALL business logic (email uniqueness, amount validation, etc.)
3. **Repositories Layer**: Handles database operations with MongoDB
4. **Models Layer**: Represents MongoDB collections
5. **DTOs Layer**: Data Transfer Objects for form submission and API responses

### Key Principles:

- ✅ **No business logic in controllers** - All business rules are in Services
- ✅ **Thin controllers** - Controllers only handle HTTP concerns
- ✅ **DTOs for all data transfer** - Database models never exposed directly
- ✅ **Dependency Injection** - All dependencies injected through constructor
- ✅ **Async/await throughout** - Non-blocking operations

## 🗄️ Database Design (MongoDB)

### Users Collection
```json
{
  "_id": ObjectId,
  "email": "string (unique, indexed)",
  "passwordHash": "string (BCrypt hashed)",
  "fullName": "string",
  "createdAt": "DateTime"
}
```

### Expenses Collection
```json
{
  "_id": ObjectId,
  "userId": "ObjectId (indexed)",
  "title": "string",
  "amount": "decimal (must be > 0)",
  "category": "string",
  "date": "DateTime",
  "notes": "string (optional)",
  "createdAt": "DateTime",
  "updatedAt": "DateTime (optional)"
}
```

## 🔐 Authentication & Authorization

### Cookie-based Authentication

- Uses ASP.NET Core Cookie Authentication middleware
- Session expires after 7 days (or 30 days if "Remember Me" is checked)
- Password hashed using BCrypt with salt rounds = 12
- Claims stored in cookie: UserId, Email, FullName

### Business Rules:

1. **Registration:**
   - Email must be unique
   - Password minimum 6 characters
   - Email format validation

2. **Authorization:**
   - All expense and report endpoints require authentication
   - Users can only view/modify their own expenses (enforced at repository level)

## 💰 Expense Management Features

### Features:
- ✅ Dashboard with expense overview
- ✅ Add new expense
- ✅ Edit existing expense
- ✅ Delete expense
- ✅ View expense list with filtering
- ✅ Filter by category, date range, or month

### Business Rules:
- Expense amount must be greater than zero
- Users can only access their own expenses
- Category is required
- All fields validated through DTO attributes

## 📊 Reports Module

### Features:
- ✅ Monthly expense summary
- ✅ Category-wise expense breakdown
- ✅ Percentage calculations per category
- ✅ Visual progress bars

### Business Logic:
- Aggregates expenses by category
- Calculates totals and percentages
- Supports filtering by year and month

## 🎨 UI Features

- **Responsive Design** - Bootstrap 5 responsive layout
- **Clean Forms** - User-friendly forms with validation
- **Data Tables** - Sortable expense lists
- **Dashboard Cards** - Visual summary cards
- **Alert Messages** - Success/error notifications
- **Navigation Bar** - Easy navigation between sections

## 🚀 Getting Started

### Prerequisites

1. **.NET 8 SDK** - Download from [Microsoft](https://dotnet.microsoft.com/download)
2. **MongoDB** - Download from [MongoDB](https://www.mongodb.com/try/download/community)

### Installation Steps

1. **Clone or download the project**

2. **Start MongoDB:**
   ```bash
   # Windows
   mongod
   
   # Or use MongoDB as a service
   ```

3. **Configure MongoDB Connection:**
   
   Edit `appsettings.json`:
   ```json
   {
     "MongoDbSettings": {
       "ConnectionString": "mongodb://localhost:27017/",
       "DatabaseName": "ExpenseTracker"
     }
   }
   ```

4. **Restore NuGet packages:**
   ```bash
   dotnet restore
   ```

5. **Build the project:**
   ```bash
   dotnet build
   ```

6. **Run the application:**
   ```bash
   dotnet run
   ```

7. **Open browser:**
   - Navigate to: `https://localhost:7087` or `http://localhost:5037`
   - The exact port will be shown in the console

## 📝 Usage Guide

### 1. Register a New User

1. Click **"Register"** in the navigation bar
2. Fill in:
   - Full Name
   - Email (must be unique)
   - Password (minimum 6 characters)
3. Click **"Register"**
4. You'll be automatically logged in and redirected to the dashboard

### 2. Login

1. Click **"Login"** in the navigation bar
2. Enter your email and password
3. Optionally check **"Remember Me"** for extended session (30 days)
4. Click **"Login"**

### 3. Add an Expense

1. Click **"Expenses"** → **"Add New Expense"** (or use the button on dashboard)
2. Fill in the form:
   - **Title**: e.g., "Groceries", "Rent"
   - **Amount**: Must be greater than 0
   - **Category**: Select from dropdown or type custom category
   - **Date**: Select the expense date
   - **Notes**: Optional description
3. Click **"Create Expense"**

### 4. View Expenses

1. Click **"Expenses"** in the navigation bar
2. Use filters:
   - **Category**: Filter by specific category
   - **Year & Month**: Filter by specific month
   - **Date Range**: Filter by custom date range
3. Click **"Filter"** to apply filters
4. Click **"Clear Filters"** to reset

### 5. Edit Expense

1. Go to **"Expenses"** page
2. Click **"Edit"** button next to the expense
3. Modify the fields
4. Click **"Update Expense"**

### 6. Delete Expense

1. Go to **"Expenses"** page
2. Click **"Delete"** button next to the expense
3. Confirm deletion on the confirmation page
4. Click **"Delete Expense"**

### 7. View Reports

#### Monthly Report:
1. Click **"Monthly Report"** in navigation
2. Select Year and Month from dropdowns
3. View summary with category breakdown

#### Category Report:
1. Click **"Category Report"** in navigation
2. Optionally filter by Year and Month
3. View category-wise summary with percentages

## 🔍 Business Logic Explained

### Authentication Service

**Registration:**
- Checks if email already exists (business rule: unique email)
- Hashes password using BCrypt
- Creates user entity
- Persists to MongoDB

**Login:**
- Finds user by email
- Verifies password against hash
- Creates claims (UserId, Email, FullName)
- Signs in user with cookie

### Expense Service

**Create Expense:**
- Validates amount > 0 (business rule)
- Associates expense with current user (authorization)
- Persists to MongoDB

**Update/Delete:**
- Verifies expense belongs to current user (authorization)
- Updates or deletes only if user owns the expense

### Report Service

**Monthly Summary:**
- Filters expenses by userId and month
- Groups by category
- Calculates totals and percentages
- Returns aggregated data

**Category Summary:**
- Groups all expenses by category
- Calculates totals and percentages
- Supports optional month filtering

## 🧪 Testing

### Manual Testing Checklist:

- [x] User registration with valid data
- [x] User registration with duplicate email (should fail)
- [x] User login with valid credentials
- [x] User login with invalid credentials (should fail)
- [x] Add expense with valid data
- [x] Add expense with negative amount (should fail)
- [x] View own expenses
- [x] Edit own expense
- [x] Delete own expense
- [x] Filter expenses by category
- [x] Filter expenses by month
- [x] View monthly report
- [x] View category report
- [x] Logout functionality

## 📚 Key Concepts for Exams/Viva

### 1. **Layered Architecture**
- **Controllers**: HTTP request/response handling
- **Services**: Business logic layer
- **Repositories**: Data access layer
- **Models**: Database entities
- **DTOs**: Data transfer objects

### 2. **Dependency Injection**
- Constructor injection pattern
- Service lifetime: Scoped, Singleton, Transient
- Registered in `Program.cs`

### 3. **Cookie Authentication**
- Claims-based authentication
- Cookie middleware configuration
- Session management
- `[Authorize]` attribute usage

### 4. **MongoDB Integration**
- Document-based storage
- Async operations with `MongoDB.Driver`
- Index creation for performance
- Filtering and aggregation

### 5. **Business Logic Separation**
- All business rules in Services
- Controllers remain thin
- Repository pattern for data access

### 6. **DTO Pattern**
- Separate models from API contracts
- Validation attributes
- Security (no direct model exposure)

### 7. **Async/Await**
- Non-blocking I/O operations
- Improves scalability
- Used throughout the application

## 🛠️ Troubleshooting

### MongoDB Connection Issues

**Problem:** Cannot connect to MongoDB

**Solution:**
1. Ensure MongoDB is running: `mongod`
2. Check connection string in `appsettings.json`
3. Verify MongoDB port (default: 27017)

### Build Errors

**Problem:** Package restore fails

**Solution:**
```bash
dotnet clean
dotnet restore
dotnet build
```

### Cookie Authentication Not Working

**Problem:** User gets logged out immediately

**Solution:**
- Check cookie configuration in `Program.cs`
- Ensure HTTPS is used (or adjust `SecurePolicy`)
- Verify session middleware is configured

## 📄 License

This project is created for educational purposes.

## 👨‍💻 Author

Built with ASP.NET Core 8 MVC and MongoDB following industry best practices.

---

**Note:** This application is suitable for:
- Academic submissions
- Portfolio projects
- Learning ASP.NET Core MVC
- Understanding clean architecture
- Exam and viva preparation
