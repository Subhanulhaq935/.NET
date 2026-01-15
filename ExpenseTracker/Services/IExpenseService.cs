using ExpenseTracker.DTOs.Expense;

namespace ExpenseTracker.Services;

/// <summary>
/// Service interface for expense management business logic
/// </summary>
public interface IExpenseService
{
    Task<ExpenseResponseDto> CreateExpenseAsync(string userId, CreateExpenseDto createDto);
    Task<ExpenseResponseDto?> GetExpenseByIdAsync(string userId, string expenseId);
    Task<List<ExpenseResponseDto>> GetUserExpensesAsync(string userId);
    Task<List<ExpenseResponseDto>> GetExpensesByDateRangeAsync(string userId, DateTime startDate, DateTime endDate);
    Task<List<ExpenseResponseDto>> GetExpensesByCategoryAsync(string userId, string category);
    Task<List<ExpenseResponseDto>> GetExpensesByMonthAsync(string userId, int year, int month);
    Task<ExpenseResponseDto?> UpdateExpenseAsync(string userId, string expenseId, UpdateExpenseDto updateDto);
    Task<bool> DeleteExpenseAsync(string userId, string expenseId);
}