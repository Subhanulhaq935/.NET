-- SkillHub Database - Quick Data Check Queries
-- Run these in SQL Server Management Studio or sqlcmd

USE SkillHubDB;
GO

-- ============================================
-- 1. VIEW ALL REGISTERED USERS
-- ============================================
SELECT 
    Id,
    Name,
    Email,
    Role,
    IsApproved,
    EmailConfirmed,
    CreatedAt,
    CASE 
        WHEN Role = 'Provider' AND IsApproved = 0 THEN 'Pending Approval'
        WHEN Role = 'Provider' AND IsApproved = 1 THEN 'Approved'
        ELSE 'N/A'
    END as ApprovalStatus
FROM AspNetUsers
ORDER BY CreatedAt DESC;

-- ============================================
-- 2. USER COUNT BY ROLE
-- ============================================
SELECT 
    Role,
    COUNT(*) as TotalUsers,
    SUM(CASE WHEN IsApproved = 1 THEN 1 ELSE 0 END) as Approved,
    SUM(CASE WHEN IsApproved = 0 THEN 1 ELSE 0 END) as Pending
FROM AspNetUsers
GROUP BY Role;

-- ============================================
-- 3. VIEW ALL PROVIDERS
-- ============================================
SELECT 
    Name,
    Email,
    IsApproved,
    EmailConfirmed,
    CreatedAt
FROM AspNetUsers
WHERE Role = 'Provider'
ORDER BY CreatedAt DESC;

-- ============================================
-- 4. VIEW ALL CUSTOMERS
-- ============================================
SELECT 
    Name,
    Email,
    EmailConfirmed,
    CreatedAt
FROM AspNetUsers
WHERE Role = 'Customer'
ORDER BY CreatedAt DESC;

-- ============================================
-- 5. PENDING PROVIDER APPROVALS
-- ============================================
SELECT 
    Name,
    Email,
    CreatedAt
FROM AspNetUsers
WHERE Role = 'Provider' AND IsApproved = 0;

-- ============================================
-- 6. RECENT REGISTRATIONS (Last 10)
-- ============================================
SELECT TOP 10
    Name,
    Email,
    Role,
    CreatedAt
FROM AspNetUsers
ORDER BY CreatedAt DESC;

-- ============================================
-- 7. VIEW ALL CATEGORIES
-- ============================================
SELECT * FROM Categories;

-- ============================================
-- 8. VIEW ALL SERVICES WITH PROVIDER INFO
-- ============================================
SELECT 
    s.ServiceId,
    s.Title,
    s.Price,
    s.Location,
    s.IsActive,
    c.Name as Category,
    u.Name as ProviderName,
    u.Email as ProviderEmail,
    u.IsApproved as ProviderApproved
FROM Services s
INNER JOIN Categories c ON s.CategoryId = c.CategoryId
INNER JOIN AspNetUsers u ON s.ProviderId = u.Id
ORDER BY s.CreatedAt DESC;

-- ============================================
-- 9. VIEW ALL BOOKINGS WITH DETAILS
-- ============================================
SELECT 
    b.BookingId,
    s.Title as Service,
    cu.Name as CustomerName,
    cu.Email as CustomerEmail,
    pu.Name as ProviderName,
    b.BookingDateTime,
    b.Status,
    b.CreatedAt
FROM Bookings b
INNER JOIN Services s ON b.ServiceId = s.ServiceId
INNER JOIN AspNetUsers cu ON b.CustomerId = cu.Id
INNER JOIN AspNetUsers pu ON b.ProviderId = pu.Id
ORDER BY b.CreatedAt DESC;

-- ============================================
-- 10. VIEW ALL REVIEWS
-- ============================================
SELECT 
    r.ReviewId,
    r.Rating,
    r.Comment,
    u.Name as CustomerName,
    s.Title as Service,
    r.Date
FROM Reviews r
INNER JOIN AspNetUsers u ON r.CustomerId = u.Id
INNER JOIN Bookings b ON r.BookingId = b.BookingId
INNER JOIN Services s ON b.ServiceId = s.ServiceId
ORDER BY r.Date DESC;

-- ============================================
-- 11. SUMMARY STATISTICS
-- ============================================
SELECT 
    (SELECT COUNT(*) FROM AspNetUsers) as TotalUsers,
    (SELECT COUNT(*) FROM AspNetUsers WHERE Role = 'Admin') as TotalAdmins,
    (SELECT COUNT(*) FROM AspNetUsers WHERE Role = 'Provider') as TotalProviders,
    (SELECT COUNT(*) FROM AspNetUsers WHERE Role = 'Customer') as TotalCustomers,
    (SELECT COUNT(*) FROM AspNetUsers WHERE Role = 'Provider' AND IsApproved = 0) as PendingProviders,
    (SELECT COUNT(*) FROM Categories) as TotalCategories,
    (SELECT COUNT(*) FROM Services) as TotalServices,
    (SELECT COUNT(*) FROM Bookings) as TotalBookings,
    (SELECT COUNT(*) FROM Reviews) as TotalReviews;





