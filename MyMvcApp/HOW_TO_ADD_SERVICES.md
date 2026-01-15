# How to Add More Services to SkillHub

There are several ways to add services to the platform. Choose the method that best fits your needs.

---

## Method 1: Through Provider Dashboard (Recommended) ✅

This is the **easiest and most common way** for service providers to add their services.

### Steps:

1. **Login as Provider**:
   - Go to: `http://localhost:5165/Account/Login`
   - Email: `provider@skillhub.com`
   - Password: `Provider@123`

2. **Navigate to Manage Services**:
   - Click on **"Manage Services"** in the navigation menu
   - Or go directly to: `http://localhost:5165/Provider/ManageServices`

3. **Click "Add New Service"**:
   - You'll see a button at the top right: **"Add New Service"**
   - Or go directly to: `http://localhost:5165/Provider/AddService`

4. **Fill in the Service Form**:
   - **Title**: Name of your service (e.g., "Plumbing Repair", "Math Tutoring")
   - **Category**: Select from existing categories (Plumbing, Cleaning, Tutoring, etc.)
   - **Description**: Detailed description of what the service includes
   - **Price**: Service price in dollars (e.g., 150.00)
   - **Location**: Where the service is available (e.g., "Downtown", "Citywide", "Online")

5. **Submit**:
   - Click **"Add Service"** button
   - The service will be saved and immediately visible to customers

### Example Service:
- **Title**: "Home Electrical Repair"
- **Category**: "Plumbing" (or create a new "Electrical" category first)
- **Description**: "Professional electrical repair services for homes. Fix outlets, wiring, circuit breakers, and more."
- **Price**: 175.00
- **Location**: "Citywide"

---

## Method 2: Through Admin Panel (For Admins)

Admins can also add services, but typically services are added by providers themselves.

### Steps:

1. **Login as Admin**:
   - Go to: `http://localhost:5165/Account/Login`
   - Email: `admin@skillhub.com`
   - Password: `Admin@123`

2. **Note**: Currently, admins manage categories, but services are typically added by providers. If you need admin-level service management, this feature can be added.

---

## Method 3: Direct SQL Insert (For Bulk Adding)

Use this method if you need to add multiple services quickly via SQL.

### Steps:

1. **Open SQL Server Management Studio** (SSMS)
2. **Connect to**: `(localdb)\MSSQLLocalDB`
3. **Select Database**: `SkillHubDB`

### SQL Query Template:

```sql
-- First, get the ProviderId and CategoryId
DECLARE @ProviderId NVARCHAR(450);
DECLARE @CategoryId INT;

-- Get Provider ID (replace email with actual provider email)
SELECT @ProviderId = Id FROM AspNetUsers WHERE Email = 'provider@skillhub.com';

-- Get Category ID (replace name with actual category name)
SELECT @CategoryId = CategoryId FROM Categories WHERE Name = 'Plumbing';

-- Insert the service
INSERT INTO Services (ProviderId, CategoryId, Title, Description, Price, Location, IsActive, CreatedAt)
VALUES (
    @ProviderId,
    @CategoryId,
    'Service Title Here',
    'Service description here. What does this service include?',
    150.00,  -- Price
    'Location Here',  -- Location
    1,  -- IsActive (1 = Active, 0 = Inactive)
    GETUTCDATE()  -- CreatedAt
);
```

### Example: Adding Multiple Services:

```sql
-- Get Provider and Categories
DECLARE @ProviderId NVARCHAR(450);
DECLARE @PlumbingCategory INT;
DECLARE @CleaningCategory INT;
DECLARE @TutoringCategory INT;

SELECT @ProviderId = Id FROM AspNetUsers WHERE Email = 'provider@skillhub.com';
SELECT @PlumbingCategory = CategoryId FROM Categories WHERE Name = 'Plumbing';
SELECT @CleaningCategory = CategoryId FROM Categories WHERE Name = 'Cleaning';
SELECT @TutoringCategory = CategoryId FROM Categories WHERE Name = 'Tutoring';

-- Add multiple services
INSERT INTO Services (ProviderId, CategoryId, Title, Description, Price, Location, IsActive, CreatedAt)
VALUES
    (@ProviderId, @PlumbingCategory, 'Bathroom Renovation', 'Complete bathroom renovation services including plumbing, tiling, and fixtures.', 5000.00, 'Citywide', 1, GETUTCDATE()),
    (@ProviderId, @CleaningCategory, 'Office Cleaning', 'Professional office cleaning services for businesses. Weekly or monthly contracts available.', 300.00, 'Downtown', 1, GETUTCDATE()),
    (@ProviderId, @TutoringCategory, 'Science Tutoring', 'Expert science tutoring for middle and high school students. Physics, Chemistry, Biology.', 60.00, 'Online/In-Person', 1, GETUTCDATE());
```

