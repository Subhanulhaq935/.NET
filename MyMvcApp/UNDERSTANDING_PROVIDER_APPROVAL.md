# Understanding Provider Approval in SkillHub

## What is Provider Approval?

In SkillHub, **Provider accounts require administrator approval** before they can:
- Add services
- Manage bookings
- Set availability
- Access provider dashboard

This is a **security and quality control measure** to ensure only legitimate service providers can offer services on the platform.

---

## Why Do Providers Need Approval?

1. **Quality Control**: Ensures only legitimate businesses/professionals offer services
2. **Security**: Prevents spam accounts and fake service listings
3. **Customer Trust**: Customers know all providers are verified
4. **Platform Integrity**: Maintains the quality of services on the platform

---

## Account Types & Approval Status

| Account Type | Auto-Approved? | Needs Admin Approval? |
|--------------|----------------|----------------------|
| **Customer** | ✅ Yes | ❌ No |
| **Provider** | ❌ No | ✅ Yes |
| **Admin** | ✅ Yes | ❌ No |

---

## How to Check Your Approval Status

### Method 1: Try to Login
- If you see: **"Your account is pending approval by an administrator"**
- Status: **Pending** ❌

### Method 2: Check via SQL
```sql
SELECT 
    Name,
    Email,
    IsApproved,
    CASE 
        WHEN IsApproved = 1 THEN 'Approved ✅'
        ELSE 'Pending ❌'
    END as Status
FROM AspNetUsers
WHERE Email = 'your-email@example.com';
```

### Method 3: Admin Dashboard
- Login as Admin
- Go to: `/Admin/ManageProviders`
- Find your account and check the status badge

---

## How to Get Approved (For Providers)

### Step 1: Wait for Admin
- Contact the platform administrator
- Or wait for them to review your account

### Step 2: Admin Approves
- Admin logs in
- Goes to: `/Admin/ManageProviders`
- Clicks "Approve" button next to your account

### Step 3: Login Again
- Once approved, you can login normally
- You'll have full access to provider features

---

## How to Approve Providers (For Admins)

### Via Web Interface:

1. **Login as Admin**:
   - Email: `admin@skillhub.com`
   - Password: `Admin@123`

2. **Navigate to Manage Providers**:
   - Click "Providers" in navigation
   - Or go to: `http://localhost:5165/Admin/ManageProviders`

3. **Find Pending Providers**:
   - Look for accounts with "Pending" badge
   - Check their email and registration date

4. **Approve Provider**:
   - Click the green **"Approve"** button
   - Provider account is now active

5. **Verify Approval**:
   - Status changes to "Approved" ✅
   - Provider can now login and use features

### Via SQL (Direct Database):

```sql
-- View all pending providers
SELECT Name, Email, CreatedAt
FROM AspNetUsers
WHERE Role = 'Provider' AND IsApproved = 0;

-- Approve a specific provider
UPDATE AspNetUsers
SET IsApproved = 1
WHERE Email = 'provider-email@example.com' AND Role = 'Provider';

-- Verify approval
SELECT Name, Email, IsApproved
FROM AspNetUsers
WHERE Email = 'provider-email@example.com';
```

---

## What Happens After Approval?

Once approved, providers can:
- ✅ Login to their account
- ✅ Access Provider Dashboard
- ✅ Add new services
- ✅ Manage their services (edit/delete)
- ✅ View and manage bookings
- ✅ Set availability calendar
- ✅ View customer reviews and ratings

---

## Troubleshooting

### "I'm approved but still can't login"
1. **Check Email Confirmation**: Make sure email is confirmed
2. **Clear Browser Cache**: Clear cookies and cache
3. **Try Incognito Mode**: Test in private browsing
4. **Check Database**: Verify `IsApproved = 1` in database

### "I don't see the Approve button"
1. **Check Admin Role**: Make sure you're logged in as Admin
2. **Check Provider Status**: Provider might already be approved
3. **Refresh Page**: Try refreshing the page

### "How do I know if I'm approved?"
1. **Try Logging In**: If you can login, you're approved
2. **Check Admin Panel**: Admin can see your status
3. **SQL Query**: Run the SQL query above

---

## Quick Reference

### For Providers:
- **Status**: Pending → Contact admin or wait
- **Status**: Approved → You can login and use features

### For Admins:
- **Approve**: `/Admin/ManageProviders` → Click "Approve"
- **Disable**: `/Admin/ManageProviders` → Click "Disable"

### SQL Commands:
- **Check Status**: `SELECT IsApproved FROM AspNetUsers WHERE Email = '...'`
- **Approve**: `UPDATE AspNetUsers SET IsApproved = 1 WHERE Email = '...'`
- **Disable**: `UPDATE AspNetUsers SET IsApproved = 0 WHERE Email = '...'`

---

## Security Notes

- ⚠️ Only Admins can approve providers
- ⚠️ Providers cannot approve themselves
- ⚠️ Approval is required for security
- ⚠️ Disabled providers cannot access provider features

---

## Need Help?

If you're stuck:
1. Check this guide
2. Verify your account status in database
3. Contact the platform administrator
4. Check application logs for errors





