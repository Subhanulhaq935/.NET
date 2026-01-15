using ExpenseTracker.DTOs.Report;
using ExpenseTracker.Repositories;

namespace ExpenseTracker.Services;

/// <summary>
/// Service implementation for reports business logic
/// </summary>
public class ReportService : IReportService
{
    private readonly IExpenseRepository _expenseRepository;

    public ReportService(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    /// <summary>
    /// Gets monthly expense summary with business logic calculations
    /// Business Logic:
    /// - Calculate total amount for the month
    /// - Calculate total number of expenses
    /// - Group by category and calculate category-wise totals
    /// - Calculate percentage for each category
    /// </summary>
    public async Task<MonthlySummaryDto> GetMonthlySummaryAsync(string userId, int year, int month)
    {
        // Business logic validation
        if (month < 1 || month > 12)
        {
            throw new ArgumentException("Month must be between 1 and 12");
        }

        if (year < 1900 || year > 2100)
        {
            throw new ArgumentException("Year must be between 1900 and 2100");
        }

        // Get expenses for the month
        var expenses = await _expenseRepository.GetByUserIdAndMonthAsync(userId, year, month);

        // Business logic: Calculate totals
        var totalAmount = expenses.Sum(e => e.Amount);
        var totalExpenses = expenses.Count;

        // Business logic: Group by category and calculate category summaries
        var categoryGroups = expenses
            .GroupBy(e => e.Category)
            .Select(g => new CategorySummaryDto
            {
                Category = g.Key,
                TotalAmount = g.Sum(e => e.Amount),
                Count = g.Count(),
                Percentage = totalAmount > 0 ? (g.Sum(e => e.Amount) / totalAmount) * 100 : 0
            })
            .OrderByDescending(c => c.TotalAmount)
            .ToList();

        return new MonthlySummaryDto
        {
            Year = year,
            Month = month,
            TotalAmount = totalAmount,
            TotalExpenses = totalExpenses,
            CategorySummaries = categoryGroups
        };
    }

    /// <summary>
    /// Gets category-wise expense summary for all expenses
    /// Business Logic:
    /// - Group all expenses by category
    /// - Calculate totals and percentages
    /// </summary>
    public async Task<List<CategorySummaryDto>> GetCategorySummaryAsync(string userId)
    {
        var expenses = await _expenseRepository.GetByUserIdAsync(userId);

        // Business logic: Calculate total amount
        var totalAmount = expenses.Sum(e => e.Amount);

        // Business logic: Group by category
        var categoryGroups = expenses
            .GroupBy(e => e.Category)
            .Select(g => new CategorySummaryDto
            {
                Category = g.Key,
                TotalAmount = g.Sum(e => e.Amount),
                Count = g.Count(),
                Percentage = totalAmount > 0 ? (g.Sum(e => e.Amount) / totalAmount) * 100 : 0
            })
            .OrderByDescending(c => c.TotalAmount)
            .ToList();

        return categoryGroups;
    }

    /// <summary>
    /// Gets category-wise expense summary for a specific month
    /// Business Logic:
    /// - Group expenses by category for the specified month
    /// - Calculate totals and percentages
    /// </summary>
    public async Task<List<CategorySummaryDto>> GetCategorySummaryByMonthAsync(string userId, int year, int month)
    {
        // Business logic validation
        if (month < 1 || month > 12)
        {
            throw new ArgumentException("Month must be between 1 and 12");
        }

        if (year < 1900 || year > 2100)
        {
            throw new ArgumentException("Year must be between 1900 and 2100");
        }

        var expenses = await _expenseRepository.GetByUserIdAndMonthAsync(userId, year, month);

        // Business logic: Calculate total amount
        var totalAmount = expenses.Sum(e => e.Amount);

        // Business logic: Group by category
        var categoryGroups = expenses
            .GroupBy(e => e.Category)
            .Select(g => new CategorySummaryDto
            {
                Category = g.Key,
                TotalAmount = g.Sum(e => e.Amount),
                Count = g.Count(),
                Percentage = totalAmount > 0 ? (g.Sum(e => e.Amount) / totalAmount) * 100 : 0
            })
            .OrderByDescending(c => c.TotalAmount)
            .ToList();

        return categoryGroups;
    }
}