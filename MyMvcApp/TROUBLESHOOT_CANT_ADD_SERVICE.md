# Troubleshooting: Can't Add Service

## Common Reasons & Solutions

### ❌ Problem 1: Provider Account Not Approved (MOST COMMON)

**Symptom:**
- You see "Your account is pending approval by an administrator"
- You can't access `/Provider/AddService`
- You get redirected or see access denied

**Solution:**
1. **Check if you're approved:**
   ```sql
   SELECT Name, Email, IsApproved, Role
   FROM AspNetUsers
   WHERE Email = 'your-email@example.com';
   ```

2. **If `IsApproved = 0`, you need approval:**
   - Login as Admin: `admin@skillhub.com` / `Admin@123`
   - Go to: `http://localhost:5165/Admin/ManageProviders`
   - Find your account and click "Approve"

3. **Or approve via SQL:**
   ```sql
   UPDATE AspNetUsers
   SET IsApproved = 1
   WHERE Email = 'your-email@example.com' AND Role = 'Provider';
   ```

---

### ❌ Problem 2: Not Logged In as Provider

**Symptom:**
- You're logged in as Customer or not logged in at all
- You can't see "My Services" in navigation

**Solution:**
1. **Logout** if you're logged in as a different role
2. **Login as Provider:**
   - Email: `provider@skillhub.com`
   - Password: `Provider@123`
   - Or use your own provider account

3. **Verify you're logged in:**
   - Check navigation menu - you should see "My Services"
   - URL should show you're authenticated

---

### ❌ Problem 3: Wrong Role

**Symptom:**
- You registered as Customer instead of Provider
- You can't access provider features

**Solution:**
1. **Check your role:**
   ```sql
   SELECT Name, Email, Role
   FROM AspNetUsers
   WHERE Email = 'your-email@example.com';
   ```

2. **If Role = 'Customer', you need to:**
   - Register a new account with Role = 'Provider'
   - Or update your role in database (not recommended)

---

### ❌ Problem 4: Form Validation Errors

**Symptom:**
- You can access the Add Service page
- But when you submit, nothing happens or errors appear

**Solution:**
1. **Check all required fields:**
   - ✅ Title (required)
   - ✅ Category (required - must select one)
   - ✅ Description (required)
   - ✅ Price (required - must be a number)
   - ⚠️ Location (optional)

2. **Make sure Category exists:**
   - If no categories, login as Admin
   - Go to: `/Admin/ManageCategories`
   - Add a category first

3. **Check browser console** for JavaScript errors

---

### ❌ Problem 5: No Categories Available

**Symptom:**
- Category dropdown is empty
- Can't select a category

**Solution:**
1. **Login as Admin:**
   - Email: `admin@skillhub.com`
   - Password: `Admin@123`

2. **Add Categories:**
   - Go to: `http://localhost:5165/Admin/ManageCategories`
   - Click "Add Category"
   - Add categories like: Plumbing, Cleaning, Tutoring, etc.

3. **Or add via SQL:**
   ```sql
   INSERT INTO Categories (Name, Description)
   VALUES 
       ('Plumbing', 'Plumbing services'),
       ('Cleaning', 'Cleaning services'),
       ('Tutoring', 'Tutoring services');
   ```

---

### ❌ Problem 6: Access Denied / 403 Error

**Symptom:**
- You see "Access Denied" or 403 Forbidden
- Can't access provider pages

**Solution:**
1. **Check authorization:**
   - ProviderController requires: `[Authorize(Roles = "Provider", Policy = "ProviderApproved")]`
   - You need: Role = "Provider" AND IsApproved = 1

2. **Verify both conditions:**
   ```sql
   SELECT Name, Email, Role, IsApproved
   FROM AspNetUsers
   WHERE Email = 'your-email@example.com';
   ```

3. **Fix if needed:**
   - If Role != 'Provider': Register as Provider
   - If IsApproved = 0: Get admin approval

---

## Step-by-Step Diagnostic

### Step 1: Check Your Account Status