---

## Method 4: Programmatically via Data Seeding

You can add services programmatically by modifying the `DataSeeder.cs` file.

### Steps:

1. **Open**: `Services/DataSeeder.cs`
2. **Find**: The `SeedSampleServicesAsync` method
3. **Add more services** to the `services` array:

```csharp
var services = new[]
{
    // Existing services...
    new Service
    {
        ProviderId = provider.Id,
        CategoryId = plumbingCategory.CategoryId,
        Title = "Your New Service Title",
        Description = "Description of your new service.",
        Price = 200.00m,
        Location = "Your Location",
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    },
    // Add more services here...
};
```

4. **Restart the application** - Services will be seeded automatically

---

## Method 5: Create a New Provider Account

If you want to add services as a different provider:

### Steps:

1. **Register a New Provider**:
   - Go to: `http://localhost:5165/Account/Register`
   - Fill in the registration form
   - Select **Role**: "Provider"
   - Submit registration

2. **Admin Approval** (Required):
   - Login as Admin: `admin@skillhub.com` / `Admin@123`
   - Go to: `http://localhost:5165/Admin/ManageProviders`
   - Find the new provider
   - Click **"Approve"** button

3. **Provider Adds Services**:
   - Login as the new provider
   - Go to: `http://localhost:5165/Provider/ManageServices`
   - Click **"Add New Service"**
   - Fill in the form and submit

---

## Available Categories

Before adding a service, make sure the category exists. Current categories include:

- **Plumbing**
- **Cleaning**
- **Tutoring**

### To Add a New Category:

1. **Login as Admin**
2. Go to: `http://localhost:5165/Admin/ManageCategories`
3. Click **"Add Category"**
4. Enter category name and description
5. Submit

---

## Service Fields Explained

| Field | Required | Description | Example |
|-------|----------|-------------|---------|
| **Title** | Yes | Name of the service | "Emergency Plumbing Repair" |
| **Category** | Yes | Service category | "Plumbing" |
| **Description** | Yes | Detailed service description | "24/7 emergency plumbing services..." |
| **Price** | Yes | Service price in dollars | 150.00 |
| **Location** | Optional | Where service is available | "Downtown", "Citywide", "Online" |
| **IsActive** | Auto | Service status (Active/Inactive) | Automatically set to Active |

---

## Quick Access URLs

When application is running on `http://localhost:5165`:

- **Provider Dashboard**: `/Provider`
- **Manage Services**: `/Provider/ManageServices`
- **Add Service**: `/Provider/AddService`
- **Edit Service**: `/Provider/EditService/{id}`
- **Admin - Manage Categories**: `/Admin/ManageCategories`

---

## Tips

1. **Service Visibility**: Only **Active** services are visible to customers
2. **Provider Approval**: Providers must be approved by admin before their services appear
3. **Categories**: Create categories first if you need new ones
4. **Pricing**: Be competitive but realistic with pricing
5. **Descriptions**: Write clear, detailed descriptions to attract customers
6. **Location**: Specify location to help customers find local services

---

## Troubleshooting

### Service Not Appearing?

1. **Check Provider Status**: Provider must be approved (`IsApproved = 1`)
2. **Check Service Status**: Service must be active (`IsActive = 1`)
3. **Check Category**: Category must exist
4. **Refresh Browser**: Clear cache and refresh

### Can't Add Service?

1. **Check Login**: Make sure you're logged in as a Provider
2. **Check Approval**: Provider account must be approved by admin
3. **Check Form**: All required fields must be filled
4. **Check Validation**: Price must be a valid number

---

## Example Services to Add

Here are some example services you can add:

### Plumbing Services:
- "Kitchen Sink Installation" - $200
- "Water Heater Repair" - $250
- "Drain Cleaning" - $100

### Cleaning Services:
- "Carpet Cleaning" - $150
- "Window Cleaning" - $120
- "Move-In/Move-Out Cleaning" - $300

### Tutoring Services:
- "English Literature Tutoring" - $45
- "Computer Science Tutoring" - $55
- "SAT/ACT Prep" - $75

---

## Need Help?

If you encounter any issues:
1. Check the application logs
2. Verify database connection
3. Ensure all required fields are filled
4. Check that provider is approved





