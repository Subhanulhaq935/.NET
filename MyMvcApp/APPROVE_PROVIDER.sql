-- Quick SQL to approve a provider account
-- Replace 'your-email@example.com' with the actual provider email

USE SkillHubDB;
GO

-- View pending providers
SELECT 
    Name,
    Email,
    IsApproved,
    CreatedAt
FROM AspNetUsers
WHERE Role = 'Provider' AND IsApproved = 0;

-- Approve a specific provider by email
UPDATE AspNetUsers
SET IsApproved = 1
WHERE Email = 'your-email@example.com' AND Role = 'Provider';

-- Verify approval
SELECT 
    Name,
    Email,
    IsApproved,
    CASE 
        WHEN IsApproved = 1 THEN 'Approved'
        ELSE 'Pending'
    END as Status
FROM AspNetUsers
WHERE Email = 'your-email@example.com';