Run this SQL query:
```sql
SELECT 
    Name,
    Email,
    Role,
    IsApproved,
    EmailConfirmed,
    CASE 
        WHEN Role = 'Provider' AND IsApproved = 1 THEN '✅ Can Add Services'
        WHEN Role = 'Provider' AND IsApproved = 0 THEN '❌ Need Approval'
        WHEN Role = 'Customer' THEN '❌ Wrong Role - Need Provider Account'
        ELSE '❓ Unknown Status'
    END as Status
FROM AspNetUsers
WHERE Email = 'your-email@example.com';
```

### Step 2: Check Categories

```sql
SELECT * FROM Categories;
```

If empty, add categories first.

### Step 3: Try Accessing the Page

1. **Login as Provider** (approved account)
2. **Go to**: `http://localhost:5165/Provider/AddService`
3. **Check what happens:**
   - ✅ Page loads → Good! Try adding service
   - ❌ Redirected to login → Not logged in
   - ❌ Access denied → Not approved or wrong role
   - ❌ 404 Not Found → Wrong URL

### Step 4: Check Browser Console

1. Open browser Developer Tools (F12)
2. Go to Console tab
3. Try accessing Add Service page
4. Look for errors

---

## Quick Fix Checklist

- [ ] ✅ Logged in as Provider?
- [ ] ✅ Provider account is approved (IsApproved = 1)?
- [ ] ✅ Categories exist in database?
- [ ] ✅ Can access `/Provider/ManageServices`?
- [ ] ✅ Can see "My Services" in navigation?
- [ ] ✅ No JavaScript errors in browser console?
- [ ] ✅ All form fields filled correctly?

---

## Quick Solutions

### Solution 1: Use Pre-Approved Provider Account

1. **Login with:**
   - Email: `provider@skillhub.com`
   - Password: `Provider@123`

2. **This account is already approved** (from data seeding)

3. **Go to**: `http://localhost:5165/Provider/AddService`

### Solution 2: Approve Your Account

1. **Login as Admin:**
   - Email: `admin@skillhub.com`
   - Password: `Admin@123`

2. **Go to**: `http://localhost:5165/Admin/ManageProviders`

3. **Find your account** and click "Approve"

4. **Login as Provider** again

5. **Try adding service**

### Solution 3: Add Service via SQL (Bypass UI)

If you can't use the UI, add directly:

```sql
-- Get your ProviderId and CategoryId
DECLARE @ProviderId NVARCHAR(450);
DECLARE @CategoryId INT;

SELECT @ProviderId = Id FROM AspNetUsers WHERE Email = 'your-email@example.com';
SELECT @CategoryId = CategoryId FROM Categories WHERE Name = 'Plumbing';

-- Insert service
INSERT INTO Services (ProviderId, CategoryId, Title, Description, Price, Location, IsActive, CreatedAt)
VALUES (
    @ProviderId,
    @CategoryId,
    'Your Service Title',
    'Service description here',
    150.00,
    'Your Location',
    1,  -- Active
    GETUTCDATE()
);
```

---

## Still Having Issues?

1. **Check Application Logs:**
   - Look for errors in console output
   - Check for database connection issues

2. **Verify Database:**
   - Make sure database is running
   - Check connection string in `appsettings.json`

3. **Clear Browser Cache:**
   - Clear cookies and cache
   - Try incognito/private mode

4. **Check URL:**
   - Correct URL: `http://localhost:5165/Provider/AddService`
   - Make sure application is running

---

## Expected Behavior

When everything works correctly:

1. ✅ Login as Provider (approved)
2. ✅ See "My Services" in navigation
3. ✅ Click "My Services" → See Manage Services page
4. ✅ Click "Add New Service" → See Add Service form
5. ✅ Fill form → Submit → Service added successfully
6. ✅ Redirected to Manage Services with success message

---

## Need More Help?

If none of these solutions work:
1. Check the exact error message
2. Check browser console for errors
3. Check application logs
4. Verify database connection
5. Make sure application is running





