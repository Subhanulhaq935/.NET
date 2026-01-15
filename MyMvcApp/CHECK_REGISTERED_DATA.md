# How to Check Registered Data

## Method 1: Through Admin Dashboard (Easiest) ✅

### Steps:
1. **Login as Admin**:
   - Go to: `http://localhost:5165/Account/Login`
   - Email: `admin@skillhub.com`
   - Password: `Admin@123`

2. **View All Users**:
   - Click on **"All Users"** in the navigation menu
   - Or go to: `http://localhost:5165/Admin/ManageUsers`
   - You'll see a table with all registered users showing:
     - Name, Email, Role
     - Approval Status
     - Email Confirmed Status
     - Created Date

3. **View Providers Only**:
   - Click on **"Providers"** in the navigation menu
   - Or go to: `http://localhost:5165/Admin/ManageProviders`

4. **View Dashboard Statistics**:
   - Go to Admin Dashboard: `http://localhost:5165/Admin`
   - See total counts of users, providers, customers

---

## Method 2: Using SQL Server Management Studio (SSMS)

### Connect to Database:
1. Open **SQL Server Management Studio**
2. Connect to: `(localdb)\MSSQLLocalDB`
3. Database: `SkillHubDB`

### Useful SQL Queries:

#### View All Users:
```sql
SELECT 
    Id,
    Name,
    Email,
    Role,
    IsApproved,
    EmailConfirmed,
    CreatedAt
FROM AspNetUsers
ORDER BY CreatedAt DESC;
```

#### View Users by Role:
```sql
-- All Providers
SELECT * FROM AspNetUsers WHERE Role = 'Provider';

-- All Customers
SELECT * FROM AspNetUsers WHERE Role = 'Customer';

-- All Admins
SELECT * FROM AspNetUsers WHERE Role = 'Admin';
```

#### View Pending Providers:
```sql
SELECT * FROM AspNetUsers 
WHERE Role = 'Provider' AND IsApproved = 0;
```

#### View User Registration Count:
```sql
SELECT 
    Role,
    COUNT(*) as TotalUsers,
    SUM(CASE WHEN IsApproved = 1 THEN 1 ELSE 0 END) as Approved,
    SUM(CASE WHEN IsApproved = 0 THEN 1 ELSE 0 END) as Pending
FROM AspNetUsers
GROUP BY Role;
```

#### View Recent Registrations:
```sql
SELECT TOP 10
    Name,
    Email,
    Role,
    CreatedAt
FROM AspNetUsers
ORDER BY CreatedAt DESC;
```

---

## Method 3: Using Command Line (sqlcmd)

### Connect to LocalDB:
```bash
sqlcmd -S "(localdb)\MSSQLLocalDB" -d SkillHubDB
```

### Run Queries:
```sql
-- View all users
SELECT Name, Email, Role, IsApproved, CreatedAt FROM AspNetUsers;

-- Exit
GO
EXIT
```

---

## Method 4: Check via Application URLs

### Admin Dashboard:
- URL: `http://localhost:5165/Admin`
- Shows: Total users, providers, customers counts

### All Users Page:
- URL: `http://localhost:5165/Admin/ManageUsers`
- Shows: Complete list of all registered users

### Providers Page:
- URL: `http://localhost:5165/Admin/ManageProviders`
- Shows: Only provider users

---

## Default Seeded Users

After running the application, these users are automatically created:

| Name | Email | Role | Password | Approved |
|------|-------|------|----------|----------|
| Admin User | admin@skillhub.com | Admin | Admin@123 | Yes |
| John Provider | provider@skillhub.com | Provider | Provider@123 | Yes |
| Jane Customer | customer@skillhub.com | Customer | Customer@123 | Yes |

---

## Check Other Data

### Categories:
```sql
SELECT * FROM Categories;
```

### Services:
```sql
SELECT 
    s.ServiceId,
    s.Title,
    s.Price,
    c.Name as Category,
    u.Name as ProviderName
FROM Services s
INNER JOIN Categories c ON s.CategoryId = c.CategoryId
INNER JOIN AspNetUsers u ON s.ProviderId = u.Id;
```

### Bookings:
```sql
SELECT 
    b.BookingId,
    s.Title as Service,
    cu.Name as Customer,
    pu.Name as Provider,
    b.BookingDateTime,
    b.Status
FROM Bookings b
INNER JOIN Services s ON b.ServiceId = s.ServiceId
INNER JOIN AspNetUsers cu ON b.CustomerId = cu.Id
INNER JOIN AspNetUsers pu ON b.ProviderId = pu.Id;
```

### Reviews:
```sql
SELECT 
    r.ReviewId,
    r.Rating,
    r.Comment,
    u.Name as CustomerName,
    r.Date
FROM Reviews r
INNER JOIN AspNetUsers u ON r.CustomerId = u.Id;
```

---

## Quick Access Links

When application is running on `http://localhost:5165`:

- **Admin Dashboard**: `/Admin`
- **All Users**: `/Admin/ManageUsers`
- **Providers**: `/Admin/ManageProviders`
- **All Bookings**: `/Admin/AllBookings`
- **Categories**: `/Admin/ManageCategories`

---

## Tips

1. **Filter by Role**: Use the SQL queries above to filter users by role
2. **Check Registration Date**: Sort by `CreatedAt` to see newest registrations
3. **Check Approval Status**: Look for `IsApproved = 0` for pending providers
4. **Export Data**: Use the Export Excel/PDF features in Admin Dashboard for bookings





