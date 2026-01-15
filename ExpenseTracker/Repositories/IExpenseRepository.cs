using ExpenseTracker.Models;

namespace ExpenseTracker.Repositories;

/// <summary>
/// Repository interface for Expense data access operations
/// </summary>
public interface IExpenseRepository
{
    Task<Expense> CreateAsync(Expense expense);
    Task<Expense?> GetByIdAsync(string id, string userId);
    Task<List<Expense>> GetByUserIdAsync(string userId);
    Task<List<Expense>> GetByUserIdAndDateRangeAsync(string userId, DateTime startDate, DateTime endDate);
    Task<List<Expense>> GetByUserIdAndCategoryAsync(string userId, string category);
    Task<List<Expense>> GetByUserIdAndMonthAsync(string userId, int year, int month);
    Task<Expense?> UpdateAsync(string id, string userId, Expense expense);
    Task<bool> DeleteAsync(string id, string userId);
}