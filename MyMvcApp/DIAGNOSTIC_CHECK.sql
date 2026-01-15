-- Complete Diagnostic Check for Adding Services
-- Run this to check everything

USE SkillHubDB;
GO

PRINT '========================================';
PRINT 'DIAGNOSTIC CHECK FOR ADDING SERVICES';
PRINT '========================================';
PRINT '';

-- 1. Check Provider Account Status
PRINT '1. CHECKING PROVIDER ACCOUNT STATUS...';
PRINT '----------------------------------------';
SELECT 
    Name,
    Email,
    Role,
    IsApproved,
    EmailConfirmed,
    CASE 
        WHEN Role = 'Provider' AND IsApproved = 1 AND EmailConfirmed = 1 THEN '✅ READY TO ADD SERVICES'
        WHEN Role = 'Provider' AND IsApproved = 0 THEN '❌ NEED APPROVAL (IsApproved = 0)'
        WHEN Role = 'Provider' AND EmailConfirmed = 0 THEN '❌ EMAIL NOT CONFIRMED'
        WHEN Role != 'Provider' THEN '❌ WRONG ROLE (Current: ' + Role + ')'
        ELSE '❓ UNKNOWN STATUS'
    END as Status
FROM AspNetUsers
WHERE Email = 'provider@skillhub.com' OR Email LIKE '%@%';  -- Replace with your email
PRINT '';

-- 2. Check Categories
PRINT '2. CHECKING CATEGORIES...';
PRINT '----------------------------------------';
IF EXISTS (SELECT 1 FROM Categories)
BEGIN
    SELECT CategoryId, Name, Description FROM Categories;
    PRINT '✅ Categories exist';
END
ELSE
BEGIN
    PRINT '❌ NO CATEGORIES FOUND!';
    PRINT 'Run this to add categories:';
    PRINT 'INSERT INTO Categories (Name, Description) VALUES (''Plumbing'', ''Plumbing services''), (''Cleaning'', ''Cleaning services''), (''Tutoring'', ''Tutoring services'');';
END
PRINT '';

-- 3. Check Existing Services
PRINT '3. CHECKING EXISTING SERVICES...';
PRINT '----------------------------------------';
SELECT 
    s.ServiceId,
    s.Title,
    s.Price,
    c.Name as Category,
    u.Name as Provider,
    s.IsActive
FROM Services s
INNER JOIN Categories c ON s.CategoryId = c.CategoryId
INNER JOIN AspNetUsers u ON s.ProviderId = u.Id
ORDER BY s.CreatedAt DESC;
PRINT '';

-- 4. Check Provider Approval Status (All Providers)
PRINT '4. CHECKING ALL PROVIDERS...';
PRINT '----------------------------------------';
SELECT 
    Name,
    Email,
    IsApproved,
    CASE WHEN IsApproved = 1 THEN '✅ Approved' ELSE '❌ Pending' END as ApprovalStatus,
    CreatedAt
FROM AspNetUsers
WHERE Role = 'Provider'
ORDER BY CreatedAt DESC;
PRINT '';

-- 5. Quick Fix Commands
PRINT '========================================';
PRINT 'QUICK FIX COMMANDS';
PRINT '========================================';
PRINT '';
PRINT 'To approve a provider:';
PRINT 'UPDATE AspNetUsers SET IsApproved = 1 WHERE Email = ''your-email@example.com'';';
PRINT '';
PRINT 'To add categories:';
PRINT 'INSERT INTO Categories (Name, Description) VALUES (''Plumbing'', ''Plumbing services'');';
PRINT '';
PRINT 'To add a service directly (if UI fails):';
PRINT 'DECLARE @ProviderId NVARCHAR(450);';
PRINT 'DECLARE @CategoryId INT;';
PRINT 'SELECT @ProviderId = Id FROM AspNetUsers WHERE Email = ''provider@skillhub.com'';';
PRINT 'SELECT @CategoryId = CategoryId FROM Categories WHERE Name = ''Plumbing'';';
PRINT 'INSERT INTO Services (ProviderId, CategoryId, Title, Description, Price, Location, IsActive, CreatedAt)';
PRINT 'VALUES (@ProviderId, @CategoryId, ''Test Service'', ''Description'', 100.00, ''Location'', 1, GETUTCDATE());';
PRINT '';





